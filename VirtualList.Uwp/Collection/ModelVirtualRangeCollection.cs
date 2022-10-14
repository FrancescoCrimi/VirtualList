using CiccioSoft.VirtualList.Data.Domain;
using CiccioSoft.VirtualList.Data.Repository;
using CiccioSoft.VirtualList.Uwp.Collection;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CiccioSoft.VirtualList.Uwp
{
    public class ModelVirtualRangeCollection : VirtualRangeList<Model>
    {
        public ModelVirtualRangeCollection() : base()
        {
        }

        #region Protected Method

        protected override Model CreateDummyEntity()
        {
            return new Model(0, "null");
        }
         
        protected override async Task<int> GetCountAsync()
        {
            using (var repo = Ioc.Default.GetRequiredService<IModelRepository>())
            {
                var rtn = await repo.CountAsync();
                return rtn;
            }
        }

        protected override async Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            using (var repo = Ioc.Default.GetRequiredService<IModelRepository>())
            {
                return await repo.GetRangeAsync(skip, take, cancellationToken);
            }
        }

        #endregion
    }
}
