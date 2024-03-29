﻿// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Wpf;

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

    Action? ScrollToTop { set; }
    Action? UnSelectIndex { set; }
}