using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using Windows.System.Threading;
using Windows.UI.Core;

namespace CiccioSoft.VirtualList.Uwp.Collection
{
    public abstract class VirtualList<T> : IList, IList<T>, INotifyCollectionChanged, INotifyPropertyChanged where T : class
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
        protected int count = 0;
        private int index_to_fetch;
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";

        public VirtualList(int range = 20)
        {
            logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger("VirtualList");
            dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher;
            indexStack = new ConcurrentStack<int>();
            timer = ThreadPoolTimer.CreatePeriodicTimer(TimerHandler, TimeSpan.FromMilliseconds(50));
            items = new ConcurrentDictionary<int, T>();
            fakelist = new List<T>();
            dummyObject = CreateDummyEntity();
            cancellationTokenSource = new CancellationTokenSource();
            this.range = range;
            take = range * 2;
            index_to_fetch = int.MaxValue;
            //Task.Run(async () => await LoadAsync());
        }

        public async Task LoadAsync()
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                count = await GetCountAsync();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CountString));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            });
        }

        #region abstract method

        protected abstract T CreateDummyEntity();
        protected abstract Task<int> GetCountAsync();
        protected abstract Task<List<T>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken);

        #endregion


        #region private method

        private CancellationToken NewToken()
        {
            if (cancellationTokenSource.Token.CanBeCanceled)
                cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            return cancellationTokenSource.Token;
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
                    Task.Run(async () => await FetchItem(index));
                    logger.LogWarning("Indice da Fetchare: {0}", index);
                }
                else
                    logger.LogWarning("Indice già Fetchato: {0}", index);
            }
        }

        private async Task FetchItem(int index)
        {
            try
            {
                await FetchRange(index, NewToken());
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
        public event PropertyChangedEventHandler PropertyChanged;

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
