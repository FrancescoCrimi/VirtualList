using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace CiccioSoft.VirtualList.Uwp
{
    public interface IVirtualRangeCollection<T> : IList<T>,
                                                  IList,
                                                  IReadOnlyList<T>,
                                                  INotifyCollectionChanged,
                                                  INotifyPropertyChanged,
                                                  IItemsRangeInfo where T : class
    {
        Task LoadAsync(string searchString = "");
    }
}
