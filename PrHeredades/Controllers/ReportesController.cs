using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PrHeredades.Models;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using PrHeredades.Tags;

namespace PrHeredades.Controllers
{
    [TagAutenticacion]
    [TagPermiso(permiso = EnumPermisos.Reportes)]
    public class ReportesController : Controller
    {
        private dbHeredadesEntities db = new dbHeredadesEntities();

        // GET: Reportes
        public ActionResult Movimientos(DateTime? dia)
        {
            //ResumenDeVentas**************
            decimal totalVentas = 0;
            if (dia == null)
            {
                dia = DateTime.Today;    
            }

            ViewBag.dia = dia.Value.ToString("yyyy-MM-dd");
            var VentasDia = db.resumenVentasDia(dia).OrderBy(x=>x.codProducto).ToList();
                foreach (var item in VentasDia)
                {
                    totalVentas += item.entrada.Value;
                }
                ViewBag.totalVentas = Decimal.Round(totalVentas,2) ;
                ViewBag.ventas = VentasDia;
            //ventas **************************  
            //ResumenDeCompras****************
            decimal totalCompras = 0;
            var ComprasDia = db.resumenComprasDia(dia).OrderBy(x=>x.codProducto).ToList();
            foreach (var item in ComprasDia)
            {
                totalCompras += item.gasto.Value;
            }
            ViewBag.totalCompras = Decimal.Round(totalCompras, 2);
            ViewBag.compras = ComprasDia;
            //compras **************
            //ResumenDeGastos ***************
            decimal totalIngresos = 0;
            var Ingresos = db.resumenMovimientosCaja(dia).OrderBy(x=>x.codTransaccionCaja).Where(x=>x.tipoTransaccion==0).ToList();
            foreach (var item in Ingresos)
            {
                totalIngresos += item.valor.Value;
            }
            ViewBag.totalIngresos = Decimal.Round(totalIngresos, 2);
            ViewBag.Ingresos = Ingresos;
            //******************************
            //ResumenDeGastos ***************
            decimal totalGastos = 0;
            var Gastos = db.resumenMovimientosCaja(dia).OrderBy(x => x.codTransaccionCaja).Where(x => x.tipoTransaccion == 1).ToList();
            foreach (var item in Gastos)
            {
                totalGastos += item.valor.Value;
            }
            ViewBag.totalGastos = Decimal.Round(totalGastos, 2);
            ViewBag.Gastos = Gastos;
            //******************************
            //ResumenDeGastos ***************
            decimal totalRetiros = 0;
            var Retiros = db.resumenMovimientosCaja(dia).OrderBy(x => x.codTransaccionCaja).Where(x => x.tipoTransaccion == 2).ToList();
            foreach (var item in Retiros)
            {
                totalRetiros += item.valor.Value;
            }
            ViewBag.totalRetiros = Decimal.Round(totalRetiros, 2);
            ViewBag.Retiros = Retiros;
            //******************************
            return View();
        }
        public ActionResult VentasEnCurso() {
            var ventas = (from v in db.resumenVentasDia(DateTime.Today)select v).ToList();
            List <ventaClasificada> ventasclasificadas = new List<ventaClasificada>();
           List<int> productos = (from p in ventas orderby p.codProducto ascending select p.codProducto).Distinct().ToList();
            foreach (var item in productos) {
                decimal stotal=0;
                ventaClasificada v = new ventaClasificada();
                 var pre = (from p in ventas where p.codProducto == item select p).ToList();
                v.nombreProducto = pre.ElementAt(0).producto;
                v.presetacionesProd = pre;
                foreach (var itm in pre) { stotal += itm.entrada.Value; }
                v.total = stotal;
                ventasclasificadas.Add(v);   
            }

            return View(ventasclasificadas);
        }
  
           
        public ActionResult imprimirVentasEnCurso()
        {
            var ventas = (from v in db.resumenVentasDia(DateTime.Today)
                          select new productoGenerico
                          {
                              producto = v.producto.ToUpper(),
                              presentacion = v.presentacion,
                              cantidad = v.cantidad.Value,
                              GastoEntradaValor = v.entrada.Value
                          }).ToList();
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "ventasEnCurso.rpt"));
            rd.SetDataSource(ventas);          
            Response.Buffer = false;
            Response.Clear();
            Response.ClearHeaders();
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "aplication/pdf", "VentasDelDia.pdf");
        }
       
        public ActionResult imprimirMovimientos(DateTime fecha) {

            var Ventas = from v in db.resumenVentasDia(fecha)
                         .OrderBy(x => x.codProducto).ToList()
                          select new productoGenerico {
                              Mov = "VENTAS",
                             producto = v.producto,
                             presentacion = v.presentacion,
                             cantidad = v.cantidad.Value,
                             GastoEntradaValor = v.entrada.Value
                         }; 

            var Compras = from c in db.resumenComprasDia(fecha)
                          .OrderBy(x => x.codProducto).ToList()
                          select new productoGenerico {
                              Mov = "COMPRAS",
                              producto = c.producto,
                              presentacion = c.presentacion,
                              cantidad = c.cantidad.Value,
                              GastoEntradaValor = c.gasto.Value
                          };
            //var Gastos = (from g in db.resumenGastosDia(fecha)
            //              .OrderBy(x => x.codTransaccionCaja).ToList()
            //             select new productoGenerico {
            //                  Mov = "GASTOS",
            //                 descripcion = g.descripcion,
            //                 GastoEntradaValor = g.valor.Value
            //             });

            List<productoGenerico> movimientos = new List<productoGenerico>();

            movimientos.AddRange(Ventas);
            movimientos.AddRange(Compras);
           // movimientos.AddRange(Gastos);

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reports"), "ResumenMov.rpt"));
            rd.SetDataSource(movimientos);
            Response.Buffer = false;
            Response.Clear();
            Response.ClearHeaders();
            rd.SetParameterValue("fecha", fecha);
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "aplication/pdf", "Movimientos.pdf");

        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
