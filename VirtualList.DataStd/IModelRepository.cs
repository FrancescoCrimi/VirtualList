using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Data
{
    public interface IModelRepository : IDisposable
    {
        List<Model> GetModels();
        Task<List<Model>> GetModelsAsync(CancellationToken cancellationToken = default);

        List<Model> GetRangeModels(int skip, int take);
        Task<List<Model>> GetRangeModelsAsync(int skip, int take, CancellationToken cancellationToken = default);

        int Count();
        int Count(Expression<Func<Model, bool>> predicate);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<Model, bool>> predicate,
                             CancellationToken cancellationToken = default);

        Model GetModelById(int id);
        void AddModel(Model model);
        void UpdateModel(Model model);
        void DeleteModel(int id);

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
