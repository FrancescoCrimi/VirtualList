using CiccioSoft.VirtualList.DataStd.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text;

namespace CiccioSoft.VirtualList.DataStd.Database
{
    public abstract class AppDbContext : DbContext
    {
        protected AppDbContext() { }
        protected AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Model> Models { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Model>(b =>
            {
                b.Property(x => x.Id)
                    .ValueGeneratedOnAdd();
                b.Property(x => x.Name);
                b.Property(x => x.Numero);
                b.HasKey(x => x.Id);
            });

            for (uint i = 1; i <= 10000; i++)
            {
                object model = GetRandomModel(i);
                modelBuilder.Entity<Model>().HasData(model);
            }
        }

        private object GetRandomModel(uint i)
        {
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();
            char letter;
            for (int l = 0; l < 7; l++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            //Model model = new Model(i, str_build.ToString());
            var aaa = new { Id = i, Numero = i, Name = str_build.ToString() };
            return aaa;
        }
    }
}
