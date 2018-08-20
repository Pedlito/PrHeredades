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
    [TagPermiso(permiso = EnumPermisos.Usuarios)]
    public class UsuarioController : Controller
    {
        private readonly int registrosPagina = 10;
        // GET: Usuario
        public ActionResult Index(int pagina = 1, string filtro = "", bool estado = true)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbUsuario> lista = (from t in db.tbUsuario
                                     where t.codRol != 1 && t.nombre.Contains(filtro) && t.estado == estado
                                     orderby t.nombre
                                     select t).ToList();
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Index", "Usuario");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            ViewBag.estado = estado;
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        public ActionResult Crear()
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbRol> roles = db.tbRol.Where(t => t.codRol != 1).OrderBy(t => t.rol).ToList();
            ViewBag.codRol = new SelectList(roles, "codRol", "rol");
            return View();
        }

        [HttpPost]
        public ActionResult Crear(tbUsuario nuevo)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            if (!(db.tbUsuario.Any(t => t.usuario == nuevo.usuario)))
            {
                nuevo.estado = true;
                db.tbUsuario.Add(nuevo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                List<tbRol> roles = db.tbRol.Where(t => t.codRol != 1).OrderBy(t => t.rol).ToList();
                ViewBag.codRol = new SelectList(roles, "codRol", "rol");
                ModelState.AddModelError(string.Empty, "¡Ya existe este nombre de usuario!");
                return View();
            }
        }

        public ActionResult Editar(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbUsuario usuario = db.tbUsuario.Find(id);
            List<tbRol> roles = db.tbRol.Where(t => t.codRol != 1).OrderBy(t => t.rol).ToList();
            ViewBag.codRol = new SelectList(roles, "codRol", "rol");
            return View(usuario);
        }

        [HttpPost]
        public ActionResult Editar(tbUsuario editado)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            if (!(db.tbUsuario.Any(t => t.codUsuario != editado.codUsuario && t.usuario == editado.usuario )))
            {
                db.Entry(editado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                List<tbRol> roles = db.tbRol.Where(t => t.codRol != 1).OrderBy(t => t.rol).ToList();
                ViewBag.codRol = new SelectList(roles, "codRol", "rol");
                ModelState.AddModelError(string.Empty, "¡Ya existe ese usuario!");
                return View();
            }
        }

        public ActionResult CambiarEstado(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbUsuario usuario = db.tbUsuario.Find(id);
            return View(usuario);
        }

        [HttpPost]
        public ActionResult CambiarEstado(int id, FormCollection collection)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbUsuario usuario = db.tbUsuario.Find(id);
            usuario.estado = !(usuario.estado.Value);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}