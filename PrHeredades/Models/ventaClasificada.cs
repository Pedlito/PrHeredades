using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrHeredades.Models
{

    //Modelo para las ventas en curso.
    public class ventaClasificada
    {
        public string nombreProducto { get; set; }
        public decimal total { get; set; }
        public List<resumenVentasDia_Result> presetacionesProd { get; set; }
    }
}