using CiccioSoft.VirtualList.Data.Domain;
using CiccioSoft.VirtualList.Sample.Uwp.Database;
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
        private readonly AppDbContext _dbContext;

        public ModelRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
        }

        public Task<List<Model>> GetRangeAsync(int skip,
                                               int take,
                                               Expression<Func<Model, bool>> predicate,
                                               CancellationToken token = default)
        {
            return _dbContext.Models.AsQueryable()
                .Where(predicate)
                .Skip(skip)
                .Take(take)
                .OrderBy(m => m.Id)
                .ToListAsync(token);
        }

        public Task<int> CountAsync(Expression<Func<Model, bool>> predicate,
                                    CancellationToken token = default)
        {
            return _dbContext.Models.AsQueryable()
                .CountAsync(predicate, token);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
