// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using CiccioSoft.VirtualList.Sample.WinUi.Domain;
using CiccioSoft.VirtualList.WinUi;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.WinUi.Collection;

public class FakeVirtualRangeCollection : IVirtualRangeCollection<Model>
{
    private readonly ILogger _logger;
    private readonly List<Model> _list;
    private List<Model> _items;
    private const string CountString = "Count";
    private const string IndexerName = "Item[]";

    public FakeVirtualRangeCollection()
    {
        _logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger<FakeVirtualRangeCollection>();
        _list = DataService.ReadFromFile("SampleData.json");
        _items = new List<Model>();
    }


    #region Implemented Method Interface

    public Task LoadAsync(string? searchString)
    {
        searchString ??= string.Empty;
        _items = _list!.FindAll(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CountString));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        _logger.LogDebug("Collection Reset");
        return Task.CompletedTask;
    }

    public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
    {
        _logger.LogDebug("RangeChange {first} - {last}", visibleRange.FirstIndex, visibleRange.LastIndex);
    }

    public Model this[int index]
    {
        get
        {
            var item = _items[index];
            _logger.LogTrace("Index: {index} {time}", index, DateTime.Now.ToString("hh:mm:ss.fff"));
            return item;
        }
        set => throw new NotImplementedException();
    }

    object? IList.this[int index]
    {
        get => this[index];
        set => throw new NotImplementedException();
    }

    public int Count => _items.Count;

    public bool IsReadOnly => true;

    public bool IsFixedSize => false;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public event PropertyChangedEventHandler? PropertyChanged;

    IEnumerator<Model> IEnumerable<Model>.GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IList)_items).GetEnumerator();

    int IList<Model>.IndexOf(Model item) => _items.IndexOf(item);

    int IList.IndexOf(object? value) => ((IList)_items).IndexOf(value);

    public void Dispose() { }

    #endregion


    #region interface member not implemented

    void ICollection<Model>.Add(Model item) => throw new NotImplementedException();
    int IList.Add(object? value) => throw new NotImplementedException();
    void ICollection<Model>.Clear() => throw new NotImplementedException();
    void IList.Clear() => throw new NotImplementedException();
    bool ICollection<Model>.Contains(Model item) => throw new NotImplementedException();
    bool IList.Contains(object? value) => throw new NotImplementedException();
    void ICollection<Model>.CopyTo(Model[] array, int arrayIndex) => throw new NotImplementedException();
    void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();
    void IList<Model>.Insert(int index, Model item) => throw new NotImplementedException();
    void IList.Insert(int index, object? value) => throw new NotImplementedException();
    bool ICollection<Model>.Remove(Model item) => throw new NotImplementedException();
    void IList.Remove(object? value) => throw new NotImplementedException();
    void IList<Model>.RemoveAt(int index) => throw new NotImplementedException();
    void IList.RemoveAt(int index) => throw new NotImplementedException();
    bool ICollection.IsSynchronized => throw new NotImplementedException();
    object ICollection.SyncRoot => throw new NotImplementedException();

    #endregion
}
