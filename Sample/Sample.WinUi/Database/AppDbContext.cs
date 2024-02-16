// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.WinUi.Domain;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace CiccioSoft.VirtualList.Sample.WinUi.Database;

public class AppDbContext : DbContext
{
    public AppDbContext([NotNull] DbContextOptions options) : base(options) { }

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
