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
    
    public partial class tbProductoPresentacion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbProductoPresentacion()
        {
            this.tbProductoProveedor = new HashSet<tbProductoProveedor>();
            this.tbProductoTransaccion = new HashSet<tbProductoTransaccion>();
            this.tbVentaProducto = new HashSet<tbVentaProducto>();
        }
    
        public int codProducto { get; set; }
        public int codPresentacion { get; set; }
        public decimal precioVentaMinimo { get; set; }
        public decimal precioVentaMedio { get; set; }
        public decimal precioVentaMaximo { get; set; }
        public decimal agregado { get; set; }
        public short unidades { get; set; }
        public short correlativo { get; set; }
        public decimal existencia { get; set; }
    
        public virtual tbPresentacion tbPresentacion { get; set; }
        public virtual tbProducto tbProducto { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbProductoProveedor> tbProductoProveedor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbProductoTransaccion> tbProductoTransaccion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbVentaProducto> tbVentaProducto { get; set; }
    }
}
