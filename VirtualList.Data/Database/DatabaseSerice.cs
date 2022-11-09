using CiccioSoft.VirtualList.Data.Infrastructure;

namespace CiccioSoft.VirtualList.Data.Database
{
    public class DatabaseSerice
    {
        private readonly AppDbContext dbContext;

        public DatabaseSerice(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void LoadSample(int totale = 10000)
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            var list = SampleGenerator.Generate(totale);
            foreach (var item in list)
            {
                dbContext.Add(item);
            }
            dbContext.SaveChanges();
        }
    }
}
