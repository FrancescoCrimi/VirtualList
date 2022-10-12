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
        private string strSearch = string.Empty;

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
            {
                //var rtn = repo.Count(m => m.Name.Contains(strSearch));
                var rtn = repo.Count();
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

        public void Load(String searchSting)
        {
            strSearch = searchSting;
            count = GetCount();
            //Suca();
        }
    }
}
