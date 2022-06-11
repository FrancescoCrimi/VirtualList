using Microsoft.EntityFrameworkCore;

namespace CiccioSoft.VirtualList.DataStd.Database
{
    public class SqLiteDbContext : AppDbContext
    {
        public SqLiteDbContext() { }
        public SqLiteDbContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = WpfDataGridVirtual.db");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
