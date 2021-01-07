using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClosureTable.Models.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("DefaultConnection")
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<AppDbContext, Migrations.Configuration>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // has many ancestors
            modelBuilder.Entity<Establishment>().HasMany(p => p.Ancestors)
                .WithRequired(d => d.Offspring)
                .HasForeignKey(d => d.OffspringId)
                .WillCascadeOnDelete(false);

            // has many offspring
            modelBuilder.Entity<Establishment>().HasMany(p => p.Offspring)
                .WithRequired(d => d.Ancestor)
                .HasForeignKey(d => d.AncestorId)
                .WillCascadeOnDelete(false);
        }

        public DbSet<Establishment> Establishments { get; set; }
        public DbSet<EstablishmentNode> EstablishmentNodes { get; set; }
    }
}
