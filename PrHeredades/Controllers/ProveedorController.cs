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
    public class ProveedorController : Controller
    {
        private readonly int registrosPagina = 10;
        // GET: Proveedor
        public ActionResult Index(int pagina = 1, string filtro = "", bool estado = true)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbProveedor> lista = (from t in db.tbProveedor
                                       where t.proveedor.Contains(filtro) && t.estado == estado
                                       orderby t.proveedor
                                       select t).ToList();
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Index", "Proveedor");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            ViewBag.estado = estado;
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        public ActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(tbProveedor nuevo)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            if (!(db.tbProveedor.Any(t => t.proveedor == nuevo.proveedor)))
            {
                nuevo.estado = true;
                nuevo.deuda = 0;
                db.tbProveedor.Add(nuevo);
                db.SaveChanges();
                return RedirectToAction("Productos", new { id = nuevo.codProveedor  });
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
            return View(db.tbProveedor.Find(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(tbProveedor editado)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            if (!(db.tbProveedor.Any(t => t.proveedor == editado.proveedor && t.codProveedor != editado.codProveedor)))
            {
                db.Entry(editado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "¡Ya existe este proveedor!");
                return View(editado);
            }
        }

        public ActionResult CambiarEstado(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbProveedor proveedor = db.tbProveedor.Find(id);
            return View(proveedor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarEstado(int id, FormCollection collection)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbProveedor proveedor = db.tbProveedor.Find(id);
            proveedor.estado = !(proveedor.estado.Value);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Productos(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbProveedor proveedor = db.tbProveedor.Find(id);
            ViewBag.proveedor = proveedor.proveedor;
            ViewBag.codproveedor = proveedor.codProveedor;
            List<tbProducto> productos = db.tbProducto.Where(t => t.estado.Value).OrderBy(t => t.producto).ToList();
            ViewBag.codProducto = new SelectList(productos, "codProducto", "producto");
            ViewBag.codPresentacion = new SelectList(new List<tbPresentacion>(), "codPresentacion", "presentacion");
            ViewBag.productos = (from t in db.tbProductoProveedor
                                 where t.codProveedor == id && t.estado.Value
                                 orderby t.tbProductoPresentacion.correlativo descending
                                 select new
                                 {
                                     t.codProveedor,
                                     t.codProducto,
                                     t.codPresentacion,
                                     t.precioCompra,
                                     t.estado
                                 }).ToList();
            return View();
        }

        [HttpPost]
        public int GuardarProductos(List<tbProductoProveedor> modelo)
        {
            try
            {
                dbHeredadesEntities db = new dbHeredadesEntities();
                int codProveedor = modelo[0].codProveedor;
                List<tbProductoProveedor> guardadas = db.tbProductoProveedor.Where(t => t.codProveedor == codProveedor).ToList();
                List<int> deshabilitar = new List<int>();
                foreach (tbProductoProveedor item in modelo)
                {
                    if (guardadas.Any(t => t.codProveedor == item.codProveedor && t.codProducto == item.codProducto && t.codPresentacion == item.codPresentacion))
                    {
                        //esta en bd
                        int index = guardadas.FindIndex(t => t.codProveedor == item.codProveedor && t.codProducto == item.codProducto && t.codPresentacion == item.codPresentacion);
                        if (index > -1)
                        {
                            guardadas[index].precioCompra = item.precioCompra;
                            guardadas[index].estado = true;
                        }
                        deshabilitar.Add(index);
                    }
                    else
                    {
                        //no esta en bd
                        db.tbProductoProveedor.Add(item);
                    }
                }
                for (int i = 0; i < guardadas.Count; i++)
                {
                    if (!deshabilitar.Contains(i))
                    {
                        guardadas[i].estado = false;
                    }
                }
                db.SaveChanges();
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        [HttpPost]
        public JsonResult CargarPresentaciones(int codProducto)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            var lista = (from prod in db.tbProductoPresentacion
                         join pres in db.tbPresentacion on prod.codPresentacion equals pres.codPresentacion
                         where prod.codProducto == codProducto && prod.correlativo > 0
                         orderby prod.correlativo descending
                         select new
                         {
                             pres.codPresentacion,
                             pres.presentacion
                         }).ToList();
            return Json(lista);
        }

        [HttpPost]
        public ActionResult ListarProductos(List<tbProductoProveedor> lista)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            if (lista != null)
            {
                foreach (var item in lista)
                {
                    item.tbProductoPresentacion = db.tbProductoPresentacion.Find(item.codProducto, item.codPresentacion);
                }
            }
            return PartialView("_Productos", lista);
        }
    }
}