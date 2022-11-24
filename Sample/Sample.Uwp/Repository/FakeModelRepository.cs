using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CiccioSoft.VirtualList.Data.Domain;
using CiccioSoft.VirtualList.Sample.Uwp.Infrastructure;

namespace CiccioSoft.VirtualList.Sample.Uwp.Repository
{
    public class FakeModelRepository : IModelRepository
    {
        private readonly List<Model> models;

        public FakeModelRepository(int total = 1000000)
        {
            models = SampleGenerator.Generate(total);
        }

        public Task<int> CountAsync(Expression<Func<Model, bool>> predicate,
                                    CancellationToken cancellationToken = default)
        {
            return Task.FromResult(models.Where(predicate.Compile()).Count());
        }

        public Task<List<Model>> GetRangeAsync(int skip,
                                               int take,
                                               Expression<Func<Model, bool>> predicate,
                                               CancellationToken cancellationToken = default)
        {
            var rst = models.Where(predicate.Compile()).Skip(skip).Take(take).ToList();
            return Task.FromResult(rst);
        }

        public void Dispose()
        {
        }
    }
}
