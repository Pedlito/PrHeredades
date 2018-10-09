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
    public class VentaController : Controller
    {
        private readonly int registrosPagina = 10;
        // GET: Venta
        public ActionResult Index(int pagina = 1, string filtro = "", bool estado = true)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbVenta> lista;
            if (filtro != "")
            {
                DateTime fecha = DateTime.Parse(filtro);
                lista = (from t in db.tbVenta
                         where DbFunctions.TruncateTime(t.fecha) == fecha && t.estado == estado
                         orderby t.fecha descending
                         select t).ToList();
            }
            else
            {
                lista = (from t in db.tbVenta
                         where t.estado == estado
                         orderby t.fecha descending
                         select t).ToList();
            }
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Index", "Categoria");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            ViewBag.estado = estado;
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        public ActionResult Vender()
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbProducto> productos = db.tbProducto.OrderBy(t => t.producto).ToList();
            ViewBag.codPresentacion = new SelectList(new List<tbPresentacion>(), "codPresentacion", "presentacion");
            ViewBag.codProducto = new SelectList(productos, "codProducto", "producto");
            return View();
        }

        [HttpPost]
        public int VenderContado(List<tbVentaProducto> lista)
        {
            try
            {
                dbHeredadesEntities db = new dbHeredadesEntities();
                tbVenta venta = new tbVenta
                {
                    fecha = DateTime.Now,
                    estado = true
                };
                foreach (tbVentaProducto item in lista)
                {
                    item.precioVenta = db.tbProductoPresentacion.Find(item.codProducto, item.codPresentacion).precioVenta;
                    venta.tbVentaProducto.Add(item);
                }
                db.tbVenta.Add(venta);
                db.SaveChanges();
                return 1;
            }
            catch (Exception)
            { 
                return 2;
            }

        }

        [HttpPost]
        public ActionResult ListarClientes(string filtro)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbDeudor> lista = (from t in db.tbDeudor
                                    where t.nombre.Contains(filtro)
                                    orderby t.nombre
                                    select t).Take(10).ToList();
            return PartialView("_Clientes", lista);
        }

        [HttpPost]
        public ActionResult ListarProductosVenta(List<tbVentaProducto> lista)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<ProductoVenta> listaVenta = new List<ProductoVenta>();
            decimal total = 0;
            if (lista != null)
            {
                foreach (tbVentaProducto item in lista)
                {
                    tbProductoPresentacion producto = db.tbProductoPresentacion.Find(item.codProducto, item.codPresentacion);
                    listaVenta.Add(new ProductoVenta
                    {
                        codProducto = producto.codProducto,
                        producto = producto.tbProducto.producto,
                        codPresentacion = producto.codPresentacion,
                        presentacion = producto.tbPresentacion.presentacion,
                        cantidad = item.cantidad.Value,
                        precioVenta = producto.precioVenta.Value
                    });
                    total += listaVenta.Last().precioVenta * listaVenta.Last().cantidad;
                }
            }
            ViewBag.total = Math.Truncate(total * 100) / 100;
            return PartialView("_ProductosVenta", listaVenta);
        }

        [HttpPost]
        public JsonResult CargarPresentaciones(int codProducto)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            var lista = (from t in db.tbProductoPresentacion
                         where t.codProducto == codProducto && t.tbProducto.estado == true
                         orderby t.correlativo
                         select new
                         {
                             t.codPresentacion,
                             t.tbPresentacion.presentacion
                         }).ToList();
            return Json(lista);
        }
    }
}