using CiccioSoft.VirtualList.Data.Domain;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace CiccioSoft.VirtualList.Data.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext([NotNull] DbContextOptions options) : base(options) { }

        public DbSet<Model> Models
        {
            get; set;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Model>(m =>
            {
                m.Property(x => x.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd();
                m.Property(x => x.Name);
                m.Property(x => x.Numero);
                m.HasKey(x => x.Id);
            });
        }
    }
}
