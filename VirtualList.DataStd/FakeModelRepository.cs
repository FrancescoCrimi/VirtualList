using CiccioSoft.VirtualList.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Data
{
    public class FakeModelRepository : IModelRepository
    {
        readonly int count;
        readonly List<Model> models;

        public FakeModelRepository(int count)
        {
            this.count = count;
            models = new List<Model>();
            for (uint i = 1; i <= count; i++)
            {
                Model model = GetRandomModel(i);
                models.Add(model);
            }
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
            Model aaa = new Model(i, str_build.ToString()) { Id = i };
            return aaa;
        }


        public void AddModel(Model model)
        {
            models.Add(model);
        }

        public int Count() => count;

        public int Count(Expression<Func<Model, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(count);
        }

        public Task<int> CountAsync(Expression<Func<Model, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void DeleteModel(int id)
        {
            throw new NotImplementedException();
        }

        public Model GetModelById(int id)
        {
            return models[id];
        }

        public List<Model> GetModels()
        {
            return models;
        }

        public Task<List<Model>> GetModelsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(models);
        }

        public List<Model> GetRangeModels(int skip, int take)
        {
            return models.Skip(skip).Take(take).ToList();
        }

        public Task<List<Model>> GetRangeModelsAsync(int skip, int take, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(models.Skip(skip).Take(take).ToList());
        }

        public int SaveChanges()
        {
            return 0;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0);
        }

        public void UpdateModel(Model model)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
