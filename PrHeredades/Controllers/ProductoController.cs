using PrHeredades.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PrHeredades.Tags;

namespace PrHeredades.Controllers
{
    [TagAutenticacion]
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

        // --- Crear ---
        public ActionResult Crear()
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            List<tbPresentacion> presentaciones = db.tbPresentacion.Where(t => t.estado == true).OrderBy(t => t.presentacion).ToList();
            ViewBag.codPresentacion = new SelectList(presentaciones, "codPresentacion", "presentacion");
            List<tbCategoria> categorias = db.tbCategoria.Where(t => t.estado == true).OrderBy(t => t.categoria).ToList();
            ViewBag.codCategoria = new SelectList(categorias, "codCategoria", "categoria");
            return View();
        }

        [HttpPost]
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
                foreach (tbProductoPresentacion presentacion in modelo.presentaciones)
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

        // --- Editar ---
        public ActionResult Editar(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbProducto producto = db.tbProducto.Find(id);
            List<int> usadas = producto.tbProductoPresentacion.Where(t => t.correlativo > 0).Select(t => t.codPresentacion).ToList();
            List<tbPresentacion> presentaciones = db.tbPresentacion.Where(t => t.estado == true && !usadas.Contains(t.codPresentacion)).OrderBy(t => t.presentacion).ToList();
            ViewBag.codPresentacion = new SelectList(presentaciones, "codPresentacion", "presentacion");
            List<tbCategoria> categorias = db.tbCategoria.Where(t => t.estado == true).OrderBy(t => t.categoria).ToList();
            ViewBag.codCategoria = new SelectList(categorias, "codCategoria", "categoria");
            ViewBag.presentaciones = producto.tbProductoPresentacion.Where(t => t.correlativo > 0).Select(t => new { t.codProducto, t.codPresentacion, t.precioVenta, t.unidades });
            return View(producto);
        }

        [HttpPost]
        public int ConfirmarEdicion(ProductoPresentaciones modelo)
        {
            if (modelo.presentaciones == null)
            {
                return 3;
            }
            dbHeredadesEntities db = new dbHeredadesEntities();
            if (!(db.tbProducto.Where(t => t.codProducto != modelo.producto.codProducto).Any(t => t.producto == modelo.producto.producto)))
            {
                List<tbProductoPresentacion> presentaciones = modelo.presentaciones;
                tbProducto producto = modelo.producto;
                producto.estado = true;
                // realiza cambios en producto como tal
                db.Entry(producto).State = EntityState.Modified;
                // logica para reordenar e insertar presentaciones
                List<tbProductoPresentacion> guardadas = db.tbProductoPresentacion.Where(t => t.codProducto == producto.codProducto).ToList();
                List<int> editadas = new List<int>();
                short correlativo = 1;
                foreach (tbProductoPresentacion item in presentaciones)
                {
                    if (guardadas.Select(t => t.codPresentacion).Contains(item.codPresentacion))
                    {
                        // ya existe esta presentacion en bd: hay que editarla y reordenarla 
                        // como existe en bd, la quito de la lista de guardadas para generar una lista con las existentes que no estan en las nuevas
                        int index = guardadas.FindIndex(t => t.codPresentacion == item.codPresentacion);
                        if (index > -1)
                        {
                            guardadas[index].precioVenta = item.precioVenta;
                            guardadas[index].unidades = item.unidades;
                            guardadas[index].correlativo = correlativo;
                        }
                        //se agregar a List editadas para luego cambiar correlativo de no editadas
                        editadas.Add(index);
                    }
                    else
                    {
                        // no existe esta presentacion en bd: hay que insertarla y ordenarla 
                        item.codProducto = producto.codProducto;
                        item.correlativo = correlativo;
                        db.tbProductoPresentacion.Add(item);
                    }
                    correlativo++;
                }
                for (int i = 0; i < guardadas.Count; i++)
                {
                    if (!editadas.Contains(i))
                    {
                        guardadas[i].correlativo = 0;
                        // hay que deshabilitarla en todos los productoProveedores
                        db.DeshabilitarProductos(guardadas[i].codProducto, guardadas[i].codPresentacion);
                    }
                }
                db.SaveChanges();
                return 1;
            }
            else
            {
                return 2;
            }
        }

        // --- Cambiar estado ---
        public ActionResult CambiarEstado(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbProducto producto = db.tbProducto.Find(id);
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarEstado(int id, FormCollection collection)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            tbProducto producto = db.tbProducto.Find(id);
            producto.estado = !(producto.estado.Value);
            db.SaveChanges();
            return RedirectToAction("Index");
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