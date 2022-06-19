using Microsoft.EntityFrameworkCore;

namespace CiccioSoft.VirtualList.Data.Database
{
    public class SqlServerDbContext : AppDbContext
    {
        public SqlServerDbContext() { }
        public SqlServerDbContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
