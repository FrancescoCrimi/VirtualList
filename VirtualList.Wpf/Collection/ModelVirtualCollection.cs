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
        public ModelVirtualCollection() : base() { }

        protected override Model CreateDummyEntity()
        {
            return new Model(0, "null");
        }

        protected override int GetCount()
        {
            using IModelRepository? db = Ioc.Default.GetRequiredService<IModelRepository>();
            return db.Count();
        }

        protected override async Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            using IModelRepository? db = Ioc.Default.GetRequiredService<IModelRepository>();
            List<Model> list = await db.GetRangeAsync(skip, take, cancellationToken);
            return list;
        }
    }
}