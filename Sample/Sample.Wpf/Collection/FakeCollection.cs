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
    private readonly List<Model> fakelist;
    private List<Model> items;
    private int count = 0;
    private int selectedIndex = -1;
    private string _searchString = string.Empty;
    private const string CountString = "Count";
    private const string IndexerName = "Item[]";

    public FakeCollection()
    {
        logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger<FakeCollection>();
        list = SampleDataService.ReadFromFile("SampleData.json");
        fakelist = [];
        items = [];
    }

    public async Task LoadAsync(string? searchString)
    {
        searchString ??= string.Empty;
        _searchString = searchString;

        items = list!.FindAll(x => x.Name.Contains(_searchString.ToUpper()));

        SelectedIndex = -1;
        count = items.Count;
        await dispatcher.InvokeAsync(() =>
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CountString));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        });
        logger.LogWarning("Evento Collection Reset 2");
    }

    public int SelectedIndex
    {
        get => selectedIndex;
        set
        {
            if (selectedIndex != value)
            {
                selectedIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndex)));
            }
        }
    }


    #region interface member Implemented

    public Model this[int index]
    {
        get
        {
            var item = items?[index];
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
