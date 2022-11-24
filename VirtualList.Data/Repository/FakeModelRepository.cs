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

        public FakeModelRepository(int total = 1000000)
        {
            count = total;
            models = SampleGenerator.Generate(total);
        }

        public void Add(Model item)
        {
            models.Add(item);
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(count);
        }

        public Task<int> CountAsync(Expression<Func<Model, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(models.Count(predicate.Compile()));
        }

        public Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(models.Skip(skip).Take(take).ToList());
        }

        public Task<List<Model>> GetRangeAsync(int skip, int take, Expression<Func<Model, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(models.Where(predicate.Compile()).Skip(skip).Take(take).ToList());
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
