using PrHeredades.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrHeredades.Controllers
{
    public class ProductoController : Controller
    {
        private readonly int registrosPagina = 10;
        // GET: Categoria
        public ActionResult Index(int pagina = 1, string filtro = "", bool estado = true)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbProducto> lista = (from t in db.tbProducto
                                      where t.producto.Contains(filtro) && t.estado == estado
                                      orderby t.producto
                                      select t).ToList();
            int paginas = (int)Math.Ceiling((double)lista.Count() / registrosPagina);
            Paginacion paginacion = new Paginacion(pagina, paginas, "Index", "Categoria");
            ViewBag.paginacion = paginacion;
            ViewBag.filtro = filtro;
            ViewBag.estado = estado;
            return View(lista.Skip((pagina - 1) * registrosPagina).Take(registrosPagina));
        }

        public ActionResult Crear()
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbPresentacion> presentaciones = db.tbPresentacion.Where(t => t.estado == true).OrderBy(t => t.presentacion).ToList();
            ViewBag.codPresentacion = new SelectList(presentaciones, "codPresentacion", "presentacion");
            List<tbCategoria> categorias = db.tbCategoria.Where(t => t.estado == true).OrderBy(t => t.categoria).ToList();
            ViewBag.codCategoria = new SelectList(categorias, "codCategoria", "categoria");
            return View();
        }

        public int Guardar(ProductoPresentaciones modelo)
        {
            if (modelo.presentaciones == null)
            {
                return 3;
            }
            dbHeredadesEntities db = new dbHeredadesEntities();
            if (!(db.tbProducto.Any(t => t.producto == modelo.producto.producto)))
            {
                modelo.producto.estado = true;
                short correlativo = 1;
                foreach (var presentacion in modelo.presentaciones)
                {
                    presentacion.correlativo = correlativo;
                    modelo.producto.tbProductoPresentacion.Add(presentacion);
                    correlativo++;
                }
                db.tbProducto.Add(modelo.producto);
                db.SaveChanges();
                return 1;
            }
            else 
            { 
                return 2;
            }
        }

        public ActionResult Editar(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbProducto producto = db.tbProducto.Find(id);
            List<int> usadas = producto.tbProductoPresentacion.Select(t => t.codPresentacion).ToList();
            List<tbPresentacion> presentaciones = db.tbPresentacion.Where(t => t.estado == true && !usadas.Contains(t.codPresentacion)).OrderBy(t => t.presentacion).ToList();
            ViewBag.codPresentacion = new SelectList(presentaciones, "codPresentacion", "presentacion");
            List<tbCategoria> categorias = db.tbCategoria.Where(t => t.estado == true).OrderBy(t => t.categoria).ToList();
            ViewBag.codCategoria = new SelectList(categorias, "codCategoria", "categoria");
            ViewBag.presentaciones = producto.tbProductoPresentacion.Select(t => new { t.codProducto, t.codPresentacion, t.precio, t.unidades });
            return View(producto);
        }

        [HttpPost]
        public ActionResult Presentaciones(List<tbProductoPresentacion> lista)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            if (lista != null)
            {
                foreach (var item in lista)
                {
                    item.tbPresentacion = db.tbPresentacion.Find(item.codPresentacion);
                }
            }
            return PartialView("_Presentaciones", lista);
        }

        public ActionResult ProdPresentaciones(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbProductoPresentacion> lista = db.tbProductoPresentacion.Where(t => t.codProducto == id && t.correlativo > 0).OrderBy(t => t.correlativo).ToList();
            return PartialView("_ListaPresentaciones", lista);
        }

    }
}