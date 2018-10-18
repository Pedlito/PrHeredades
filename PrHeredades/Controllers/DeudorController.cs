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
    public class DeudorController : Controller
    {
        private readonly int registrosPagina = 10;
        // GET: Deudor
        public ActionResult Index(int pagina = 1, string filtro = "")
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbDeudor> lista = (from t in db.tbDeudor
                                    where t.nombre.Contains(filtro)
                                    orderby t.nombre
                                    select t).ToList();
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Index", "Deudor");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        [HttpPost]
        public int CrearDeudor(tbDeudor deudor)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            db.tbDeudor.Add(deudor);
            db.SaveChanges();
            return deudor.codDeudor;
        }
    }
}