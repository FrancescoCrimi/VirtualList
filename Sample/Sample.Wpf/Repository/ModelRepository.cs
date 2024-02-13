using CiccioSoft.VirtualList.Sample.Wpf.Database;
using CiccioSoft.VirtualList.Sample.Wpf.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.Wpf.Repository;

public class ModelRepository : IModelRepository
{
    private readonly AppDbContext _dbContext;

    public ModelRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> CountAsync(Expression<Func<Model, bool>> predicate,
                                CancellationToken token = default)
    {
        return _dbContext.Models.AsQueryable().CountAsync(predicate, token);
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
            .OrderBy(x => x.Id)
            .ToListAsync(token);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
