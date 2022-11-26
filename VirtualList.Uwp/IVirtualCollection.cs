using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Uwp
{
    public interface IVirtualCollection<T> : IList<T>,
                                             IList,
                                             IReadOnlyList<T>,
                                             INotifyCollectionChanged,
                                             INotifyPropertyChanged where T : class
    {
        Task LoadAsync(string searchString = "");
    }
}
