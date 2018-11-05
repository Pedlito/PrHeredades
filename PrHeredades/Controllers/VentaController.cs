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
        // true = al contado, false = al credito
        // GET: Venta

        public ActionResult Index(int pagina = 1, string filtro = "", bool tipoVenta = true, bool estado = true)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbVenta> lista;
            if (filtro != "")
            {
                DateTime fecha = DateTime.Parse(filtro);
                if (tipoVenta)
                {
                    lista = (from t in db.tbVenta
                             where DbFunctions.TruncateTime(t.fecha) == fecha && t.codDeudor == null && t.estado == estado
                             orderby t.fecha descending
                             select t).ToList();
                }
                else
                {
                    lista = (from t in db.tbVenta
                             where DbFunctions.TruncateTime(t.fecha) == fecha && t.codDeudor != null && t.estado == estado
                             orderby t.fecha descending
                             select t).ToList();
                }

            }
            else
            {
                if (tipoVenta)
                {
                    lista = (from t in db.tbVenta
                             where t.codDeudor == null && t.estado == estado
                             orderby t.fecha descending
                             select t).ToList();
                }
                else
                {
                    lista = (from t in db.tbVenta
                             where t.codDeudor != null && t.estado == estado
                             orderby t.fecha descending
                             select t).ToList();
                }
            }
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Index", "Categoria");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            ViewBag.estado = estado;
            ViewBag.tipoVenta = tipoVenta;
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        public ActionResult Vender()
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbProducto> productos = db.tbProducto.OrderBy(t => t.producto).ToList();
            ViewBag.codProducto = new SelectList(productos, "codProducto", "producto");
            ViewBag.codPresentacion = new SelectList(new List<tbPresentacion>(), "codPresentacion", "presentacion");
            return View();
        }

        [HttpPost]
        public int Vender(List<tbVentaProducto> lista, int? codDeudor)
        {
            try
            {
                dbHeredadesEntities db = new dbHeredadesEntities();
                //con la lista, comprobar que hay existencia de cada producto y si no transaformar en cascada, si no existe ningun producto, enviar notificación
                tbDeudor deudor = null;
                tbVenta venta = new tbVenta
                {
                    fecha = DateTime.Now,
                    codUsuario = Sesion.ObtenerCodigo(),
                    estado = true
                };
                if (codDeudor != null)
                {
                    venta.codDeudor = codDeudor;
                    deudor = db.tbDeudor.Find(codDeudor);
                }
                //variable para calcular la deuda total al deudor o que entra a caja
                decimal total = 0;
                foreach (tbVentaProducto item in lista)
                {
                    //si el producto tiene existencia con esa presentacion
                    if (TieneExistencia(item.codProducto, item.codPresentacion, item.cantidad.Value))
                    {
                        tbProductoPresentacion prodPres = db.tbProductoPresentacion.Find(item.codProducto, item.codPresentacion);
                        //se agregar el precio de venta actual y se agrega a la venta
                        item.precioVenta = prodPres.precioVenta;
                        venta.tbVentaProducto.Add(item);
                        //se reduce la venta a la existencia
                        prodPres.existencia -= item.cantidad;
                        //se agrega al total
                        total += item.precioVenta.Value * item.cantidad.Value;
                    }
                    else
                    {
                        //si no, se intenta realizar una conversión del producto con la presentacion
                        tbProductoPresentacion prod = db.tbProductoPresentacion.Find(item.codProducto, item.codPresentacion);
                        if (Convertir(prod))
                        {
                            //si se logra realizar la conversion
                            //se agregar el precio de venta actual y se agrega a la venta
                            item.precioVenta = prod.precioVenta;
                            venta.tbVentaProducto.Add(item);
                            //se reduce la venta a la existencia
                            prod.existencia -= item.cantidad;
                            //se agrega al total
                            total += item.precioVenta.Value * item.cantidad.Value;
                        }
                        else
                        {
                            //se informa de la falta de producto
                        }
                    }
                }
                if (deudor != null)
                {
                    //si hay un deudor, se le agrega a la deuda
                    deudor.deuda += total;
                }
                else
                {
                    //si no hay deudor, se agrega a la caja
                    tbTransaccionCaja transaccionCaja = new tbTransaccionCaja
                    {
                        tipoTransaccion = 1,
                        cantidad = total,
                        fecha = DateTime.Now
                    };
                    CajaController.Sumar(total);
                    db.tbTransaccionCaja.Add(transaccionCaja);
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

        public ActionResult DetalleVenta(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbVenta venta = db.tbVenta.Find(id);
            decimal total = 0;
            foreach (tbVentaProducto item in venta.tbVentaProducto)
            {
                total += item.precioVenta.Value * item.cantidad.Value;
            }
            ViewBag.total = Math.Truncate(total * 100) / 100;
            return View(venta);
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

        //Se encarga de toda la logica previa a realizar una conversión
        public bool Convertir(tbProductoPresentacion prod)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbProductoPresentacion correlativoMayor = db.tbProductoPresentacion.Where(t => t.codProducto == prod.codProducto && t.correlativo == (prod.correlativo + 1)).SingleOrDefault();
            if (correlativoMayor != null)
            {
                //si existe un correlativo mayor se verifica si tiene existencia 
                if (correlativoMayor.existencia > 0)
                {
                    //si tiene al menos una unidad de existencia, se realiza la conversión
                    tbProductoPresentacion mayor = db.tbProductoPresentacion.Where(t => t.codProducto == prod.codProducto && t.correlativo == (prod.correlativo + 1)).SingleOrDefault();
                    mayor.existencia -= 1;
                    prod.existencia += mayor.unidades;
                    db.SaveChanges();

                    return true;
                }
                else
                {
                    //si no tiene nada en existencia se intenta con un correlativo mayor
                    if (Convertir(db.tbProductoPresentacion.Where(t => t.codProducto == prod.codProducto && t.correlativo == (prod.correlativo + 1)).SingleOrDefault()))
                    {
                        //se realiza conversion
                        tbProductoPresentacion mayor = db.tbProductoPresentacion.Where(t => t.codProducto == prod.codProducto && t.correlativo == (prod.correlativo + 1)).SingleOrDefault();
                        mayor.existencia -= 1;
                        prod.existencia += mayor.unidades;
                        db.SaveChanges();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        //verifica si el producto tiene mas existencia que la compra
        public bool TieneExistencia(int codProd, int codPres, decimal compra)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            decimal existencia = db.tbProductoPresentacion.Find(codProd, codPres).existencia.Value;
            return (existencia >= compra);
        }
    }
}