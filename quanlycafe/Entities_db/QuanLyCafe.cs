using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace quanlycafe.Entities_db
{
    public partial class QuanLyCafe : DbContext
    {
        public QuanLyCafe()
            : base("name=QuanLyCafe")
        {
        }

        public virtual DbSet<BAN> BAN { get; set; }
        public virtual DbSet<CTHD> CTHD { get; set; }
        public virtual DbSet<DANHMUC> DANHMUC { get; set; }
        public virtual DbSet<MON> MON { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BAN>()
                .HasMany(e => e.CTHD)
                .WithRequired(e => e.BAN)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MON>()
                .HasMany(e => e.CTHD)
                .WithRequired(e => e.MON)
                .WillCascadeOnDelete(false);
        }
    }
}
