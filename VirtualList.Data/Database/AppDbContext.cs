using CiccioSoft.VirtualList.Data.Domain;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace CiccioSoft.VirtualList.Data.Database
{
    public abstract class AppDbContext : DbContext
    {
#pragma warning disable CS8618 // Il campo non nullable deve contenere un valore non Null all'uscita dal costruttore. Provare a dichiararlo come nullable.
        protected AppDbContext([NotNull] DbContextOptions options) : base(options) { }
#pragma warning restore CS8618 // Il campo non nullable deve contenere un valore non Null all'uscita dal costruttore. Provare a dichiararlo come nullable.

        public DbSet<Model> Models { get; set; }

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
