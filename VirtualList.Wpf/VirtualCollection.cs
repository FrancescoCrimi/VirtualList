using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CiccioSoft.VirtualList.Wpf;

public abstract class VirtualCollection<T> : IVirtualCollection<T> where T : class
{
    private readonly ILogger? _logger;
    private readonly Dispatcher _dispatcher;
    private readonly IDictionary<int, T> _items;
    private readonly T _dummy;
    private readonly int _range;
    private readonly int _take;
    private CancellationTokenSource _tokenSource;
    private int _count = 0;
    private int _indexToFetch = 0;
    private string? _searchString;
    private const string CountString = "Count";
    private const string IndexerName = "Item[]";

    public VirtualCollection(int range = 20, ILogger? logger = null)
    {
        _logger = logger;
        _dispatcher = System.Windows.Application.Current.Dispatcher;
        _items = new ConcurrentDictionary<int, T>();
        _dummy = CreateDummyEntity();
        _range = range;
        _take = range * 2;
        _tokenSource = new CancellationTokenSource();
    }

    public async Task LoadAsync(string? searchString)
    {
        _searchString = searchString;
        _count = await GetCountAsync(searchString);
        ScrollToTop?.Invoke();
        _items.Clear();
        _indexToFetch = -1;
        await _dispatcher.InvokeAsync(() =>
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CountString));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        });
    }


    #region abstract method

    protected abstract T CreateDummyEntity();
    protected abstract Task<int> GetCountAsync(string? searchString);
    protected abstract Task<List<T>> GetRangeAsync(string? searchString,
                                                   int skip,
                                                   int take,
                                                   CancellationToken token);

    #endregion


    #region private method

    private void GetData(int index)
    {
        // trick per datagrid
        if (index != 0)
        {
            if (index < _indexToFetch || index >= _indexToFetch + _take || _indexToFetch == -1)
            {
                _logger?.LogDebug("Indice non Fetchato: {Index}", index);

                if (index < _range)
                    index = 0;
                else if (index > _count - _range)
                    index = _count - _take;
                else
                    index -= _range;
                _indexToFetch = index;

                UnSelectIndex?.Invoke();

                try
                {
                    var token = NewToken();
                    Task.Run(async () =>
                    {
                        // pulisco la lista interna
                        _items.Clear();

                        if (token.IsCancellationRequested)
                            token.ThrowIfCancellationRequested();

                        // ritardo inserito per velocizzare scrolling
                        await Task.Delay(50, token);

                        if (token.IsCancellationRequested)
                            token.ThrowIfCancellationRequested();

                        // recupero i dati
                        _logger?.LogDebug("FetchRange: {Skip} - {Take}", index, _take + index - 1);
                        var models = await GetRangeAsync(_searchString, index, _take, token);

                        if (token.IsCancellationRequested)
                            token.ThrowIfCancellationRequested();

                        // Aggiorno lista interna
                        for (var i = 0; i < models.Count; i++)
                        {
                            _items.Add(index + i, models[i]);
                        }

                        if (token.IsCancellationRequested)
                            token.ThrowIfCancellationRequested();

                        await _dispatcher.InvokeAsync(() =>
                        {
                            foreach (var item in _items)
                            {
                                if (token.IsCancellationRequested)
                                    token.ThrowIfCancellationRequested();
                                var eventArgs = new NotifyCollectionChangedEventArgs(
                                    NotifyCollectionChangedAction.Replace,
                                    item.Value,
                                    _dummy,
                                    item.Key);
                                CollectionChanged?.Invoke(this, eventArgs);
                            }
                        });
                    }, token);
                }
                catch (TaskCanceledException tcex)
                {
                    _logger?.LogDebug("GetData: {Index} - {Message} Id:{Id}", index, tcex.Message, tcex.Task?.Id);
                }
                catch (OperationCanceledException ocex)
                {
                    _logger?.LogDebug("GetData: {Index} - {Message}", index, ocex.Message);
                }
            }
            else
                _logger?.LogDebug("Indice già Fetchato: {Index}", index);
        }
    }

    private CancellationToken NewToken()
    {
        if (_tokenSource.Token.CanBeCanceled)
            _tokenSource.Cancel();
        _tokenSource.Dispose();
        _tokenSource = new CancellationTokenSource();
        return _tokenSource.Token;
    }

    #endregion


    #region interface member implemented

    public T this[int index]
    {
        get
        {
            if (_items.TryGetValue(index, out T? item))
            {
                //_logger?.LogDebug("Indexer get real: {Index}", index);
                return item;
            }
            else
            {
                //_logger?.LogDebug("Indexer get dummy: {Index}", index);
                GetData(index);
                return _dummy;
            }
        }
        set => throw new NotImplementedException();
    }

    object? IList.this[int index]
    {
        get => this[index];
        set => throw new NotImplementedException();
    }

    public int Count => _count;

    public bool IsReadOnly => true;

    public bool IsFixedSize => false;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public event PropertyChangedEventHandler? PropertyChanged;

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => _items.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_items.Values).GetEnumerator();

    int IList.IndexOf(object? value) => ((IList)_items.Values).IndexOf(value);

    bool IList.Contains(object? value) => ((IList)_items.Values).Contains(value);

    public Action? ScrollToTop { private get; set; }

    public Action? UnSelectIndex { private get; set; }

    #endregion


    #region interface member not implemented

    void ICollection<T>.Add(T item) => throw new NotImplementedException();
    int IList.Add(object? value) => throw new NotImplementedException();
    void ICollection<T>.Clear() => throw new NotImplementedException();
    void IList.Clear() => throw new NotImplementedException();
    bool ICollection<T>.Contains(T item) => throw new NotImplementedException();
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
    void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();
    int IList<T>.IndexOf(T item) => throw new NotImplementedException();
    void IList<T>.Insert(int index, T item) => throw new NotImplementedException();
    void IList.Insert(int index, object? value) => throw new NotImplementedException();
    bool ICollection<T>.Remove(T item) => throw new NotImplementedException();
    void IList.Remove(object? value) => throw new NotImplementedException();
    void IList<T>.RemoveAt(int index) => throw new NotImplementedException();
    void IList.RemoveAt(int index) => throw new NotImplementedException();
    bool ICollection.IsSynchronized => throw new NotImplementedException();
    object ICollection.SyncRoot => throw new NotImplementedException();

    #endregion
}
