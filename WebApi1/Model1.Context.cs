﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApi1
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class KPI_Datamart_1Entities1 : DbContext
    {
        public KPI_Datamart_1Entities1()
            : base("name=KPI_Datamart_1Entities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<a0> a0 { get; set; }
        public virtual DbSet<a1> a1 { get; set; }
        public virtual DbSet<atw> atws { get; set; }
    }
}
