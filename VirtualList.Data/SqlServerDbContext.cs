using Microsoft.EntityFrameworkCore;

namespace CiccioSoft.VirtualList.Data
{
    public class SqlServerDbContext : AppDbContext
    {
        public SqlServerDbContext() { }
        public SqlServerDbContext(DbContextOptions options) : base(options) { }
    }
}
