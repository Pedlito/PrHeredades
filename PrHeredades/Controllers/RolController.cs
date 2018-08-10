using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrHeredades.Models;
using PrHeredades.Tags;

namespace PrHeredades.Controllers
{
    [TagAutenticacion]
    [TagPermiso(permiso = EnumPermisos.Roles)]
    public class RolController : Controller
    {
        // GET: Rol
        public ActionResult Index()
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbRol> lista = (from t in db.tbRol where t.codRol != 1 orderby t.rol select t).ToList();
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
            if (!(from t in db.tbRol where t.rol == nuevo.rol select t).Any())
            {
                db.tbRol.Add(nuevo);
                db.SaveChanges();
                List<int> permisos = (from t in db.tbPermiso select t.codPermiso).ToList();
                List<tbRolPermiso> rolPermisos = new List<tbRolPermiso>();
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
            tbRol rol = (from t in db.tbRol where t.codRol == id select t).SingleOrDefault();
            return View(rol);
        }

        [HttpPost]
        public ActionResult Editar(tbRol editado)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbRol rol = (from t in db.tbRol where t.codRol == editado.codRol select t).SingleOrDefault();
            if (!(from t in db.tbRol where t.codRol != editado.codRol && t.rol == editado.rol select t).Any())
            {
                rol.rol = editado.rol;
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
            tbRolPermiso permiso = (from t in db.tbRolPermiso where t.codRol == codRol && t.codPermiso == codPermiso select t).SingleOrDefault();
            if (permiso.estado.Value)
            {
                permiso.estado = false;
            }
            else
            {
                permiso.estado = true;
            }
            db.SaveChanges();
            return PartialView("_ListaPermisos", (from t in db.tbRolPermiso where t.codRol == codRol select t).ToList());
        }
    }
}