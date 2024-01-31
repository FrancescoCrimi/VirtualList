using CiccioSoft.VirtualList.Sample.Uwp.Database;
using CiccioSoft.VirtualList.Sample.Uwp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.Uwp.Repository
{
    public class FakeModelRepository : IModelRepository
    {
        private readonly List<Model> models;

        public FakeModelRepository()
        {
            models = SampleDataService.ReadFromFile("SampleData.json");
        }

        public async Task<int> CountAsync(Expression<Func<Model, bool>> predicate,
                                          CancellationToken token = default)
        {
            await Task.Delay(3000, token);
            return await Task.FromResult(models.Count(predicate.Compile()));
        }

        public async Task<List<Model>> GetRangeAsync(int skip,
                                                     int take,
                                                     Expression<Func<Model, bool>> predicate,
                                                     CancellationToken token = default)
        {
            await Task.Delay(3000, token);
            return await Task.FromResult(models.Where(predicate.Compile()).Skip(skip).Take(take).ToList());
        }

        public void Dispose()
        {
        }
    }
}
