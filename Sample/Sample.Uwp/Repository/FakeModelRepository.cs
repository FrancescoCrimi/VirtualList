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

        public FakeModelRepository(SampleDataService databaseSerice)
        {
            //models = databaseSerice.Generate(1000000);
            models = databaseSerice.ReadFromFile("SampleData.json");
        }

        public Task<int> CountAsync(Expression<Func<Model, bool>> predicate,
                                    CancellationToken token = default)
        {
            return Task.FromResult(models.Count(predicate.Compile()));
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
