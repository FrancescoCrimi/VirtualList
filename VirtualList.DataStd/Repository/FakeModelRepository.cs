using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CiccioSoft.VirtualList.Data.Domain;
using CiccioSoft.VirtualList.Data.Infrastructure;

namespace CiccioSoft.VirtualList.Data.Repository
{
    public class FakeModelRepository : IModelRepository
    {
        private readonly int count;
        private readonly List<Model> models;

        public FakeModelRepository(int total = 1000)
        {
            count = total;
            models = SampleGenerator.Generate(total);
        }

        public void Add(Model model)
        {
            models.Add(model);
        }

        public int Count() => count;

        public int Count(Expression<Func<Model, bool>> predicate)
        {
            return count;
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(count);
        }

        public Task<int> CountAsync(Expression<Func<Model, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Model GetById(int id)
        {
            return models[id];
        }

        public List<Model> GetAll()
        {
            return models;
        }

        public Task<List<Model>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(models);
        }

        public List<Model> GetRange(int skip, int take)
        {
            return models.Skip(skip).Take(take).ToList();
        }

        public Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken = default)
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

        public void Update(Model model)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public Task<List<Model>> GetRangeAsync(int skip, int take, Expression<Func<Model, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
