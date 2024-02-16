// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.Uwp.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.Uwp.Repository
{
    public interface IModelRepository : IDisposable
    {
        Task<List<Model>> GetRangeAsync(int skip,
                                        int take,
                                        Expression<Func<Model, bool>> predicate,
                                        CancellationToken token = default);
        Task<int> CountAsync(Expression<Func<Model, bool>> predicate,
                             CancellationToken token = default);
    }
}
