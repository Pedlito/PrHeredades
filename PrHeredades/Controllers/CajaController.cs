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
    public class CajaController : Controller
    {
        // GET: Caja
        // codigo de tipo transaccion 0 = entradas, 1 = gastos, 2 = retiros
        private readonly int registrosPagina = 10;

        public ActionResult Index()
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbCaja caja = db.tbCaja.Find(1);
            ViewBag.cantidad = caja.cantidad;
            return View();
        }

        public ActionResult Ingresos(int pagina = 1, string filtro = "")
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbTransaccionCaja> lista;
            if (filtro != "")
            {
                DateTime fecha = DateTime.Parse(filtro);
                lista = (from t in db.tbTransaccionCaja
                         where t.tipoTransaccion == 0 && DbFunctions.TruncateTime(t.fecha) == fecha
                         orderby t.fecha descending
                         select t).ToList();
            }
            else
            {
                lista = (from t in db.tbTransaccionCaja
                         where t.tipoTransaccion == 0
                         orderby t.fecha descending
                         select t).ToList();
            }
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Ingresos", "Caja");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        public ActionResult Gastos(int pagina = 1, string filtro = "")
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbTransaccionCaja> lista;
            if (filtro != "")
            {
                DateTime fecha = DateTime.Parse(filtro);
                lista = (from t in db.tbTransaccionCaja
                         where t.tipoTransaccion == 1 && DbFunctions.TruncateTime(t.fecha) == fecha
                         orderby t.fecha descending
                         select t).ToList();
            }
            else
            {
                lista = (from t in db.tbTransaccionCaja
                         where t.tipoTransaccion == 1
                         orderby t.fecha descending
                         select t).ToList();
            }
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Gastos", "Caja");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        public ActionResult Retiros(int pagina = 1, string filtro = "")
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbTransaccionCaja> lista;
            if (filtro != "")
            {
                DateTime fecha = DateTime.Parse(filtro);
                lista = (from t in db.tbTransaccionCaja
                         where t.tipoTransaccion == 2 && DbFunctions.TruncateTime(t.fecha) == fecha
                         orderby t.fecha descending
                         select t).ToList();
            }
            else
            {
                lista = (from t in db.tbTransaccionCaja
                         where t.tipoTransaccion == 2
                         orderby t.fecha descending
                         select t).ToList();
            }
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Gastos", "Caja");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        public ActionResult CrearTransaccion(int tipoTrasaccion)
        {
            switch (tipoTrasaccion)
            {
                case 0:
                    ViewBag.titulo = "Agregar ingreso";
                    ViewBag.cancelar = "Ingresos";
                    break;
                case 1:
                    ViewBag.titulo = "Agregar gasto";
                    ViewBag.cancelar = "Gastos";
                    break;
                case 2:
                    ViewBag.titulo = "Agregar retiro";
                    ViewBag.cancelar = "Retiros";
                    break;
            }
            ViewBag.tipoTransaccion = tipoTrasaccion;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearTransaccion(tbTransaccionCaja transaccion)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            int usuario = Sesion.ObtenerCodigo();
            transaccion.codUsuario = Sesion.ObtenerCodigo();
            transaccion.fecha = DateTime.Now;
            db.tbTransaccionCaja.Add(transaccion);
            if (transaccion.tipoTransaccion == 0)
            {
                CajaController.Sumar(transaccion.cantidad.Value);
            }
            else
            {
                CajaController.Restar(transaccion.cantidad.Value);
            }
            db.SaveChanges();
            switch (transaccion.tipoTransaccion)
            {
                case 0:
                    return RedirectToAction("Ingresos");
                case 1:
                    return RedirectToAction("Gastos");
                case 2:
                    return RedirectToAction("Retiros");
                default:
                    return RedirectToAction("Index");
            }
        }

        public ActionResult DetalleTransaccion(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbTransaccionCaja transaccion = db.tbTransaccionCaja.Find(id);
            switch (transaccion.tipoTransaccion)
            {
                case 0:
                    ViewBag.titulo = "Detalle de ingreso";
                    ViewBag.regresar = "Ingresos";
                    break;
                case 1:
                    ViewBag.titulo = "Detalle de gasto";
                    ViewBag.regresar = "Gastos";
                    break;
                case 2:
                    ViewBag.titulo = "Detalle de retiro";
                    ViewBag.regresar = "Retiros";
                    break;
            }
            return View(transaccion);
        }

        public static void Sumar(decimal cantidad)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbCaja caja = db.tbCaja.Find(1);
            caja.cantidad += cantidad;
            db.SaveChanges();
        }

        public static void Restar(decimal cantidad)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbCaja caja = db.tbCaja.Find(1);
            caja.cantidad -= cantidad;
            db.SaveChanges();
        }
    }
}