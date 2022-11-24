using CiccioSoft.VirtualList.Data.Domain;
using CiccioSoft.VirtualList.Sample.Uwp.Database;
using CiccioSoft.VirtualList.Sample.Uwp.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.Uwp.Repository
{
    internal class ModelRepository : IModelRepository
    {
        private readonly AppDbContext appDbContext;

        public ModelRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public Task<List<Model>> GetRangeAsync(int skip,
                                               int take,
                                               Expression<Func<Model, bool>> predicate,
                                               CancellationToken cancellationToken = default)
        {
            IQueryable<Model> query = appDbContext.Models.AsQueryable();
            query = query.Where(predicate);
            query = query.Skip(skip);
            query = query.Take(take);
            //todo: fix order by
            query = query.OrderBy(m => m.Id);
            return query.ToListAsync(cancellationToken);
        }

        public Task<int> CountAsync(Expression<Func<Model, bool>> predicate,
                                    CancellationToken cancellationToken = default)
        {
            return appDbContext.Models.AsQueryable().CountAsync(predicate, cancellationToken);
        }

        public void Dispose()
        {
            appDbContext.Dispose();
        }
    }
}
