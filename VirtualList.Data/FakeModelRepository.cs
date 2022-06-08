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
        private readonly int count;
        private readonly List<Model> models;

        public FakeModelRepository(int count = 0)
        {
            this.count = count;
            models = new List<Model>();
            if(count != 0)
            {
                for (uint i = 0; i < count; i++)
                {
                    models.Add(GetRandomModel(i));
                }
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
            Model model = new Model(i, str_build.ToString()) { Id = i };
            return model;
        }

        public int Count()
        {
            return count;
        }

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

        public void Add(Model entity)
        {
            throw new NotImplementedException();
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}
