// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.Uwp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.Uwp.Repository
{
    public class FakeModelRepository : IModelRepository
    {
        private readonly List<Model> models;

        public FakeModelRepository()
        {
            models = DataService.ReadFromFile("SampleData.json");
        }

        public async Task<int> CountAsync(Expression<Func<Model, bool>> predicate,
                                          CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();
            await Task.Delay(1000, token);
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();
            return await Task.FromResult(models.Count(predicate.Compile()));
        }

        public async Task<List<Model>> GetRangeAsync(int skip,
                                                     int take,
                                                     Expression<Func<Model, bool>> predicate,
                                                     CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();
            await Task.Delay(3000, token);
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();
            return await Task.FromResult(models.Where(predicate.Compile()).Skip(skip).Take(take).ToList());
        }

        public void Dispose()
        {
        }
    }
}
