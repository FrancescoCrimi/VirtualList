using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CiccioSoft.VirtualList.Data.Domain;
using CiccioSoft.VirtualList.Data.Repository;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace CiccioSoft.VirtualList.Uwp.Collection
{
    public class ModelVirtualCollection : VirtualCollection<Model>
    {
        private string searchString = string.Empty;

        public ModelVirtualCollection()
            : base()
        {
        }


        #region protected override method

        protected override Model CreateDummyEntity()
        {
            return new Model(0, "null");
        }

        protected async override Task<int> GetCountAsync()
        {
            using (var repo = Ioc.Default.GetRequiredService<IModelRepository>())
            {
                var rtn = await repo.CountAsync(m => m.Name.Contains(searchString.ToUpper()));
                return rtn;
            }
        }

        protected async override Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            using (var repo = Ioc.Default.GetRequiredService<IModelRepository>())
            {
                return await repo.GetRangeAsync(skip, take, m => m.Name.Contains(searchString.ToUpper()), cancellationToken);
            }
        }

        #endregion


        public async Task LoadAsync(string searchString = "")
        {
            this.searchString = searchString;
            await InitAsync();
        }
    }
}
