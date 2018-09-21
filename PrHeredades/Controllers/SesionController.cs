using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrHeredades.Models;
using PrHeredades.Tags;

namespace PrHeredades.Controllers
{
    public class SesionController : Controller
    {
        // GET: Cuenta
        [NoIniciarSesion]
        public ActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public ActionResult IniciarSesion(tbUsuario inicio)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbUsuario prueba = db.tbUsuario.Where(t => t.usuario == inicio.usuario && t.password == inicio.password).SingleOrDefault();
            if (prueba != null)
            {
                if (prueba.estado.Value)
                {
                    Sesion.Iniciar(new Usuario {
                        codUsuario = prueba.codUsuario,
                        codRol = prueba.codRol,
                        nombre = prueba.nombre,
                        usuario = prueba.usuario
                    });
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "¡Este usuario esta deshabilitado!");
                    return View(inicio);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "¡El usuario y/o la contraseña son invalidos!");
                return View(inicio);
            }
        }

        [HttpPost]
        public ActionResult CerrarSesion()
        {
            Sesion.Cerrar();
            return RedirectToAction("Index", "Home");
        }
    }
}