using CiccioSoft.VirtualList.Sample.Uwp.Domain;
using CiccioSoft.VirtualList.Sample.Uwp.Repository;
using CiccioSoft.VirtualList.Uwp;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.Uwp.Collection
{
    public class ModelVirtualCollection : VirtualCollection<Model>
    {
        public ModelVirtualCollection()
            : base(20, Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger<ModelVirtualCollection>())
        {
        }

        protected override Model CreateDummyEntity()
        {
            return new Model(0, "null");
        }

        protected async override Task<int> GetCountAsync(string searchString)
        {
            using (var repo = Ioc.Default.GetRequiredService<IModelRepository>())
            {
                var rtn = await repo.CountAsync(m => m.Name.Contains(searchString.ToUpper()));
                return rtn;
            }
        }

        protected async override Task<List<Model>> GetRangeAsync(string searchString,
                                                                 int skip,
                                                                 int take,
                                                                 CancellationToken token)
        {
            using (var repo = Ioc.Default.GetRequiredService<IModelRepository>())
            {
                return await repo.GetRangeAsync(skip, take, m => m.Name.Contains(searchString.ToUpper()), token);
            }
        }
    }
}
