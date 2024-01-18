using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Uwp
{
    public interface IVirtualCollection<T> : ICollection<T>,
                                             IEnumerable<T>,
                                             IEnumerable,
                                             IList<T>,
                                             IReadOnlyCollection<T>,
                                             IReadOnlyList<T>,
                                             ICollection,
                                             IList,
                                             INotifyCollectionChanged,
                                             INotifyPropertyChanged where T : class
    {
        Task LoadAsync(string searchString = "");
    }
}
