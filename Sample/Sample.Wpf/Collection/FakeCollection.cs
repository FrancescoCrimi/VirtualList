using CiccioSoft.VirtualList.Sample.Database;
using CiccioSoft.VirtualList.Sample.Domain;
using CiccioSoft.VirtualList.Wpf;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CiccioSoft.VirtualList.Sample.Wpf.Collection;

public class FakeCollection : IVirtualCollection<Model>
{
    private readonly ILogger logger;
    private readonly Dispatcher dispatcher = System.Windows.Application.Current.Dispatcher;
    private readonly List<Model> list;
    //private readonly List<Model> fakelist;
    private List<Model> items;
    //private int count = 0;
    private string _searchString = string.Empty;
    private const string CountString = "Count";
    private const string IndexerName = "Item[]";

    public FakeCollection()
    {
        logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger<FakeCollection>();
        list = SampleDataService.ReadFromFile("SampleData.json");
        //count = list.Count;
        //fakelist = [];
        items = new List<Model>(list);
    }

    public async Task LoadAsync(string? searchString)
    {
        searchString ??= string.Empty;
        _searchString = searchString;

        items = list!.FindAll(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase));

        //count = items.Count;
        await dispatcher.InvokeAsync(() =>
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CountString));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        });
        logger.LogWarning("Evento Collection Reset 2");
    }


    #region interface member Implemented

    public Model this[int index]
    {
        get
        {
            var item = items![index];
            logger.LogWarning("Index: {Index}", index);
            return item;
        }
        set => throw new NotImplementedException();
    }

    object? IList.this[int index]
    {
        get => this[index];
        set => throw new NotImplementedException();
    }

    public int Count => items.Count;

    public bool IsReadOnly => true;

    public bool IsFixedSize => false;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public event PropertyChangedEventHandler? PropertyChanged;

    IEnumerator<Model> IEnumerable<Model>.GetEnumerator() => items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IList)items).GetEnumerator();

    int IList<Model>.IndexOf(Model item) => items.IndexOf(item);

    int IList.IndexOf(object? value) => ((IList)items).IndexOf(value);

    bool ICollection<Model>.Contains(Model item) => items.Contains(item);

    bool IList.Contains(object? value) => ((IList)items).Contains(value);

    #endregion


    #region interface member not implemented

    bool ICollection.IsSynchronized => throw new NotImplementedException();
    object ICollection.SyncRoot => throw new NotImplementedException();
    void ICollection<Model>.Add(Model item) => throw new NotImplementedException();
    int IList.Add(object? value) => throw new NotImplementedException();
    void ICollection<Model>.Clear() => throw new NotImplementedException();
    void IList.Clear() => throw new NotImplementedException();
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
