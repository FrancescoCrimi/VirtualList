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
    private string _searchString = string.Empty;

    public ModelVirtualCollection()
        : base(20, Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger<ModelVirtualCollection>())
    { }


    #region protected override method

    public override async Task LoadAsync(string? searchString)
    {
        searchString ??= string.Empty;
        _searchString = searchString;
        await LoadAsync();
    }

    protected override Model CreateDummyEntity()
    {
        return new Model(0, "null");
    }

    protected async override Task<int> GetCountAsync()
    {
        using var db = Ioc.Default.GetRequiredService<IModelRepository>();
        var count = await db.CountAsync(m => m.Name.Contains(_searchString.ToUpper()));
        return count;
    }

    protected async override Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
    {
        using var repo = Ioc.Default.GetRequiredService<IModelRepository>();
        var list = await repo.GetRangeAsync(skip, take, m => m.Name.Contains(_searchString.ToUpper()), cancellationToken);
        return list;
    }

    #endregion
}
