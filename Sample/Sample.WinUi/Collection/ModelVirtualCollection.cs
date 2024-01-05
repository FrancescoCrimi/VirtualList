using CiccioSoft.VirtualList.Sample.Domain;
using CiccioSoft.VirtualList.Sample.Repository;
using CiccioSoft.VirtualList.WinUi;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.WinUi.Collection;

public class ModelVirtualCollection : VirtualCollection<Model>
{
    public ModelVirtualCollection()
        : base(20, Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger<ModelVirtualCollection>())
    { }

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
