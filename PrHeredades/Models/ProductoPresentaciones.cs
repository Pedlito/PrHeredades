using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrHeredades.Models
{
    public class ProductoPresentaciones
    {
        public tbProducto producto { get; set; }
        public List<tbProductoPresentacion> presentaciones { get; set; }

        public ProductoPresentaciones(int id)
        {
            dbHeredadesEntities db = new dbHeredadesEntities();
            producto = db.tbProducto.Find(id);
            presentaciones = producto.tbProductoPresentacion.ToList();
        }
    }
}