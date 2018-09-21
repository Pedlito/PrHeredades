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
    public class TransaccionController : Controller
    {
        // las transacciones son 1 para entradas, 2 para salidas
        private readonly int registrosPagina = 10;


        //  Entradas
        public ActionResult Entradas(int pagina = 1, string filtro = "", bool estado = true)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbTransaccion> lista;
            if (filtro != "")
            {
                DateTime fecha = DateTime.Parse(filtro);
                lista = (from t in db.tbTransaccion
                         where t.codTipoTransaccion == 1 && DbFunctions.TruncateTime(t.fecha) == fecha && t.estado == estado
                         orderby t.fecha descending
                         select t).ToList();
            }
            else
            {
                lista = (from t in db.tbTransaccion
                         where t.codTipoTransaccion == 1 && t.estado == estado
                         orderby t.fecha descending
                         select t).ToList();
            }
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
            List<int> codigos = db.tbProductoProveedor.Where(t => t.codProveedor == id && t.estado.Value).GroupBy(t => t.codProducto).Select(t => t.Key).ToList();
            List<tbProducto> productos = db.tbProducto.Where(t => codigos.Contains(t.codProducto)).OrderBy(t => t.producto).ToList();
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
                    codUsuario = Sesion.ObtenerCodigo(),
                    descripcion = descripcion,
                    fecha = DateTime.Now,
                    estado = true
                };
                decimal total = 0;
                foreach (tbProductoTransaccion item in lista)
                {
                    item.precioCompra = db.tbProductoProveedor.Find(codProveedor, item.codProducto, item.codPresentacion).precioCompra;
                    nuevaEntrada.tbProductoTransaccion.Add(item);
                    total += item.precioCompra.Value;
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

        public ActionResult DetalleEntrada(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbTransaccion entrada = db.tbTransaccion.Find(id);
            decimal total = 0;
            foreach (tbProductoTransaccion item in entrada.tbProductoTransaccion)
            {
                total += item.precioCompra.Value * item.cantidad.Value;
            }
            ViewBag.total = total;
            return View(entrada);
        }

        //Salidas

        public ActionResult Salidas(int pagina = 1, string filtro = "", bool estado = true)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbTransaccion> lista;
            if (filtro != "")
            {
                DateTime fecha = DateTime.Parse(filtro);
                lista = (from t in db.tbTransaccion
                         where t.codTipoTransaccion == 2 && DbFunctions.TruncateTime(t.fecha) == fecha && t.estado == estado
                         orderby t.fecha descending
                         select t).ToList();
            }
            else
            {
                lista = (from t in db.tbTransaccion
                         where t.codTipoTransaccion == 2 && t.estado == estado
                         orderby t.fecha descending
                         select t).ToList();
            }
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Index", "Categoria");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            ViewBag.estado = estado;
            ViewBag.proveedores = db.tbProveedor.OrderBy(t => t.proveedor).ToList();
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        public ActionResult CrearSalida()
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<int> codigos = (from t in db.vExistencias
                                 where t.existencia > 0
                                 group t by t.codProducto into cod
                                 select cod.Key).ToList();
            List<tbProducto> productos = db.tbProducto.Where(t => codigos.Contains(t.codProducto)).OrderBy(t => t.producto).ToList();
            ViewBag.codPresentacion = new SelectList(new List<tbPresentacion>(), "codPresentacion", "presentacion");
            ViewBag.codProducto = new SelectList(productos, "codProducto", "producto");
            return View();
        }

        [HttpPost]
        public int CrearSalida(List<tbProductoTransaccion> lista, string descripcion)
        {
            try
            {
                dbHeredadesEntities db = new dbHeredadesEntities();
                tbTransaccion nuevaEntrada = new tbTransaccion
                {
                    codUsuario = Sesion.ObtenerCodigo(),
                    codTipoTransaccion = 2,
                    descripcion = descripcion,
                    fecha = DateTime.Now,
                    estado = true
                };
                foreach (tbProductoTransaccion item in lista)
                {
                    item.cantidad *= -1;
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

        public ActionResult DetalleSalida(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbTransaccion salida = db.tbTransaccion.Find(id);
            return View(salida);
        }

        //Funciones

        public ActionResult CambiarEstado(int id, int tipoTransaccion)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbTransaccion transaccion = db.tbTransaccion.Find(id);
            string texto = "";
            if (tipoTransaccion == 1)
                texto = "Entrada";
            else
                texto = "Salida";
            ViewBag.transaccion = texto;
            return View(transaccion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarEstado(int id, FormCollection collection)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbTransaccion transaccion = db.tbTransaccion.Find(id);
            transaccion.estado = !(transaccion.estado.Value);
            db.SaveChanges();
            if (transaccion.codTipoTransaccion == 1)
            {
                return RedirectToAction("Entradas");
            }
            else
            {
                return RedirectToAction("Salidas");
            }
        }

        [HttpPost]
        public JsonResult CargarPresentacionesEntrada(int codProducto, int codProveedor)
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
        public JsonResult CargarPresentacionesSalida(int codProducto)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            var lista = (from t in db.vExistencias
                         where t.codProducto == codProducto && t.existencia > 0
                         orderby t.producto
                         select new
                         {
                             t.codPresentacion,
                             t.presentacion
                         }).ToList();
            return Json(lista);
        }

        [HttpPost]
        public ActionResult ListarProductosEntrada(int codProveedor, List<tbProductoTransaccion> lista)
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

        [HttpPost]
        public ActionResult ListarProductosSalida(List<tbProductoTransaccion> lista)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<ProductoSalida> listaSalida = new List<ProductoSalida>();
            if (lista != null)
            {
                foreach (tbProductoTransaccion item in lista)
                {
                    vExistencias existencias = db.vExistencias.Find(item.codProducto, item.codPresentacion);
                    listaSalida.Add(new ProductoSalida
                    {
                        codProducto = item.codProducto,
                        producto = existencias.producto,
                        codPresentacion = item.codPresentacion,
                        presentacion = existencias.presentacion,
                        cantidad = item.cantidad.Value,
                        existencia = existencias.existencia.Value
                    });
                }
            }
            return PartialView("_ProductosSalida", listaSalida);
        }
    }
}