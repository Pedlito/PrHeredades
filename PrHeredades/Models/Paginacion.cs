using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PrHeredades.Models
{
    public class Paginacion
    {
        public int actual { get; set; }
        public int paginas { get; set; }
        public string accion { get; set; }
        public string controlador { get; set; }

        public Paginacion(int actual, int paginas, string accion, string controlador)
        {
            this.actual = actual;
            this.paginas = paginas;
            this.accion = accion;
            this.controlador = controlador;
        }
    }
}