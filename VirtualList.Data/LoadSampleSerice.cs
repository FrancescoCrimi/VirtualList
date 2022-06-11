using CiccioSoft.VirtualList.Data.Domain;
using CiccioSoft.VirtualList.Data.Repository;
using System;
using System.Text;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Data
{
    public class LoadSampleSerice
    {
        private readonly IModelRepository dataService;

        public LoadSampleSerice(IModelRepository dataService)
        {
            this.dataService = dataService;
        }

        public async Task LoadSample()
        {
            for (uint i = 1; i <= 1000000; i++)
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
                dataService.Add(model);
            }
            await dataService.SaveChangesAsync();
        }
    }
}
