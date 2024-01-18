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
    private readonly ConcurrentStack<int> indexStack;
    private readonly Timer timer;
    private readonly IDictionary<int, T> items;
    private readonly List<T> fakelist;
    private readonly T dummyObject;
    private int count = 0;
    private readonly int _range;
    private readonly int take;
    private int index_to_fetch = 0;
    private const string CountString = "Count";
    private const string IndexerName = "Item[]";
    private int selectedIndex = -1;

    public VirtualCollection(int range = 20, ILogger? logger = null)
    {
        _logger = logger;
        dispatcher = System.Windows.Application.Current.Dispatcher;
        cancellationTokenSource = new CancellationTokenSource();
        indexStack = new ConcurrentStack<int>();
        timer = new Timer(TimerHandler, null, 50, 50);
        items = new ConcurrentDictionary<int, T>();
        fakelist = new List<T>();
        dummyObject = CreateDummyEntity();
        _range = range;
        take = range * 2;
    }

    public int SelectedIndex
    {
        get => selectedIndex;
        set
        {
            if (selectedIndex != value)
            {
                selectedIndex = value;
                dispatcher.Invoke(() =>
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndex))));
            }
        }
    }

    protected async Task LoadAsync()
    {
        SelectedIndex = -1;
        index_to_fetch = 0;
        count = await GetCountAsync();
        _logger?.LogDebug("FetchData: {Skip} - {Take}", 0, take - 1);
        var models = await GetRangeAsync(0, take, NewToken());
        items.Clear();
        for (var i = 0; i < models.Count; i++)
        {
            items.Add(i, models[i]);
        }
        await dispatcher.InvokeAsync(() =>
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CountString));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        });
    }


    #region abstract method

    public abstract Task LoadAsync(string? searchString);
    protected abstract T CreateDummyEntity();
    protected abstract Task<int> GetCountAsync();
    protected abstract Task<List<T>> GetRangeAsync(int skip, int take, CancellationToken token);

    #endregion


    #region private method

    private void TimerHandler(object? state)
    {
        if (!indexStack.IsEmpty)
        {
            while (!indexStack.IsEmpty)
            {
                indexStack.TryPop(out var index);

                // trick per datagrid
                if (index != 0)
                {
                    indexStack.Clear();

                    //_logger?.LogDebug("timer: {Time} {Millisecond} index: {Index}",
                    //                  DateTime.Now.ToLongTimeString(),
                    //                  DateTime.Now.Millisecond.ToString(),
                    //                  index.ToString());

                    if (index < index_to_fetch || index >= index_to_fetch + take)
                    {
                        _logger?.LogDebug("Indice non Fetchato: {Index}", index);
                        if (index < _range)
                            index = 0;
                        else if (index > count - _range)
                            index = count - take;
                        else
                            index -= _range;
                        index_to_fetch = index;
                        //_logger?.LogDebug("timer: {Time} {Millisecond} skip: {Index}",
                        //                  DateTime.Now.ToLongTimeString(),
                        //                  DateTime.Now.Millisecond.ToString(),
                        //                  index.ToString());
                        var token = NewToken();
                        Task.Run(async () => await FetchRange(index, token), token);
                    }
                    else
                        _logger?.LogDebug("Indice già Fetchato: {Index}", index);
                }
            }
        }
    }

    private CancellationToken NewToken()
    {
        if (cancellationTokenSource.Token.CanBeCanceled)
            cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
        cancellationTokenSource = new CancellationTokenSource();
        return cancellationTokenSource.Token;
    }

    private async Task FetchRange(int skip, CancellationToken token)
    {
        try
        {
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            // ritardo inserito per velocizzare scrolling
            await Task.Delay(50, token);

            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            // recupero i dati
            _logger?.LogDebug("FetchRange: {Skip} - {Take}", skip, take + skip - 1);
            var models = await GetRangeAsync(skip, take, token);

            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            // Aggiorno lista interna
            items.Clear();
            for (var i = 0; i < models.Count; i++)
            {
                items.Add(skip + i, models[i]);
            }

            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            SelectedIndex = -1;
            await dispatcher.InvokeAsync(() =>
                CollectionChanged?.Invoke(
                    this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)));
        }
        catch (TaskCanceledException tcex)
        {
            _logger?.LogDebug("FetchRange: {Skip} - {Take} {Message} Id:{Id}", skip, skip + take - 1, tcex.Message, tcex.Task?.Id);
        }
        catch (OperationCanceledException ocex)
        {
            _logger?.LogDebug(ocex.Message);
        }
    }

    #endregion


    #region interface member implemented

    public T this[int index]
    {
        get
        {
            if (items.ContainsKey(index))
            {
                //_logger?.LogDebug("Indexer get real: {Index}", index);
                return items[index];
            }
            else
            {
                //_logger?.LogDebug("Indexer get dummy: {Index}", index);
                indexStack.Push(index);
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

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => fakelist.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IList)fakelist).GetEnumerator();

    int IList<T>.IndexOf(T item) => -1;

    int IList.IndexOf(object? value) => -1;

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
