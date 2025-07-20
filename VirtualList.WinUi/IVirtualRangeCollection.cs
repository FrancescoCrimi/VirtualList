// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using Microsoft.UI.Xaml.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.WinUi
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
        new T this[int index] { get; set; }

        // The ‘new’ modifiers unify the duplicated Count and indexer 
        // members inherited from both generic and non-generic interfaces.
        new int Count { get; }
        Task LoadAsync(string? searchString);
    }
}
