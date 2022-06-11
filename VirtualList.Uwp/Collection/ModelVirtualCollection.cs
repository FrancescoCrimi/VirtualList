using CiccioSoft.VirtualList.DataStd.Domain;
using CiccioSoft.VirtualList.DataStd.Repository;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Uwp
{
    public class ModelVirtualCollection : VirtualCollection<Model>
    {
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

        protected override Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            using (var repo = Ioc.Default.GetRequiredService<IModelRepository>())
                return repo.GetRangeAsync(skip, take, cancellationToken);
        }
    }
}
