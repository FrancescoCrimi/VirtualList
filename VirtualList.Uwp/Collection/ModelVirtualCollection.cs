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

        public ModelVirtualCollection() : base()
        {
            count = GetCount();
        }

        protected override Model CreateDummyEntity()
        {
            return new Model(0, "null");
        }

        protected override int GetCount()
        {
            using (var repo = Ioc.Default.GetRequiredService<IModelRepository>())
                return repo.Count();
        }

        protected override async Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            using (var repo = Ioc.Default.GetRequiredService<IModelRepository>())
                return await repo.GetRangeAsync(skip, take, cancellationToken);
        }

        internal async Task SearchAsync(string searchString)
        {
            this.searchString = searchString;
            await Task.CompletedTask;
            //await ReloadAsync();
        }
    }
}
