using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.UI.Xaml.Data;

namespace CiccioSoft.VirtualList.WinUi;

public class VirtualRangeCollection<T> : IList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged, IItemsRangeInfo where T : class
{
    T IList<T>.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    object? IList.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    int ICollection<T>.Count => throw new NotImplementedException();

    int ICollection.Count => throw new NotImplementedException();

    bool ICollection<T>.IsReadOnly => throw new NotImplementedException();

    bool IList.IsReadOnly => throw new NotImplementedException();

    bool IList.IsFixedSize => throw new NotImplementedException();

    bool ICollection.IsSynchronized => throw new NotImplementedException();

    object ICollection.SyncRoot => throw new NotImplementedException();

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    public void Dispose() => throw new NotImplementedException();
    public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems) => throw new NotImplementedException();
    void ICollection<T>.Add(T item) => throw new NotImplementedException();
    int IList.Add(object? value) => throw new NotImplementedException();
    void ICollection<T>.Clear() => throw new NotImplementedException();
    void IList.Clear() => throw new NotImplementedException();
    bool ICollection<T>.Contains(T item) => throw new NotImplementedException();
    bool IList.Contains(object? value) => throw new NotImplementedException();
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
    void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    int IList<T>.IndexOf(T item) => throw new NotImplementedException();
    int IList.IndexOf(object? value) => throw new NotImplementedException();
    void IList<T>.Insert(int index, T item) => throw new NotImplementedException();
    void IList.Insert(int index, object? value) => throw new NotImplementedException();
    bool ICollection<T>.Remove(T item) => throw new NotImplementedException();
    void IList.Remove(object? value) => throw new NotImplementedException();
    void IList<T>.RemoveAt(int index) => throw new NotImplementedException();
    void IList.RemoveAt(int index) => throw new NotImplementedException();
}