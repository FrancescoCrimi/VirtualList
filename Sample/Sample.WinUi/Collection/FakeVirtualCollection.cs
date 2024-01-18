using CiccioSoft.VirtualList.Sample.Domain;
using CiccioSoft.VirtualList.Sample.Infrastructure;
using CiccioSoft.VirtualList.WinUi;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CiccioSoft.VirtualList.Sample.WinUi.Collection;

public class FakeVirtualCollection : IVirtualCollection<Model>
{
    private readonly ILogger logger;
    private List<Model> fakelist;
    private int count = 0;
    private const string CountString = "Count";
    private const string IndexerName = "Item[]";

    public FakeVirtualCollection(int total = 1000000)
    {
        logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger<FakeVirtualCollection>();
        fakelist = SampleGenerator.Generate(total);
        count = fakelist.Count;
    }

    private void OnNotifyCollectionReset()
    {
        //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CountString));
        //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        logger.LogDebug("Evento Collection Reset");
    }

    public Task LoadAsync(string str = "")
    {
        OnNotifyCollectionReset();
        return Task.CompletedTask;
    }


    #region Implemented Method Interface

    public Model this[int index]
    {
        get
        {
            var item = fakelist[index];
            logger.LogDebug("Index: {0}", index);
            return item;
        }
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

    IEnumerator<Model> IEnumerable<Model>.GetEnumerator() => fakelist.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IList)fakelist).GetEnumerator();

    int IList<Model>.IndexOf(Model item) => -1;

    int IList.IndexOf(object? value) => -1;

    #endregion


    #region interface member not implemented

    bool ICollection.IsSynchronized => throw new NotImplementedException();
    object ICollection.SyncRoot => throw new NotImplementedException();
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

    #endregion
}
