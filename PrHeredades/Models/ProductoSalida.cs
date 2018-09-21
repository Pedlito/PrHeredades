using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrHeredades.Models
{
    public class ProductoSalida
    {
        public int codProducto { get; set; }
        public string producto { get; set; }
        public int codPresentacion { get; set; }
        public string presentacion { get; set; }
        public int cantidad { get; set; }
        public int existencia { get; set; }
    }
}