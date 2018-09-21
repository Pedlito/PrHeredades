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
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [TagPermiso(permiso = EnumPermisos.Inventario)]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        [TagPermiso(permiso = EnumPermisos.Catalogos)]
        public ActionResult Catalogos()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}