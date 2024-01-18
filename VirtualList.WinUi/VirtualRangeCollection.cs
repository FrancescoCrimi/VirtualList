using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace CiccioSoft.VirtualList.WinUi;

/// <summary>
/// Collezione Virtuale
/// 
/// per funzionare correttamente impostare la Proprietà CacheLength dell'ItemStackPanel a 0.0 cosi
///
///     <ListView.ItemsPanel>
///       <ItemsPanelTemplate>
///         <ItemsStackPanel Orientation = "Vertical" CacheLength="0.0"/>
///       </ItemsPanelTemplate>
///     </ListView.ItemsPanel>
///     
/// Per usare la classe subclassa questa classe implementando i metodi astratti
/// 
/// </summary>

public class VirtualRangeCollection<T> : IVirtualRangeCollection<T> where T : class
{
    private readonly ILogger? _logger;
    private readonly List<T> fakelist;
    private readonly int _range;
    private int count = 0;

    public VirtualRangeCollection(int range = 20, ILogger? logger = null)
    {
        _logger = logger;
        fakelist = new List<T>();
        _range = range;
    }

    #region interface member implemented 

    public T this[int index]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    object? IList.this[int index] 
    {
        get => this[index];        
        set => throw new NotImplementedException();
    }

    public int Count => count;

    public bool IsReadOnly => true;

    public bool IsFixedSize => false;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public event PropertyChangedEventHandler? PropertyChanged;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => fakelist.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IList)fakelist).GetEnumerator();

    int IList<T>.IndexOf(T item) => -1;

    int IList.IndexOf(object? value) => -1;

    public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems) => throw new NotImplementedException();

    public void Dispose() => throw new NotImplementedException();

    #endregion


    #region interface member not implemented

    bool ICollection.IsSynchronized => throw new NotImplementedException();
    object ICollection.SyncRoot => throw new NotImplementedException();
    void ICollection<T>.Add(T item) => throw new NotImplementedException();
    int IList.Add(object? value) => throw new NotImplementedException();
    void ICollection<T>.Clear() => throw new NotImplementedException();
    void IList.Clear() => throw new NotImplementedException();
    bool ICollection<T>.Contains(T item) => throw new NotImplementedException();
    bool IList.Contains(object? value) => throw new NotImplementedException();
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
    void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();
    void IList<T>.Insert(int index, T item) => throw new NotImplementedException();
    void IList.Insert(int index, object? value) => throw new NotImplementedException();
    bool ICollection<T>.Remove(T item) => throw new NotImplementedException();
    void IList.Remove(object? value) => throw new NotImplementedException();
    void IList<T>.RemoveAt(int index) => throw new NotImplementedException();
    void IList.RemoveAt(int index) => throw new NotImplementedException();

    #endregion
}
