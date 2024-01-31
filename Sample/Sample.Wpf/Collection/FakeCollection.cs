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
    private readonly Dispatcher dispatcher;
    private readonly List<Model> list;
    private List<Model> items;
    private const string CountString = "Count";
    private const string IndexerName = "Item[]";

    public FakeCollection()
    {
        logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger<FakeCollection>();
        dispatcher = System.Windows.Application.Current.Dispatcher;
        list = SampleDataService.ReadFromFile("SampleData.json");
        items = new List<Model>();
    }

    public async Task LoadAsync(string? searchString)
    {
        searchString ??= string.Empty;

        await dispatcher.InvokeAsync(() =>
        {
            ScrollToTop?.Invoke();
            items = list!.FindAll(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CountString));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            logger.LogWarning("Evento Collection Reset 2");
        });
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

    public int Count
    {
        get
        {
            logger.LogWarning("Count: {Count} {Time}", items.Count, DateTime.Now.ToString("hh:mm:ss.fff"));
            return items.Count;
        }
    }

    public bool IsReadOnly => true;

    public bool IsFixedSize => false;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public event PropertyChangedEventHandler? PropertyChanged;

    IEnumerator<Model> IEnumerable<Model>.GetEnumerator() => items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IList)items).GetEnumerator();

    int IList.IndexOf(object? value) => ((IList)items).IndexOf(value);

    bool IList.Contains(object? value) => ((IList)items).Contains(value);

    public Action? ScrollToTop { private get; set; }

    public Action? UnSelectIndex { private get; set; }

    #endregion


    #region interface member not implemented

    void ICollection<Model>.Add(Model item) => throw new NotImplementedException();
    int IList.Add(object? value) => throw new NotImplementedException();
    void ICollection<Model>.Clear() => throw new NotImplementedException();
    void IList.Clear() => throw new NotImplementedException();
    bool ICollection<Model>.Contains(Model item) => throw new NotImplementedException();
    void ICollection<Model>.CopyTo(Model[] array, int arrayIndex) => throw new NotImplementedException();
    void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();
    int IList<Model>.IndexOf(Model item) => throw new NotImplementedException();
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
