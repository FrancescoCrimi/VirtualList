// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.WinUi.Domain;
using CiccioSoft.VirtualList.Sample.WinUi.Repository;
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
    {
    }

    protected override Model CreateDummyEntity()
    {
        return new Model(0, "null");
    }

    protected async override Task<int> GetCountAsync(string? searchString)
    {
        using (var repo = Ioc.Default.GetRequiredService<IModelRepository>())
        {
            var aaa = await repo.CountAsync(m => !string.IsNullOrEmpty(m.Name) && m.Name.Contains(searchString!.ToUpper()));
            return aaa;
        }
    }

    protected async override Task<List<Model>> GetRangeAsync(string? searchString,
                                                             int skip,
                                                             int take,
                                                             CancellationToken token)
    {
        using (var repo = Ioc.Default.GetRequiredService<IModelRepository>())
        {
            searchString ??= "";
            return await repo.GetRangeAsync(skip, take, m => !string.IsNullOrEmpty(m.Name) && m.Name.Contains(searchString.ToUpper()), token);
        }
    }
}
