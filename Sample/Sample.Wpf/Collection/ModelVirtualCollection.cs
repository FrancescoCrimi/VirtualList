using CiccioSoft.VirtualList.Sample.Domain;
using CiccioSoft.VirtualList.Sample.Repository;
using CiccioSoft.VirtualList.Wpf;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.Wpf.Collection;

public class ModelVirtualCollection : VirtualCollection<Model>
{
    public ModelVirtualCollection()
        : base(20, Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger<ModelVirtualCollection>())
    {
    }


    #region protected override method

    protected override Model CreateDummyEntity()
    {
        return new Model(0, "null");
    }

    protected async override Task<int> GetCountAsync(string? searchString)
    {
        searchString ??= string.Empty;
        using var db = Ioc.Default.GetRequiredService<IModelRepository>();
        var count = await db.CountAsync(m => !string.IsNullOrEmpty(m.Name) && m.Name.Contains(searchString.ToUpper()));
        return count;
    }

    protected async override Task<List<Model>> GetRangeAsync(string? searchString, int skip, int take, CancellationToken cancellationToken)
    {
        searchString ??= string.Empty;
        using var repo = Ioc.Default.GetRequiredService<IModelRepository>();
        var list = await repo.GetRangeAsync(skip, take, m => !string.IsNullOrEmpty(m.Name) && m.Name.Contains(searchString.ToUpper()), cancellationToken);
        return list;
    }

    #endregion
}
