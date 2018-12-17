using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrHeredades.Models
{
    //Modelo que hereda de ProductoVenta con dos atributos mas que se reutilizara en los reportes de movimientos.
    public class productoGenerico : ProductoVenta
    {
        public string Mov { get; set; }
        public decimal GastoEntradaValor { get; set; }
        public string descripcion { get; set; }
    }
}