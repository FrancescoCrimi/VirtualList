using CiccioSoft.VirtualList.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Wpf
{
    public class ModelVirtualCollection : VirtualCollection<Model>
    {
        private readonly IServiceProvider serviceProvider;

        public ModelVirtualCollection(IServiceProvider serviceProvider)
            : base()
        {
            this.serviceProvider = serviceProvider;
        }

        protected override Model CreateDummyEntity()
        {
            return new Model(0, "null");
        }

        protected override int GetCount()
        {
            using (IModelRepository? db = serviceProvider.GetRequiredService<IModelRepository>())
            {
                return db.Count();
            }
        }

        protected override async Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            using (IModelRepository? db = serviceProvider.GetRequiredService<IModelRepository>())
            {
                List<Model> list = await db.GetRangeModelsAsync(skip, take, cancellationToken);
                return list;
            }
        }
    }
}