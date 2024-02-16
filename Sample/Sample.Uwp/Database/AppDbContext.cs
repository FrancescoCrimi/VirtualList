// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.Uwp.Domain;
using Microsoft.EntityFrameworkCore;

namespace CiccioSoft.VirtualList.Sample.Uwp.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Model> Models
        {
            get; set;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Model>(b =>
            {
                b.Property(x => x.Id)
                    .IsRequired()
                    .ValueGeneratedOnAdd();
                b.Property(x => x.Name);
                b.Property(x => x.Numero);
                b.HasKey(x => x.Id);
            });

            //for (uint i = 1; i <= 10000; i++)
            //{
            //    object model = GetRandomModel(i);
            //    modelBuilder.Entity<Model>().HasData(model);
            //}
        }
    }
}
