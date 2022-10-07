using CiccioSoft.VirtualList.Data.Domain;
using CiccioSoft.VirtualList.Data.Repository;
using CiccioSoft.VirtualList.Uwp.Collection;
using CommunityToolkit.Mvvm.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Uwp
{
    public class ModelVirtualRangeCollection : VirtualRangeList<Model>
    {
        public ModelVirtualRangeCollection() : base()
        {
            count = GetCount();
        }

        protected override Model CreateDummyEntity()
        {
            return new Model(0, "");
        }

        protected override int GetCount()
        {
            using (var repo = Ioc.Default.GetRequiredService<IModelRepository>())
                return repo.Count();
        }

        protected override Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            using (var repo = Ioc.Default.GetRequiredService<IModelRepository>())
                return repo.GetRangeAsync(skip, take, cancellationToken);
        }
    }
}
