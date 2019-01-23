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
    [TagPermiso(permiso = EnumPermisos.Rol)]
    public class RolController : Controller
    {
        // GET: Rol
        public ActionResult Index()
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbRol> lista = db.tbRol.Where(t => t.codRol != 1).OrderBy(t => t.rol).ToList();
            return View(lista);
        }

        public ActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Crear(tbRol nuevo)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            if (!(db.tbRol.Any(t => t.rol == nuevo.rol)))
            {
                db.tbRol.Add(nuevo);
                db.SaveChanges();
                List<int> permisos = db.tbPermiso.Select(t => t.codPermiso).ToList();
                List <tbRolPermiso> rolPermisos = new List<tbRolPermiso>();
                foreach (int item in permisos)
                {
                    rolPermisos.Add(new tbRolPermiso
                    {
                        codRol = nuevo.codRol,
                        codPermiso = item,
                        estado = false
                    });
                }
                db.tbRolPermiso.AddRange(rolPermisos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "¡Ya existe este rol!");
                return View();
            }
        }

        public ActionResult Editar(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbRol rol = db.tbRol.Find(id);
            return View(rol);
        }

        [HttpPost]
        public ActionResult Editar(tbRol editado)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            if (!(db.tbRol.Any(t => t.rol == editado.rol && t.codRol != editado.codRol)))
            {
                db.Entry(editado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "¡Ya existe un rol con ese nombre!");
                return View();
            }
        }

        public ActionResult Permisos(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbRolPermiso> lista = (from t in db.tbRolPermiso where t.codRol == id select t).ToList();
            ViewBag.rol = lista[0].tbRol.rol;
            return View(lista);
        }

        public ActionResult CambiarEstadoPermiso(int codRol, int codPermiso)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbRolPermiso permiso = db.tbRolPermiso.Find(codRol, codPermiso);
            permiso.estado = !(permiso.estado);
            db.SaveChanges();
            return PartialView("_ListaPermisos", (from t in db.tbRolPermiso where t.codRol == codRol select t).ToList());
        }
    }
}