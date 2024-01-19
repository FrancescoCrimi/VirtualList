﻿using Microsoft.UI.Xaml.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CiccioSoft.VirtualList.WinUi;

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
    new int Count { get; }
}