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
    [TagPermiso(permiso = EnumPermisos.Usuarios)]
    public class UsuarioController : Controller
    {
        private readonly int registrosPagina = 10;
        // GET: Usuario
        public ActionResult Index(int pagina = 1, string filtro = "")
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbUsuario> lista = new List<tbUsuario>();
            if (filtro == "")
            {
                lista = (from t in db.tbUsuario
                         where t.codRol != 1
                         orderby t.nombre
                         select t).ToList();
            }
            else
            {
                lista = (from t in db.tbUsuario
                         where t.codRol != 1 && t.nombre.Contains(filtro)
                         orderby t.nombre
                         select t).ToList();
            }
            
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Index", "Usuario");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        public ActionResult Crear()
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbRol> roles = (from t in db.tbRol where t.codRol != 1 orderby t.codRol descending select t).ToList();
            ViewBag.codRol = new SelectList(roles, "codRol", "rol");
            return View();
        }

        [HttpPost]
        public ActionResult Crear(tbUsuario nuevo)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            if (!(from t in db.tbUsuario where t.usuario == nuevo.usuario select t).Any())
            {
                nuevo.estado = true;
                db.tbUsuario.Add(nuevo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                List<tbRol> roles = (from t in db.tbRol where t.codRol != 1 orderby t.codRol descending select t).ToList();
                ViewBag.codRol = new SelectList(roles, "codRol", "rol");
                ModelState.AddModelError(string.Empty, "¡Ya existe este nombre de usuario!");
                return View();
            }
        }

        public ActionResult Editar(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbUsuario usuario = (from t in db.tbUsuario where t.codUsuario == id select t).SingleOrDefault();
            List<tbRol> roles = (from t in db.tbRol where t.codRol != 1 orderby t.codRol descending select t).ToList();
            ViewBag.codRol = new SelectList(roles, "codRol", "rol");
            return View(usuario);
        }

        [HttpPost]
        public ActionResult Editar(tbUsuario editado)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbUsuario usuario = (from t in db.tbUsuario where t.codUsuario == editado.codUsuario select t).SingleOrDefault();
            if (!(from t in db.tbUsuario where t.codUsuario != editado.codUsuario && t.usuario == editado.usuario select t).Any())
            {
                usuario.nombre = editado.nombre;
                usuario.codRol = editado.codRol;
                usuario.usuario = editado.usuario;
                usuario.password = editado.password;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                List<tbRol> roles = (from t in db.tbRol where t.codRol != 1 orderby t.codRol descending select t).ToList();
                ViewBag.codRol = new SelectList(roles, "codRol", "rol");
                ModelState.AddModelError(string.Empty, "¡Ya existe ese usuario!");
                return View();
            }
        }

        public ActionResult CambiarEstado(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbUsuario usuario = (from t in db.tbUsuario where t.codUsuario == id select t).SingleOrDefault();
            return View(usuario);
        }

        [HttpPost]
        public ActionResult CambiarEstado(int id, FormCollection collection)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbUsuario usuario = (from t in db.tbUsuario where t.codUsuario == id select t).SingleOrDefault();
            if (usuario.estado.Value)
            {
                usuario.estado = false;
            }
            else
            {
                usuario.estado = true;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}