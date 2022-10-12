using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace CiccioSoft.VirtualList.Data.Database
{
    public class SqLiteDbContext : AppDbContext
    {
        public SqLiteDbContext([NotNull] DbContextOptions options)
            : base(options)
        {
        }
    }
}
