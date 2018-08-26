using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PrHeredades.Models;

namespace PrHeredades.Controllers
{
    public class PruebaEntityController : Controller
    {
        private dbHeredadesEntities db = new dbHeredadesEntities();

        // GET: PruebaEntity
        public ActionResult Index()
        {
            var tbProducto = db.tbProducto.Include(t => t.tbCategoria);
            return View(tbProducto.ToList());
        }

        // GET: PruebaEntity/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbProducto tbProducto = db.tbProducto.Find(id);
            if (tbProducto == null)
            {
                return HttpNotFound();
            }
            return View(tbProducto);
        }

        // GET: PruebaEntity/Create
        public ActionResult Create()
        {
            ViewBag.codCategoria = new SelectList(db.tbCategoria, "codCategoria", "categoria");
            return View();
        }

        // POST: PruebaEntity/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "codProducto,codCategoria,producto,estado")] tbProducto tbProducto)
        {
            if (ModelState.IsValid)
            {
                db.tbProducto.Add(tbProducto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.codCategoria = new SelectList(db.tbCategoria, "codCategoria", "categoria", tbProducto.codCategoria);
            return View(tbProducto);
        }

        // GET: PruebaEntity/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbProducto tbProducto = db.tbProducto.Find(id);
            if (tbProducto == null)
            {
                return HttpNotFound();
            }
            ViewBag.codCategoria = new SelectList(db.tbCategoria, "codCategoria", "categoria", tbProducto.codCategoria);
            return View(tbProducto);
        }

        // POST: PruebaEntity/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "codProducto,codCategoria,producto,estado")] tbProducto tbProducto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbProducto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.codCategoria = new SelectList(db.tbCategoria, "codCategoria", "categoria", tbProducto.codCategoria);
            return View(tbProducto);
        }

        // GET: PruebaEntity/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbProducto tbProducto = db.tbProducto.Find(id);
            if (tbProducto == null)
            {
                return HttpNotFound();
            }
            return View(tbProducto);
        }

        // POST: PruebaEntity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbProducto tbProducto = db.tbProducto.Find(id);
            db.tbProducto.Remove(tbProducto);
            db.SaveChanges();
            return RedirectToAction("Index");
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
