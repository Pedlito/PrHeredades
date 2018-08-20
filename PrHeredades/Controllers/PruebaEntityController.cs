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
            return View(db.tbPresentacion.ToList());
        }

        // GET: PruebaEntity/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbPresentacion tbPresentacion = db.tbPresentacion.Find(id);
            if (tbPresentacion == null)
            {
                return HttpNotFound();
            }
            return View(tbPresentacion);
        }

        // GET: PruebaEntity/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PruebaEntity/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "codPresentacion,presentacion,estado")] tbPresentacion tbPresentacion)
        {
            if (ModelState.IsValid)
            {
                db.tbPresentacion.Add(tbPresentacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbPresentacion);
        }

        // GET: PruebaEntity/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbPresentacion tbPresentacion = db.tbPresentacion.Find(id);
            if (tbPresentacion == null)
            {
                return HttpNotFound();
            }
            return View(tbPresentacion);
        }

        // POST: PruebaEntity/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "codPresentacion,presentacion,estado")] tbPresentacion tbPresentacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbPresentacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbPresentacion);
        }

        // GET: PruebaEntity/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbPresentacion tbPresentacion = db.tbPresentacion.Find(id);
            if (tbPresentacion == null)
            {
                return HttpNotFound();
            }
            return View(tbPresentacion);
        }

        // POST: PruebaEntity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbPresentacion tbPresentacion = db.tbPresentacion.Find(id);
            db.tbPresentacion.Remove(tbPresentacion);
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
