using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrHeredades.Models
{
    public class Usuario
    {
        public int codUsuario { get; set; }
        public int codRol { get; set; }
        public string usuario { get; set; }
        public string nombre { get; set; }
    }
}