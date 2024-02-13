using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.WinUi;

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
    new T this[int index] { get; set; }
    new int Count { get; }
    Task LoadAsync(string? searchString);
}
