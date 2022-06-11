using CiccioSoft.VirtualList.DataStd.Domain;
using CiccioSoft.VirtualList.DataStd.Repository;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Uwp
{
    public class ModelIncrementalSource : IIncrementalSource<Model>
    {
        private readonly IModelRepository modelRepository;

        public ModelIncrementalSource()
        {
            modelRepository = Ioc.Default.GetRequiredService<IModelRepository>();
        }

        public async Task<IEnumerable<Model>> GetPagedItemsAsync(int pageIndex,
                                                                 int pageSize,
                                                                 CancellationToken cancellationToken = default)
        {
            var list = await modelRepository.GetRangeAsync(pageIndex * pageSize, pageSize, cancellationToken);
            return list.AsEnumerable();
        }
    }
}
