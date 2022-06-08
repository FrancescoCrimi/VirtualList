using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace CiccioSoft.VirtualList.Data
{
    public class SqLiteDbContext : AppDbContext
    {
        public SqLiteDbContext() { }

        public SqLiteDbContext([NotNull] DbContextOptions options) : base(options) { }
    }
}
