using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;
using System.ComponentModel;
using Microsoft.UI.Dispatching;

namespace VirtualList.WinUi.Collection;

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
/// <typeparam name="T"></typeparam>
public abstract class VirtualCollection<T> : IList, IList<T>, INotifyCollectionChanged, INotifyPropertyChanged where T : class
{
    private readonly ILogger logger;
    private readonly DispatcherQueue dispatcherQueue;
    private CancellationTokenSource cancellationTokenSource;
    private readonly ConcurrentStack<int> indexStack;
    private readonly ThreadPoolTimer timer = null;
    private readonly IDictionary<int, T> items;
    private readonly List<T> fakelist;
    private readonly T dummyObject;
    private readonly int range;
    private readonly int take;
    private int count = 0;
    private int index_to_fetch;
    private const string CountString = "Count";
    private const string IndexerName = "Item[]";

    public VirtualCollection(int range = 20)
    {
        logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger("VirtualCollection");
        dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        indexStack = new ConcurrentStack<int>();
        timer = ThreadPoolTimer.CreatePeriodicTimer(TimerHandler, TimeSpan.FromMilliseconds(50));
        items = new ConcurrentDictionary<int, T>();
        fakelist = new List<T>();
        dummyObject = CreateDummyEntity();
        cancellationTokenSource = new CancellationTokenSource();
        this.range = range;
        take = range * 2;
        index_to_fetch = int.MaxValue;
    }

    public async Task LoadAsync()
    {
        await Task.Run(() =>
        {
            dispatcherQueue.TryEnqueue(async () =>
            {
                count = await GetCountAsync();
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

    private async Task FetchRange(int skip, CancellationToken token)
    {
        try
        {
            // Aggiungo ritardo solo per test
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();
            await Task.Delay(60, token);

            // recupero i dati
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();
            List<T> models = await GetRangeAsync(skip, take, token);

            // Aggiorno lista interna
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();
            items.Clear();
            for (int i = 0; i < models.Count; i++)
            {
                items.Add(skip + i, models[i]);
            }

            // invoco CollectionChanged Replace per singolo item
            if (token.IsCancellationRequested)
                token.ThrowIfCancellationRequested();
            dispatcherQueue.TryEnqueue(() =>
            {
                foreach (var item in items)
                {
                    var eventArgs = new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Replace,
                        item.Value,
                        null,
                        item.Key);
                    CollectionChanged?.Invoke(this, eventArgs);
                    //logger.LogWarning("CollectionChanged Replace: {0}", item.Key);
                }
            });
        }
        catch (TaskCanceledException tcex)
        {
            logger.LogError("{0} Id:{1}", tcex.Message, tcex.Task.Id);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
        }
    }

    private void TimerHandler(ThreadPoolTimer timer)
    {
        if (!indexStack.IsEmpty)
        {
            indexStack.TryPop(out int index);
            indexStack.Clear();
            if (index < index_to_fetch || index >= index_to_fetch + take)
            {
                logger.LogWarning("Indice non Fetchato: {0}", index);
                if (index < range)
                    index = 0;
                else if (index > count - range)
                    index = count - take;
                else
                    index -= range;
                index_to_fetch = index;
                Task.Run(async () => await FetchRange(index, NewToken()));
                logger.LogWarning("Indice da Fetchare: {0}", index);
            }
            else
                logger.LogWarning("Indice già Fetchato: {0}", index);
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

    #endregion


    #region interface member implemented 

    public event NotifyCollectionChangedEventHandler CollectionChanged;
    public event PropertyChangedEventHandler PropertyChanged;

    public T this[int index]
    {
        get
        {
            if (items.ContainsKey(index))
            {
                //logger.LogWarning("Indexer get real: {0}", index);
                return items[index];
            }
            else
            {
                //logger.LogWarning("Indexer get dummy: {0}", index);
                indexStack.Push(index);
                return dummyObject;
            }
        }
        set => throw new NotImplementedException();
    }

    object IList.this[int index]
    {
        get => this[index];
        set => throw new NotImplementedException();
    }

    public int Count => count;

    public IEnumerator<T> GetEnumerator() => fakelist.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IList)fakelist).GetEnumerator();

    public int IndexOf(T item) => -1;

    int IList.IndexOf(object value) => -1;

    public bool IsReadOnly => true;

    public bool IsFixedSize => false;

    #endregion


    #region interface member not implemented

    void ICollection<T>.Add(T item) => throw new NotImplementedException();

    int IList.Add(object value) => throw new NotImplementedException();

    public void Clear() => throw new NotImplementedException();

    bool ICollection<T>.Contains(T item) => throw new NotImplementedException();

    bool IList.Contains(object value) => throw new NotImplementedException();

    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

    void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();

    void IList<T>.Insert(int index, T item) => throw new NotImplementedException();

    void IList.Insert(int index, object value) => throw new NotImplementedException();

    bool ICollection.IsSynchronized => throw new NotImplementedException();

    bool ICollection<T>.Remove(T item) => throw new NotImplementedException();

    void IList.Remove(object value) => throw new NotImplementedException();

    public void RemoveAt(int index) => throw new NotImplementedException();

    object ICollection.SyncRoot => throw new NotImplementedException();

    #endregion
}
