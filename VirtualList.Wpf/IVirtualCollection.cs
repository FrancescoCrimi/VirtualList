using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Wpf
{
    public interface IVirtualCollection<T> : IList<T>, IList, IReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged where T : class
    {
        public int SelectedIndex
        {
            get; set;
        }

        public Task LoadAsync(string searchString = "");

        new T this[int index] { get; set; }

        new int Count
        {
            get;
        }
    }
}