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
    public class CategoriaController : Controller
    {
        private readonly int registrosPagina = 10;
        // GET: Categoria
        public ActionResult Index(int pagina = 1, string filtro = "", bool estado = true)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbCategoria> lista = (from t in db.tbCategoria
                                       where t.categoria.Contains(filtro) && t.estado == estado
                                       orderby t.categoria select t).ToList();
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Index", "Categoria");
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
        public ActionResult Crear(tbCategoria nueva)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            if (!(db.tbCategoria.Any(t => t.categoria == nueva.categoria)))
            {
                nueva.estado = true;
                db.tbCategoria.Add(nueva);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "¡Ya existe esta categoría!");
                return View(nueva);
            }
        }

        public ActionResult Editar(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            return View(db.tbCategoria.Find(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(tbCategoria editada)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            if (!(db.tbCategoria.Any(t => t.categoria == editada.categoria && t.codCategoria != editada.codCategoria)))
            {
                db.Entry(editada).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "¡Ya existe esta categoría!");
                return View(editada);
            }
        }

        public ActionResult CambiarEstado(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbCategoria categoria = db.tbCategoria.Find(id);
            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarEstado(int id, FormCollection collection)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbCategoria categoria = db.tbCategoria.Find(id);
            categoria.estado = !(categoria.estado.Value);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}