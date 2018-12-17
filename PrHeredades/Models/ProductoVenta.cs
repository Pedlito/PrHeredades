using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrHeredades.Models
{
    public class ProductoVenta
    {
        public int codProducto { get; set; }
        public string producto { get; set; }
        public int codPresentacion { get; set; }
        public string presentacion { get; set; }
        public decimal cantidad { get; set; }
        public decimal existencia { get; set; }
        public decimal precioVenta { get; set; }
        public bool tieneMayor { get; set; }
    }
}