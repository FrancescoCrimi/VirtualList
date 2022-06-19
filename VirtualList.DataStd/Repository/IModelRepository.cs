using CiccioSoft.VirtualList.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Data.Repository
{
    public interface IModelRepository : IDisposable
    {
        List<Model> GetAll();
        Task<List<Model>> GetAllAsync(CancellationToken cancellationToken = default);

        List<Model> GetRange(int skip, int take);
        Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken = default);

        int Count();
        int Count(Expression<Func<Model, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<Model, bool>> predicate,
                             CancellationToken cancellationToken = default);

        Model GetById(int id);
        void Add(Model model);
        void Update(Model model);
        void Delete(int id);

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
