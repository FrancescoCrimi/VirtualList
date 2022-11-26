using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using Windows.System.Threading;
using Windows.UI.Core;

namespace CiccioSoft.VirtualList.Uwp
{
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
        private readonly ILogger logger;
        private readonly CoreDispatcher dispatcher;
        private CancellationTokenSource cancellationTokenSource;
        private readonly ConcurrentStack<int> indexStack;
        private readonly ThreadPoolTimer timer = null;
        private readonly IDictionary<int, T> items;
        private readonly List<T> fakelist;
        private readonly T dummyObject;
        private readonly int range;
        private readonly int take;
        private int index_to_fetch = 0;
        private int count = 0;
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";

        public VirtualCollection(int range = 20)
        {
            logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger("VirtualCollection");
            dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher;
            cancellationTokenSource = new CancellationTokenSource();
            indexStack = new ConcurrentStack<int>();
            timer = ThreadPoolTimer.CreatePeriodicTimer(TimerHandler, TimeSpan.FromMilliseconds(50));
            items = new ConcurrentDictionary<int, T>();
            fakelist = new List<T>();
            dummyObject = CreateDummyEntity();
            this.range = range;
            take = range * 2;
        }

        protected async Task LoadAsync()
        {
            index_to_fetch = 0;
            count = await GetCountAsync();
            logger.LogInformation("FetchData: {0} - {1}", 0, take - 1);
            var models = await GetRangeAsync(0, take, NewToken());
            items.Clear();
            for (var i = 0; i < models.Count; i++)
            {
                items.Add(i, models[i]);
            }
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CountString));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            });
        }


        #region abstract method

        public abstract Task LoadAsync(string searchString = "");
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
                if (index < index_to_fetch || index >= index_to_fetch + take)
                {
                    logger.LogInformation("Indice non Fetchato: {0}", index);
                    if (index < range)
                        index = 0;
                    else if (index > count - range)
                        index = count - take;
                    else
                        index -= range;
                    index_to_fetch = index;
                    var token = NewToken();
                    Task.Run(async () => await FetchRange(index, token), token);
                }
                else
                    logger.LogInformation("Indice già Fetchato: {0}", index);
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
                logger.LogInformation("FetchRange: {0} - {1}", skip, skip + take - 1);
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

                // invoco CollectionChanged Replace per singolo item
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        foreach (var item in items)
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
                        logger.LogInformation("NotifyCollectionChanged Replace: {0} - {1} {2}", skip, skip + take - 1, ocex.Message);
                    }
                });
            }
            catch (TaskCanceledException tcex)
            {
                logger.LogInformation("FetchRange: {0} - {1} {2} Id:{3}", skip, skip + take - 1, tcex.Message, tcex.Task.Id);
            }
            catch (OperationCanceledException ocex)
            {
                logger.LogInformation(ocex.Message);
            }
        }

        #endregion


        #region interface member Implemented 

        public T this[int index]
        {
            get
            {
                if (items.ContainsKey(index))
                {
                    //logger.LogInformation("Indexer get real: {0}", index);
                    return items[index];
                }
                else
                {
                    //logger.LogInformation("Indexer get dummy: {0}", index);
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

        public bool IsReadOnly => true;

        public bool IsFixedSize => false;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => fakelist.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IList)fakelist).GetEnumerator();
        int IList<T>.IndexOf(T item) => -1;
        int IList.IndexOf(object value) => -1;

        #endregion


        #region interface member not implemented

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

        void ICollection<T>.Add(T item) => throw new NotImplementedException();
        int IList.Add(object value) => throw new NotImplementedException();
        void ICollection<T>.Clear() => throw new NotImplementedException();
        void IList.Clear() => throw new NotImplementedException();
        bool ICollection<T>.Contains(T item) => throw new NotImplementedException();
        bool IList.Contains(object value) => throw new NotImplementedException();
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
        void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();
        void IList<T>.Insert(int index, T item) => throw new NotImplementedException();
        void IList.Insert(int index, object value) => throw new NotImplementedException();
        bool ICollection<T>.Remove(T item) => throw new NotImplementedException();
        void IList.Remove(object value) => throw new NotImplementedException();
        void IList<T>.RemoveAt(int index) => throw new NotImplementedException();
        void IList.RemoveAt(int index) => throw new NotImplementedException();

        #endregion
    }
}
