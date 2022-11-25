using CiccioSoft.VirtualList.Sample.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.Repository
{
    public interface IModelRepository : IDisposable
    {
        Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken = default);
        Task<List<Model>> GetRangeAsync(int skip, int take, Expression<Func<Model, bool>> predicate, CancellationToken cancellationToken = default);

        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<Model, bool>> predicate,
                             CancellationToken cancellationToken = default);

        void Add(Model item);

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
