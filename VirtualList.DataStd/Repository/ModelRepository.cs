using CiccioSoft.VirtualList.Data.Database;
using CiccioSoft.VirtualList.Data.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Data.Repository
{
    internal class ModelRepository : IModelRepository
    {
        private readonly AppDbContext appDbContext;

        public ModelRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }


        public List<Model> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<List<Model>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public List<Model> GetRange(int skip, int take)
        {
            IQueryable<Model> query = appDbContext.Models.AsQueryable();
            query = query.Skip(skip);
            query = query.Take(take);
            return query.ToList();
        }

        public Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken = default)
        {
            IQueryable<Model> query = appDbContext.Models.AsQueryable();
            query = query.Skip(skip);
            query = query.Take(take);
            //todo: fix order by
            query = query.OrderBy(m => m.Id);
            return query.ToListAsync(cancellationToken);
        }

        public Task<List<Model>> GetRangeAsync(int skip, int take, Expression<Func<Model, bool>> predicate, CancellationToken cancellationToken = default)
        {
            IQueryable<Model> query = appDbContext.Models.AsQueryable();
            query = query.Where(predicate);
            query = query.Skip(skip);
            query = query.Take(take);
            //todo: fix order by
            query = query.OrderBy(m => m.Id);
            return query.ToListAsync(cancellationToken);
        }


        public void Add(Model entity)
        {
            appDbContext.Add(entity);
        }

        public int Count()
        {
            return appDbContext.Models.AsQueryable().Count();
        }

        public int Count(Expression<Func<Model, bool>> predicate)
        {
            return appDbContext.Models.AsQueryable().Count(predicate);
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return appDbContext.Models.AsQueryable().CountAsync(cancellationToken);
        }

        public Task<int> CountAsync(Expression<Func<Model, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return appDbContext.Models.AsQueryable().CountAsync(predicate, cancellationToken);
        }

        public Model GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Model model)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
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
