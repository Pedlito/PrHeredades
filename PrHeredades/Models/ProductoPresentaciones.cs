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
    }
}