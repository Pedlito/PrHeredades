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
        // las transacciones son 1 para entradas, 2 para salidas, 0 para pedidos
        private readonly int registrosPagina = 10;

        // pedidos
        [TagPermiso(permiso = EnumPermisos.Pedidos)]
        #region Pedidos
        public ActionResult Pedidos(int pagina = 1, string filtro = "", bool estado = true)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbTransaccion> lista;
            if (filtro != "")
            {
                DateTime fecha = DateTime.Parse(filtro);
                lista = (from t in db.tbTransaccion
                         where t.codTipoTransaccion == 0 && DbFunctions.TruncateTime(t.fecha) == fecha && t.estado == estado
                         orderby t.fecha descending
                         select t).ToList();
            }
            else
            {
                lista = (from t in db.tbTransaccion
                         where t.codTipoTransaccion == 0 && t.estado == estado
                         orderby t.fecha descending
                         select t).ToList();
            }
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Index", "Transaccion");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            ViewBag.estado = estado;
            ViewBag.proveedores = db.tbProveedor.OrderBy(t => t.proveedor).ToList();
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        [TagPermiso(permiso = EnumPermisos.Pedidos)]
        public ActionResult CrearPedido(int id)
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
        public int CrearPedido(int codProveedor, List<tbProductoTransaccion> lista, string descripcion)
        {
            try
            {
                dbHeredadesEntities db = new dbHeredadesEntities();
                // creo el objeto transacción
                tbTransaccion nuevaEntrada = new tbTransaccion
                {
                    codProveedor = codProveedor,
                    codTipoTransaccion = 0,
                    codUsuario = Sesion.ObtenerCodigo(),
                    descripcion = descripcion,
                    fecha = DateTime.Now,
                    estado = true
                };
                // se agregan todos los productos al pedido pero sin precio
                foreach (tbProductoTransaccion item in lista)
                {
                    nuevaEntrada.tbProductoTransaccion.Add(item);
                }
                //agrego la transaccion y guardo cambios
                db.tbTransaccion.Add(nuevaEntrada);
                db.SaveChanges();
                return 1;
            }
            catch (Exception)
            {
                return 2;
            }
        }

        [TagPermiso(permiso = EnumPermisos.Pedidos)]
        public ActionResult RecivirPedido(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            // obtengo el pedido y lo convierto en una entrada
            tbTransaccion transaccion = db.tbTransaccion.Find(id);
            transaccion.codTipoTransaccion = 1;
            // cambio el usuario por el que recive el pedido
            transaccion.codUsuario = Sesion.ObtenerCodigo();
            transaccion.fecha = DateTime.Now;
            foreach (tbProductoTransaccion item in transaccion.tbProductoTransaccion)
            {
                // a cada producto comprado le ingreso el precio de compra
                item.precioCompra = db.tbProductoProveedor.Find(transaccion.codProveedor, item.codProducto, item.codPresentacion).precioCompra;
                // agrego la entrada a la existencia
                tbProductoPresentacion producto = db.tbProductoPresentacion.Find(item.codProducto, item.codPresentacion);
                producto.existencia += item.cantidad;
                // agrego la deuda al proveedor
                tbProveedor proveedor = db.tbProveedor.Find(transaccion.codProveedor);
                proveedor.deuda += item.precioCompra * item.cantidad;
            }
            db.SaveChanges();
            return RedirectToAction("Pedidos");
        }

        [TagPermiso(permiso = EnumPermisos.Pedidos)]
        public ActionResult DetallePedido(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbTransaccion pedido = db.tbTransaccion.Find(id);
            return View(pedido);
        }
        #endregion

        // Entradas
        #region Entrada
        [TagPermiso(permiso = EnumPermisos.Compras)]
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
            Paginacion paginacion = new Paginacion(pagina, paginas, "Index", "Transaccion");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            ViewBag.estado = estado;
            ViewBag.proveedores = db.tbProveedor.OrderBy(t => t.proveedor).ToList();
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        [TagPermiso(permiso = EnumPermisos.Compras)]
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
                // creo el objeto transacción
                tbTransaccion nuevaEntrada = new tbTransaccion
                {
                    codProveedor = codProveedor,
                    codTipoTransaccion = 1,
                    codUsuario = Sesion.ObtenerCodigo(),
                    descripcion = descripcion,
                    fecha = DateTime.Now,
                    estado = true
                };
                decimal deuda = 0;
                foreach (tbProductoTransaccion item in lista)
                {
                    // se asigna el precio de compra al item y se agrega a la tabla JOIN de productoTransacción
                    item.precioCompra = db.tbProductoProveedor.Find(codProveedor, item.codProducto, item.codPresentacion).precioCompra;
                    nuevaEntrada.tbProductoTransaccion.Add(item);
                    // se agrega a la existencia del producto
                    tbProductoPresentacion prod = db.tbProductoPresentacion.Find(item.codProducto, item.codPresentacion);
                    prod.existencia += item.cantidad;
                    // sumo a la deuda precioCompra * cantidadEntrada.
                    deuda += item.cantidad.Value * item.precioCompra.Value;
                }
                // calculo deuda total de entrada y la sumo a deuda total al proveedor.
                tbProveedor proveedor = db.tbProveedor.Find(codProveedor);
                proveedor.deuda += deuda;
                //agrego la transaccion y guardo cambios
                db.tbTransaccion.Add(nuevaEntrada);
                db.SaveChanges();
                return 1;
            }
            catch (Exception)
            {
                return 2;
            }
        }

        [TagPermiso(permiso = EnumPermisos.Compras)]
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
        #endregion

        //Salidas
        #region Salida
        [TagPermiso(permiso = EnumPermisos.Salida)]
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
            Paginacion paginacion = new Paginacion(pagina, paginas, "Index", "Transaccion");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            ViewBag.estado = estado;
            ViewBag.proveedores = db.tbProveedor.OrderBy(t => t.proveedor).ToList();
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        [TagPermiso(permiso = EnumPermisos.Salida)]
        public ActionResult CrearSalida()
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<int> codigos = (from t in db.tbProductoPresentacion
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
                // creo la nueva transacción
                tbTransaccion nuevaSalida = new tbTransaccion
                {
                    codUsuario = Sesion.ObtenerCodigo(),
                    codTipoTransaccion = 2,
                    descripcion = descripcion,
                    fecha = DateTime.Now,
                    estado = true
                };
                foreach (tbProductoTransaccion item in lista)
                {
                    nuevaSalida.tbProductoTransaccion.Add(item);
                    //quito la existencia del producto 
                    tbProductoPresentacion prod = db.tbProductoPresentacion.Find(item.codProducto, item.codPresentacion);
                    prod.existencia -= item.cantidad;
                }
                db.tbTransaccion.Add(nuevaSalida);
                db.SaveChanges();
                return 1;
            }
            catch (Exception)
            {
                return 2;
            }
        }

        [TagPermiso(permiso = EnumPermisos.Salida)]
        public ActionResult DetalleSalida(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbTransaccion salida = db.tbTransaccion.Find(id);
            return View(salida);
        }
        #endregion

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
            if (transaccion.codTipoTransaccion == 0)
            {
                db.SaveChanges();
                return RedirectToAction("Pedidos");
            }
            else
            {
                foreach (tbProductoTransaccion item in transaccion.tbProductoTransaccion)
                {
                    tbProductoPresentacion prod = db.tbProductoPresentacion.Find(item.codProducto, item.codPresentacion);
                    if (transaccion.estado.Value)
                    {
                        prod.existencia -= item.cantidad;
                    }
                    else
                    {
                        prod.existencia += item.cantidad;
                    }
                }
                db.SaveChanges();
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
            var lista = (from t in db.tbProductoPresentacion
                         where t.codProducto == codProducto && t.existencia > 0
                         orderby t.correlativo descending
                         select new
                         {
                             t.codPresentacion,
                             t.tbPresentacion.presentacion
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
                    tbProductoPresentacion existencias = db.tbProductoPresentacion.Find(item.codProducto, item.codPresentacion);
                    listaSalida.Add(new ProductoSalida
                    {
                        codProducto = item.codProducto,
                        producto = existencias.tbProducto.producto,
                        codPresentacion = item.codPresentacion,
                        presentacion = existencias.tbPresentacion.presentacion,
                        cantidad = item.cantidad.Value,
                        existencia = existencias.existencia.Value
                    });
                }
            }
            return PartialView("_ProductosSalida", listaSalida);
        }
    }
}