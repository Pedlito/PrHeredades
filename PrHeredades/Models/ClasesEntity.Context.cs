﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class dbHeredadesEntities : DbContext
    {
        public dbHeredadesEntities()
            : base("name=dbHeredadesEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tbPermiso> tbPermiso { get; set; }
        public virtual DbSet<tbRol> tbRol { get; set; }
        public virtual DbSet<tbRolPermiso> tbRolPermiso { get; set; }
        public virtual DbSet<tbUsuario> tbUsuario { get; set; }
    }
}
