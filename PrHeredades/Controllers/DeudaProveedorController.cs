﻿using System;
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
    public class DeudaProveedorController : Controller
    {
        private readonly int registrosPagina = 10;
        // GET: DeudaProveedor 
        public ActionResult Index(int pagina = 1, string filtro = "", bool estado = true)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<vDeudaProveedor> lista = db.vDeudaProveedor.Where(t => t.proveedor.Contains(filtro)).ToList();
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Index", "Presentacion");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            ViewBag.estado = estado;
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        public ActionResult Pagar(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbProveedor proveedor = db.tbProveedor.Find(id);
            ViewBag.codProveedor = id;
            ViewBag.proveedor = proveedor.proveedor;
            return View();
        }

        [HttpPost]
        public ActionResult Pagar(FormCollection collection)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbPagoProveedor pago = new tbPagoProveedor
            {
                codProveedor = int.Parse(collection["codProveedor"]),
                codUsuario = Sesion.ObtenerCodigo(),
                fecha = DateTime.Now,
                pago = decimal.Parse(collection["pago"]),
                descripcion = collection["descripcion"],
                estado = true
            };
            db.tbPagoProveedor.Add(pago);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult VerPagos(int id, int pagina = 1, string filtro = "", bool estado = true)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbPagoProveedor> lista = new List<tbPagoProveedor>();
            if (filtro != "")
            {
                DateTime fecha = DateTime.Parse(filtro);
                lista = db.tbPagoProveedor.Where(t => t.codProveedor == id && t.estado == estado && DbFunctions.TruncateTime(t.fecha) == fecha).OrderByDescending(t => t.fecha).ToList();
            }
            else
            {
                lista = db.tbPagoProveedor.Where(t => t.codProveedor == id && t.estado == estado).OrderByDescending(t => t.fecha).ToList();
            }
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Index", "Presentacion");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            ViewBag.estado = estado;
            tbProveedor proveedor = db.tbProveedor.Find(id);
            ViewBag.proveedor = proveedor.proveedor;
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        public ActionResult DetallePago(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbPagoProveedor pago = db.tbPagoProveedor.Find(id);
            return View(pago);
        }

        public ActionResult CambiarEstado(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbPagoProveedor pago = db.tbPagoProveedor.Find(id);
            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarEstado(int id, FormCollection collection)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbPagoProveedor pago = db.tbPagoProveedor.Find(id);
            pago.estado = !(pago.estado.Value);
            db.SaveChanges();
            return RedirectToAction("VerPagos", new { id = pago.codProveedor });
        }
    }
}