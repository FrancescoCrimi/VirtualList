using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Core;

namespace CiccioSoft.VirtualList.Uwp
{
    /// <summary>
    /// Collezione Virtuale
    /// 
    /// per funzionare correttamente impostare la Proprietà CacheLength dell'ItemStackPanel a 0.0 cosi
    ///
    /// <ListView.ItemsPanel>
    ///       <ItemsPanelTemplate>
    ///         <ItemsStackPanel Orientation = "Vertical" CacheLength="0.0"/>
    ///       </ItemsPanelTemplate>
    ///     </ListView.ItemsPanel>
    ///     
    /// Per usare la classe subclassa questa classe implementando i metodi astratti
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class VirtualCollection<T> : IList, IList<T>, INotifyCollectionChanged where T : class
    {
        private readonly ILogger logger;
        private readonly CoreDispatcher dispatcher;
        private readonly ConcurrentStack<int> indexStack;
        private readonly ThreadPoolTimer timer = null;
        private readonly IDictionary<int, T> items;
        private readonly List<T> fakelist;
        private CancellationTokenSource cancellationTokenSource;
        private readonly int range;
        private readonly int take;
        protected int count;
        private int index_to_fetch;

        public VirtualCollection(int range = 20)
        {
            logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger("VirtualCollection");
            dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher;
            indexStack = new ConcurrentStack<int>();
            timer = ThreadPoolTimer.CreatePeriodicTimer(TimerHandler, TimeSpan.FromMilliseconds(50));
            items = new ConcurrentDictionary<int, T>();
            fakelist = new List<T>();
            cancellationTokenSource = new CancellationTokenSource();
            this.range = range;
            take = range * 2;
            index_to_fetch = int.MaxValue;
        }

        #region abstract method

        protected abstract T CreateDummyEntity();
        protected abstract int GetCount();
        protected abstract Task<List<T>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken);

        #endregion


        #region private method

        private void TimerHandler(ThreadPoolTimer timer)
        {
            if (!indexStack.IsEmpty)
            {
                indexStack.TryPop(out int index);
                indexStack.Clear();
                if (index < index_to_fetch || index >= index_to_fetch + take)
                {
                    if (index < range)
                        index = 0;
                    else if (index > count - range)
                        index = count - take;
                    else
                        index -= range;
                    index_to_fetch = index;
                    Task.Run(async () => await FetchItem(index));
                }
                else
                    logger.LogWarning("Indice già fetchato");
            }
        }

        private async Task FetchItem(int index)
        {
            if (cancellationTokenSource.Token.CanBeCanceled)
                cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            try
            {
                await FetchRange(index, cancellationTokenSource.Token);
            }
            catch (TaskCanceledException ex)
            {
                logger.LogError("Cancel Task Id:{0}", ex.Task.Id);
            }
        }

        private async Task FetchRange(int skip, CancellationToken cancellationToken)
        {
            // Aggiungo ritardo solo per test
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(60, cancellationToken);

            // recupero i dati
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();
            List<T> models = await GetRangeAsync(skip, take, cancellationToken);

            // Aggiorno lista interna
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();
            items.Clear();
            for (int i = 0; i < models.Count; i++)
            {
                items.Add(skip + i, models[i]);
            }

            // invoco CollectionChanged Replace per singolo item
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (var item in items)
                {
                    //if (cancellationToken.IsCancellationRequested)
                    //    cancellationToken.ThrowIfCancellationRequested();
                    var eventarg = new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Replace,
                        item.Value,
                        null,
                        item.Key);
                    CollectionChanged?.Invoke(this, eventarg);
                }
            });
        }

        #endregion


        #region interface member implemented 

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public T this[int index]
        {
            get
            {
                //logger.LogWarning("Index: {0}", index);
                if (items.ContainsKey(index))
                    return items[index];
                else
                {
                    indexStack.Push(index);
                    return CreateDummyEntity();
                }
            }
            set => throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get => this[index];
            set => throw new NotImplementedException();
        }

        public int Count
        {
            get
            {
                //logger.LogWarning("Count: " + count.ToString());
                return count;
            }

            private set => throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator() => fakelist.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IList)fakelist).GetEnumerator();

        public int IndexOf(T item) => -1;

        int IList.IndexOf(object value) => -1;

        public bool IsReadOnly => true;

        public bool IsFixedSize => false;

        #endregion


        #region interface member not implemented

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

        int IList.Add(object value) => throw new NotImplementedException();

        public void Clear() => throw new NotImplementedException();

        bool IList.Contains(object value) => throw new NotImplementedException();

        void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();

        void IList.Insert(int index, object value) => throw new NotImplementedException();

        void IList.Remove(object value) => throw new NotImplementedException();

        public void RemoveAt(int index) => throw new NotImplementedException();


        void IList<T>.Insert(int index, T item) => throw new NotImplementedException();

        void ICollection<T>.Add(T item) => throw new NotImplementedException();

        bool ICollection<T>.Contains(T item) => throw new NotImplementedException();

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

        bool ICollection<T>.Remove(T item) => throw new NotImplementedException();

        #endregion
    }
}
