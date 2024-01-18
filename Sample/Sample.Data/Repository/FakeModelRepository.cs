using CiccioSoft.VirtualList.Sample.Domain;
using CiccioSoft.VirtualList.Sample.Infrastructure;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.Repository
{
    public class FakeModelRepository : IModelRepository
    {
        private readonly List<Model> models;

        public FakeModelRepository()
        {
            models = SampleGenerator.ReadFromFile("SampleData.json");
        }

        public Task<int> CountAsync(CancellationToken token = default)
        {
            return Task.FromResult(models.Count);
        }

        public Task<int> CountAsync(Expression<Func<Model, bool>> predicate,
                                    CancellationToken token = default)
        {
            return Task.FromResult(models.Count(predicate.Compile()));
        }

        public async Task<List<Model>> GetRangeAsync(int skip,
                                                     int take,
                                                     CancellationToken token = default)
        {
            await Task.Delay(1000, token);
            return await Task.FromResult(models.Skip(skip).Take(take).ToList());
        }

        public async Task<List<Model>> GetRangeAsync(int skip,
                                                     int take,
                                                     Expression<Func<Model, bool>> predicate,
                                                     CancellationToken token = default)
        {
            await Task.Delay(1000, token);
            return await Task.FromResult(models.Where(predicate.Compile()).Skip(skip).Take(take).ToList());
        }

        public void Dispose()
        {
        }
    }
}
