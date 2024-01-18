using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace CiccioSoft.VirtualList.Uwp
{
    public interface IVirtualRangeCollection<T> : ICollection<T>,
                                                  IEnumerable<T>,
                                                  IEnumerable,
                                                  IList<T>,
                                                  IReadOnlyCollection<T>,
                                                  IReadOnlyList<T>,
                                                  ICollection,
                                                  IList,
                                                  INotifyCollectionChanged,
                                                  INotifyPropertyChanged,
                                                  IItemsRangeInfo where T : class
    {
        Task LoadAsync(string searchString = "");
    }
}
