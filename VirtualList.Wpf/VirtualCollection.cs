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
    private readonly Dispatcher dispatcher;
    private CancellationTokenSource cancellationTokenSource;
    private readonly IDictionary<int, T> items;
    private readonly T dummyObject;
    private int count = 0;
    private readonly int _range;
    private readonly int take;
    private int _indexToFetch = 0;
    private const string CountString = "Count";
    private const string IndexerName = "Item[]";
    private string? _searchString;

    public VirtualCollection(int range = 20, ILogger? logger = null)
    {
        _logger = logger;
        dispatcher = System.Windows.Application.Current.Dispatcher;
        cancellationTokenSource = new CancellationTokenSource();
        items = new ConcurrentDictionary<int, T>();
        dummyObject = CreateDummyEntity();
        _range = range;
        take = range * 2;
    }

    public async Task LoadAsync(string? searchString)
    {
        //_indexToFetch = 0;
        _searchString = searchString;
        count = await GetCountAsync(searchString);
        items.Clear();

        ScrollToTop?.Invoke();

        try
        {
            var token = NewToken();
            await FetchRange(0, token);
        }
        catch (TaskCanceledException tcex)
        {
            _logger?.LogDebug("FetchRange: {Skip} - {Take} {Message} Id:{Id}", 0, take - 1, tcex.Message, tcex.Task?.Id);
        }
        catch (OperationCanceledException ocex)
        {
            _logger?.LogDebug(ocex.Message);
        }
    }


    #region abstract method

    protected abstract T CreateDummyEntity();
    protected abstract Task<int> GetCountAsync(string? searchString);
    protected abstract Task<List<T>> GetRangeAsync(string? searchString, int skip, int take, CancellationToken token);

    #endregion


    #region private method

    private void GetData(int index)
    {
        // trick per datagrid
        if (index != 0)
        {
            //_logger?.LogDebug("timer: {Time} {Millisecond} index: {Index}",
            //                  DateTime.Now.ToLongTimeString(),
            //                  DateTime.Now.Millisecond.ToString(),
            //                  index.ToString());

            if (index < _indexToFetch || index >= _indexToFetch + take)
            {
                _logger?.LogDebug("Indice non Fetchato: {Index}", index);
                if (index < _range)
                    index = 0;
                else if (index > count - _range)
                    index = count - take;
                else
                    index -= _range;
                _indexToFetch = index;
                //_logger?.LogDebug("timer: {Time} {Millisecond} skip: {Index}",
                //                  DateTime.Now.ToLongTimeString(),
                //                  DateTime.Now.Millisecond.ToString(),
                //                  index.ToString());

                UnSelectIndex?.Invoke();

                try
                {
                    var token = NewToken();
                    Task.Run(async () => await FetchRange(index, token), token);
                }
                catch (TaskCanceledException tcex)
                {
                    _logger?.LogDebug("FetchRange: {Skip} - {Take} {Message} Id:{Id}", index, index + take - 1, tcex.Message, tcex.Task?.Id);
                }
                catch (OperationCanceledException ocex)
                {
                    _logger?.LogDebug(ocex.Message);
                }
            }
            else
                _logger?.LogDebug("Indice già Fetchato: {Index}", index);
        }
    }

    private async Task FetchRange(int index, CancellationToken token)
    {
        // pulisco la lista interna
        items.Clear();
        _logger?.LogDebug("clean list");

        if (token.IsCancellationRequested)
            token.ThrowIfCancellationRequested();

        // ritardo inserito per velocizzare scrolling
        await Task.Delay(50, token);

        if (token.IsCancellationRequested)
            token.ThrowIfCancellationRequested();

        // recupero i dati
        _logger?.LogDebug("FetchRange: {Skip} - {Take}", index, take + index - 1);
        var models = await GetRangeAsync(_searchString, index, take, token);

        if (token.IsCancellationRequested)
            token.ThrowIfCancellationRequested();

        // Aggiorno lista interna
        for (var i = 0; i < models.Count; i++)
        {
            items.Add(index + i, models[i]);
        }

        if (token.IsCancellationRequested)
            token.ThrowIfCancellationRequested();

        await dispatcher.InvokeAsync(() =>
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CountString));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
            CollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        });
    }

    private CancellationToken NewToken()
    {
        if (cancellationTokenSource.Token.CanBeCanceled)
            cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        cancellationTokenSource = new CancellationTokenSource();
        return cancellationTokenSource.Token;
    }

    #endregion


    #region interface member implemented

    public T this[int index]
    {
        get
        {
            if (items.TryGetValue(index, out T? item))
            {
                _logger?.LogDebug("Indexer get real: {Index}", index);
                return item;
            }
            else
            {
                _logger?.LogDebug("Indexer get dummy: {Index}", index);
                GetData(index);
                return dummyObject;
            }
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

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => items.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)items.Values).GetEnumerator();

    int IList.IndexOf(object? value) => ((IList)items.Values).IndexOf(value);

    bool IList.Contains(object? value) => ((IList)items.Values).Contains(value);

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
