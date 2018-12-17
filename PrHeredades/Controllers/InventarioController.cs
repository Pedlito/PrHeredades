using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrHeredades.Tags;
using PrHeredades.Models;

namespace PrHeredades.Controllers
{
    [TagAutenticacion]
    [TagPermiso(permiso = EnumPermisos.Existencia)]
    public class InventarioController : Controller
    {
        // GET: Inventario
        public ActionResult Index()
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbProductoPresentacion> lista = db.tbProductoPresentacion.OrderBy(t => t.tbProducto.producto).ToList();
            return View(lista);
        }
    }
}