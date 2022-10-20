using CiccioSoft.VirtualList.Data.Domain;
using CiccioSoft.VirtualList.Data.Repository;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Uwp
{
    public class ModelVirtualCollection : VirtualCollection<Model>
    {
        private string searchString = string.Empty;

        public ModelVirtualCollection()
            : base()
        {
        }

        protected override Model CreateDummyEntity()
        {
            return new Model(0, "null");
        }

        protected async override Task<int> GetCountAsync()
        {
            using (var repo = Ioc.Default.GetRequiredService<IModelRepository>())
            {
                var aaa = await repo.CountAsync();
                return aaa;
            }
        }

        protected async override Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            using (var repo = Ioc.Default.GetRequiredService<IModelRepository>())
            {
                return await repo.GetRangeAsync(skip, take, cancellationToken);
            }
        }

        internal async Task SearchAsync(string searchString)
        {
            this.searchString = searchString;
            await Task.CompletedTask;
            //await ReloadAsync();
        }
    }
}
