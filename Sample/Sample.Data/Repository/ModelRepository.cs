using CiccioSoft.VirtualList.Sample.Database;
using CiccioSoft.VirtualList.Sample.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.Repository
{
    internal class ModelRepository : IModelRepository
    {
        private readonly AppDbContext appDbContext;

        public ModelRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return appDbContext.Models.AsQueryable().CountAsync(cancellationToken);
        }

        public Task<int> CountAsync(Expression<Func<Model, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return appDbContext.Models.AsQueryable().CountAsync(predicate, cancellationToken);
        }

        public Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken = default)
        {
            IQueryable<Model> query = appDbContext.Models.AsQueryable();
            query = query.Skip(skip);
            query = query.Take(take);
            return query.ToListAsync(cancellationToken);
        }

        public Task<List<Model>> GetRangeAsync(int skip, int take, Expression<Func<Model, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IQueryable<Model> query = appDbContext.Models.AsQueryable();
            query = query.Where(predicate);
            query = query.Skip(skip);
            query = query.Take(take);
            return query.ToListAsync(cancellationToken);
        }

        public void Add(Model entity)
        {
            appDbContext.Add(entity);
        }

        public int SaveChanges()
        {
            return appDbContext.SaveChanges();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return appDbContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            appDbContext.Dispose();
        }
    }
}
