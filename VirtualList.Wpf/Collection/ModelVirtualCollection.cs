using CiccioSoft.VirtualList.Data.Domain;
using CiccioSoft.VirtualList.Data.Repository;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Wpf.Collection
{
    public class ModelVirtualCollection : VirtualCollection<Model>
    {
        private string searchString = string.Empty;

        public ModelVirtualCollection() : base() { }


        #region protected override method

        public async override Task LoadAsync(string searchString = "")
        {
            this.searchString = searchString;
            await LoadAsync();
        }

        protected override Model CreateDummyEntity()
        {
            return new Model(0, "null");
        }

        protected async override Task<int> GetCountAsync()
        {
            using IModelRepository? db = Ioc.Default.GetRequiredService<IModelRepository>();
            var count = await db.CountAsync(m => m.Name.Contains(searchString.ToUpper()));
            return count;
        }

        protected async override Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            using IModelRepository? db = Ioc.Default.GetRequiredService<IModelRepository>();
            List<Model> list = await db.GetRangeAsync(skip, take, m => m.Name.Contains(searchString.ToUpper()), cancellationToken);
            return list;
        }

        #endregion
    }
}