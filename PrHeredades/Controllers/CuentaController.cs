using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrHeredades.Models;
using PrHeredades.Tags;

namespace PrHeredades.Controllers
{
    public class CuentaController : Controller
    {
        dbHeredadesEntities db = new dbHeredadesEntities();
        // GET: Cuenta
        [NoIniciarSesion]
        public ActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public ActionResult IniciarSesion(tbUsuario inicio)
        {
            tbUsuario prueba = (from t in db.tbUsuario where t.usuario == inicio.usuario && t.password == inicio.password select t).SingleOrDefault();
            if (prueba != null)
            {
                Sesion.Iniciar(new Usuario { codUsuario = prueba.codUsuario, nombre = prueba.nombre, usuario = prueba.usuario } );
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "El usuario y/o la contraseña son invalidos");
                return View();
            }
            
        }

        [TagPermiso(permiso = Permisos.Catalogos)]
        public ActionResult Prueba()
        {
            return View();
        }

        public ActionResult Registrarse()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrarse(tbUsuario nuevo)
        {
            db.tbUsuario.Add(nuevo);
            db.SaveChanges();
            Sesion.Iniciar(new Usuario { codUsuario = nuevo.codUsuario, nombre = nuevo.nombre, usuario = nuevo.usuario });
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult CerrarSesion()
        {
            Sesion.Cerrar();
            return RedirectToAction("Index", "Home");
        }
    }
}