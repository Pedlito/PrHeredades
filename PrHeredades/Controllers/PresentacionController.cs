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
    [TagPermiso(permiso = EnumPermisos.Presentacion)]
    public class PresentacionController : Controller
    {
        private readonly int registrosPagina = 10;
        // GET: Presentacion
        public ActionResult Index(int pagina = 1, string filtro = "", bool estado = true)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbPresentacion> lista = (from t in db.tbPresentacion
                                          where t.presentacion.Contains(filtro) && t.estado == estado
                                          orderby t.presentacion
                                          select t).ToList();
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Index", "Presentacion");
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
        public ActionResult Crear(tbPresentacion nueva)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            if (!(db.tbPresentacion.Any(t => t.presentacion == nueva.presentacion)))
            {
                nueva.estado = true;
                db.tbPresentacion.Add(nueva);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "¡Ya existe esta presentación!");
                return View(nueva);
            }
        }

        public ActionResult Editar(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            return View(db.tbPresentacion.Find(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(tbPresentacion editada)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            if (!(db.tbPresentacion.Any(t => t.presentacion == editada.presentacion && t.codPresentacion != editada.codPresentacion)))
            {
                db.Entry(editada).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "¡Ya existe esta presentación!");
                return View(editada);
            }
        }

        public ActionResult CambiarEstado(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbPresentacion presentacion = db.tbPresentacion.Find(id);
            return View(presentacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarEstado(int id, FormCollection collection)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbPresentacion presentacion = db.tbPresentacion.Find(id);
            presentacion.estado = !(presentacion.estado);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}