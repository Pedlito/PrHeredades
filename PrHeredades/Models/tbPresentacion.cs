//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PrHeredades.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbPresentacion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbPresentacion()
        {
            this.tbProductoPresentacion = new HashSet<tbProductoPresentacion>();
        }
    
        public int codPresentacion { get; set; }
        public string presentacion { get; set; }
        public bool estado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbProductoPresentacion> tbProductoPresentacion { get; set; }
    }
}
