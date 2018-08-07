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
    [TagPermiso(permiso = Permisos.Usuarios)]
    public class UsuarioController : Controller
    {
        private readonly int registrosPagina = 4;
        dbHeredadesEntities db = new dbHeredadesEntities();
        // GET: Usuario
        public ActionResult Index(int pagina = 1, string filtro = "")
        {
            List<tbUsuario> lista = new List<tbUsuario>();
            if (filtro == "")
            {
                lista = (from t in db.tbUsuario
                         where t.codRol != 1
                         orderby t.nombre
                         select t).Skip((pagina - 1) * registrosPagina).Take(registrosPagina).ToList();
            }
            else
            {
                lista = (from t in db.tbUsuario
                         where t.codRol != 1 && t.nombre.Contains(filtro)
                         orderby t.nombre
                         select t).Skip((pagina - 1) * registrosPagina).Take(registrosPagina).ToList();
            }
            
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Index", "Usuario");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            return View(lista);
        }

        public ActionResult Crear()
        {
            List<tbRol> roles = (from t in db.tbRol where t.codRol != 1 orderby t.codRol descending select t).ToList();
            ViewBag.codRol = new SelectList(roles, "codRol", "rol");
            return View();
        }

        [HttpPost]
        public ActionResult Crear(tbUsuario nuevo)
        {
            if (!(from t in db.tbUsuario where t.usuario == nuevo.usuario select t).Any())
            {
                db.tbUsuario.Add(nuevo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Editar(int codUsuario)
        {
            List<tbRol> roles = (from t in db.tbRol where t.codRol != 1 orderby t.codRol descending select t).ToList();
            tbUsuario usuario = (from t in db.tbUsuario where t.codUsuario == codUsuario select t).SingleOrDefault();
            ViewBag.codRol = new SelectList(roles, "codRol", "rol");
            return View(usuario);
        }
        
        [HttpPost]
        public ActionResult Editar(int codUsuario, FormCollection datos)
        {
            tbUsuario usuario = (from t in db.tbUsuario where t.codUsuario == codUsuario select t).SingleOrDefault();
            usuario.nombre = datos["nombre"];
            usuario.codRol = int.Parse(datos["codRol"]);
            usuario.usuario = datos["usuario"];
            usuario.password = datos["password"];
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CambiarEstado(int codUsuario)
        {
            tbUsuario usuario = (from t in db.tbUsuario where t.codUsuario == codUsuario select t).SingleOrDefault();
            return View(usuario);
        }

        [HttpPost]
        public ActionResult CambiarEstado(int codUsuario, FormCollection collection)
        {
            tbUsuario usuario = (from t in db.tbUsuario where t.codUsuario == codUsuario select t).SingleOrDefault();
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