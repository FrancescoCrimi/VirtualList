using CiccioSoft.VirtualList.Sample.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.Repository;

public interface IModelRepository : IDisposable
{
    Task<List<Model>> GetRangeAsync(int skip,
                                    int take,
                                    CancellationToken token = default);
    Task<List<Model>> GetRangeAsync(int skip,
                                    int take,
                                    Expression<Func<Model, bool>> predicate,
                                    CancellationToken token = default);
    Task<int> CountAsync(CancellationToken token = default);
    Task<int> CountAsync(Expression<Func<Model, bool>> predicate,
                         CancellationToken token = default);
}
