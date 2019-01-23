using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrHeredades.Models;
using PrHeredades.Tags;

namespace PrHeredades.Controllers
{
    [TagAutenticacion]
    [TagPermiso(permiso = EnumPermisos.Deudor)]
    public class DeudorController : Controller
    {
        private readonly int registrosPagina = 10;
        // GET: Deudor
        public ActionResult Index(int pagina = 1, string filtro = "")
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbDeudor> lista = (from t in db.tbDeudor
                                    where t.nombre.Contains(filtro)
                                    orderby t.nombre
                                    select t).ToList();
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Index", "Deudor");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        public ActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(tbDeudor nuevo)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            if (!(db.tbDeudor.Any(t => t.nombre == nuevo.nombre)))
            {
                nuevo.deuda = 0;
                db.tbDeudor.Add(nuevo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "¡Ya existe este proveedor!");
                return View(nuevo);
            }
        }

        public ActionResult Editar(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbDeudor deudor = db.tbDeudor.Find(id);
            return View(deudor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(tbDeudor deudor)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            if (!(db.tbDeudor.Any(t => t.nombre == deudor.nombre && t.codDeudor != deudor.codDeudor)))
            {
                db.Entry(deudor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "¡Ya existe este deudor!");
                return View(deudor);
            }
        }

        public ActionResult Pagos(int id, int pagina = 1)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbPagoDeudor> lista = db.tbPagoDeudor.Where(t => t.codDeudor == id).OrderByDescending(t => t.fecha).ToList();
            tbDeudor deudor = db.tbDeudor.Find(id);
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(id, pagina, paginas, "Pagos", "Deudor");
            ViewBag.paginacion = paginacion;
            ViewBag.deudor = deudor.nombre;
            ViewBag.codDeudor = deudor.codDeudor;
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        public ActionResult Pagar(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbDeudor deudor = db.tbDeudor.Find(id);
            ViewBag.deudor = deudor.nombre;
            ViewBag.codDeudor = deudor.codDeudor;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Pagar(int codDeudor, decimal pago)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbPagoDeudor debitar = new tbPagoDeudor
            {
                codDeudor = codDeudor,
                codUsuario = Sesion.ObtenerCodigo(),
                pago = pago,
                fecha = DateTime.Now
            };
            db.tbPagoDeudor.Add(debitar);
            tbDeudor deudor = db.tbDeudor.Find(codDeudor);
            deudor.deuda -= pago;
            tbCaja caja = db.tbCaja.Find(1);
            caja.cantidad += pago;
            db.SaveChanges();
            return RedirectToAction("Pagos", new { id = codDeudor });
        }

        public ActionResult DetallePago(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbPagoDeudor pago = db.tbPagoDeudor.Find(id);
            return View(pago);
        }

        public ActionResult Compras(int id, int pagina = 1)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbVenta> lista = db.tbVenta.Where(t => t.codDeudor == id).OrderByDescending(t => t.fecha).ToList();
            tbDeudor deudor = db.tbDeudor.Find(id);
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(id, pagina, paginas, "Pagos", "Deudor");
            ViewBag.paginacion = paginacion;
            ViewBag.deudor = deudor.nombre;
            ViewBag.codDeudor = deudor.codDeudor;
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        public ActionResult DetalleCompra(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbVenta venta = db.tbVenta.Find(id);
            decimal total = 0;
            foreach (tbVentaProducto item in venta.tbVentaProducto)
            {
                total += item.precioVenta * item.cantidad;
            }
            ViewBag.total = Math.Truncate(total * 100) / 100;
            ViewBag.codDeudor = venta.codDeudor;
            return View(venta);
        }

        [HttpPost]
        public int CrearDeudor(tbDeudor deudor)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            deudor.deuda = 0;
            db.tbDeudor.Add(deudor);
            db.SaveChanges();
            return deudor.codDeudor;
        }
    }
}