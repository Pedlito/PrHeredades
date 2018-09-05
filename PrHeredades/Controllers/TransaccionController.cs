using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrHeredades.Models;
using PrHeredades.Tags;

namespace PrHeredades.Controllers
{
    public class TransaccionController : Controller
    {
        // las transacciones son 1 para entradas, 2 para salidas y 3 para ventas
        private readonly int registrosPagina = 10;
        // GET: Transaccion
        public ActionResult Entradas(int pagina = 1, string filtro = null, bool estado = true)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbTransaccion> lista = (from t in db.tbTransaccion
                                         where t.codTipoTransaccion == 1 && t.estado == true
                                         orderby t.fecha
                                         select t).ToList();
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Index", "Categoria");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            ViewBag.estado = estado;
            ViewBag.proveedores = db.tbProveedor.OrderBy(t => t.proveedor).ToList();
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        public ActionResult CrearEntrada(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbProveedor proveedor = db.tbProveedor.Find(id);
            ViewBag.codProveedor = id;
            ViewBag.proveedor = proveedor.proveedor;
            List<tbProducto> productos = (from prod in db.tbProducto
                                          join prov in db.tbProductoProveedor on prod.codProducto equals prov.codProducto
                                          where prov.codProveedor == id
                                          orderby prod.producto
                                          select prod).ToList();
            ViewBag.codPresentacion = new SelectList(new List<tbPresentacion>(), "codPresentacion", "presentacion");
            ViewBag.codProducto = new SelectList(productos, "codProducto", "producto");
            return View();
        }

        [HttpPost]
        public int CrearEntrada(int codProveedor, List<tbProductoTransaccion> lista, string descripcion)
        {
            try
            {
                dbHeredadesEntities db = new dbHeredadesEntities();
                tbTransaccion nuevaEntrada = new tbTransaccion
                {
                    codProveedor = codProveedor,
                    codTipoTransaccion = 1,
                    descripcion = descripcion,
                    fecha = DateTime.Now,
                    estado = true
                };
                foreach (tbProductoTransaccion item in lista)
                {
                    item.precioCompra = db.tbProductoProveedor.Find(codProveedor, item.codProducto, item.codPresentacion).precioCompra;
                    nuevaEntrada.tbProductoTransaccion.Add(item);
                }
                db.tbTransaccion.Add(nuevaEntrada);
                db.SaveChanges();
                return 1;
            }
            catch (Exception)
            {
                return 2;
            }
        }

        [HttpPost]
        public JsonResult CargarPresentaciones(int codProducto, int codProveedor)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            var lista = (from pres in db.tbPresentacion
                         join prov in db.tbProductoProveedor on pres.codPresentacion equals prov.codPresentacion
                         where prov.codProveedor == codProveedor && prov.codProducto == codProducto
                         orderby pres.presentacion
                         select new
                         {
                             pres.codPresentacion,
                             pres.presentacion
                         }).ToList();
            return Json(lista);
        }

        [HttpPost]
        public ActionResult ListarProductos(int codProveedor, List<tbProductoTransaccion> lista)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            decimal total = 0;
            if (lista != null)
            {
                foreach (tbProductoTransaccion item in lista)
                {
                    item.tbProductoPresentacion = db.tbProductoPresentacion.Find(item.codProducto, item.codPresentacion);
                    item.precioCompra = db.tbProductoProveedor.Find(codProveedor, item.codProducto, item.codPresentacion).precioCompra;
                    total += item.precioCompra.Value * item.cantidad.Value;
                }
            }
            ViewBag.total = total;
            return PartialView("_ProductosEntrada", lista);
        }

    }
}