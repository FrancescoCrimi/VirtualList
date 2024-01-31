using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;

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
/// </summary>
public abstract class VirtualCollection<T> : IVirtualCollection<T> where T : class
{
    private readonly ILogger? _logger;
    private readonly DispatcherQueue _dispatcher;
    private readonly IDictionary<int, T> _items;
    private readonly List<T> fakelist;
    private readonly T _dummy;
    private readonly int _range;
    private readonly int _take;
    private CancellationTokenSource _tokenSource;
    private int _count = 0;
    private int _indexToFetch = 0;
    private string? _searchString = "";
    private const string CountString = "Count";
    private const string IndexerName = "Item[]";

    private readonly ConcurrentStack<int> indexStack;
    private readonly ThreadPoolTimer timer;

    public VirtualCollection(int range = 20, ILogger? logger = null)
    {
        _logger = logger;
        _dispatcher = DispatcherQueue.GetForCurrentThread();
        _items = new ConcurrentDictionary<int, T>();
        fakelist = new List<T>();
        _dummy = CreateDummyEntity();
        _range = range;
        _take = range * 2;
        _tokenSource = new CancellationTokenSource();
        _indexToFetch = int.MaxValue;

        indexStack = new ConcurrentStack<int>();
        timer = ThreadPoolTimer.CreatePeriodicTimer(TimerHandler, TimeSpan.FromMilliseconds(50));
    }

    public async Task LoadAsync(string? searchString)
    {
        await Task.Run(() =>
        {
            _dispatcher.TryEnqueue(async () =>
            {
                _count = await GetCountAsync();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CountString));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            });
        });
    }


    #region abstract method

    protected abstract T CreateDummyEntity();
    protected abstract Task<int> GetCountAsync();
    protected abstract Task<List<T>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken);

    #endregion


    #region private method

    private void TimerHandler(ThreadPoolTimer timer)
    {
        if (!indexStack.IsEmpty)
        {
            indexStack.TryPop(out var index);
            indexStack.Clear();
            if (index < _indexToFetch || index >= _indexToFetch + _take)
            {
                _logger?.LogDebug("Indice non Fetchato: {Index}", index);
                if (index < _range)
                    index = 0;
                else if (index > _count - _range)
                    index = _count - _take;
                else
                    index -= _range;
                _indexToFetch = index;
                var token = NewToken();
                Task.Run(async () => await FetchRange(index, token), token);
            }
            else
                _logger?.LogDebug("Indice già Fetchato: {Index}", index);
        }
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
            _logger?.LogDebug("FetchRange: {Skip} - {Take}", skip, skip + _take - 1);
            var models = await GetRangeAsync(skip, _take, token);

            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            // Aggiorno lista interna
            _items.Clear();
            for (var i = 0; i < models.Count; i++)
            {
                _items.Add(skip + i, models[i]);
            }

            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();

            // invoco CollectionChanged Replace per singolo item
            _dispatcher.TryEnqueue(() =>
            {
                try
                {
                    foreach (var item in _items)
                    {
                        if (token.IsCancellationRequested)
                            token.ThrowIfCancellationRequested();
                        var eventArgs = new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Replace,
                            item.Value,
                            null,
                            item.Key);
                        CollectionChanged?.Invoke(this, eventArgs);
                    }
                }
                catch (OperationCanceledException ocex)
                {
                    _logger?.LogDebug("NotifyCollectionChanged Replace: {Skip} - {Take} {Message}", skip, skip + _take - 1, ocex.Message);
                }
            });
        }
        catch (TaskCanceledException tcex)
        {
            _logger?.LogDebug("FetchRange: {Skip} - {Take} {Message} Id:{Id}", skip, skip + _take - 1, tcex.Message, tcex.Task?.Id);
        }
        catch (OperationCanceledException ocex)
        {
            _logger?.LogDebug(ocex.Message);
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
            if (_items.ContainsKey(index))
            {
                //logger.LogDebug("Indexer get real: {Index}", index);
                return _items[index];
            }
            else
            {
                //logger.LogDebug("Indexer get dummy: {Index}", index);
                indexStack.Push(index);
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

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => fakelist.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IList)fakelist).GetEnumerator();

    int IList<T>.IndexOf(T item) => -1;

    int IList.IndexOf(object? value) => -1;

    #endregion


    #region interface member not implemented

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
    bool ICollection.IsSynchronized => throw new NotImplementedException();
    object ICollection.SyncRoot => throw new NotImplementedException();

    #endregion
}
