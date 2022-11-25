using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CiccioSoft.VirtualList.Sample.Repository;
using CiccioSoft.VirtualList.Sample.Domain;
using CommunityToolkit.Mvvm.DependencyInjection;
using CiccioSoft.VirtualList.WinUi;

namespace CiccioSoft.VirtualList.Sample.WinUi.Collection;
internal class ModelVirtualCollection : VirtualCollection<Model>
{

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
}
