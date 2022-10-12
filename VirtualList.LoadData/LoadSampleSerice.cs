using CiccioSoft.VirtualList.Data.Database;
using CiccioSoft.VirtualList.Data.Domain;
using System;
using System.Text;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.LoadData
{
    public class LoadSampleSerice
    {
        private readonly AppDbContext dbContext;

        public LoadSampleSerice(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task LoadSample()
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            for (uint i = 1; i <= 10000; i++)
            {
                //StringBuilder str_build = new StringBuilder();
                //Random random = new Random();
                //char letter;
                //for (int l = 0; l < 7; l++)
                //{
                //    double flt = random.NextDouble();
                //    int shift = Convert.ToInt32(Math.Floor(25 * flt));
                //    letter = Convert.ToChar(shift + 65);
                //    str_build.Append(letter);
                //}
                //Model model = new Model(i, str_build.ToString());

                //Model model = GetRandomModel(i);
                //dbContext.Add(model);
            }
            await dbContext.SaveChangesAsync();
        }

        private Model GetRandomModel(uint i)
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
            Model model = new Model(i, str_build.ToString());
            return model;
        }
    }
}
