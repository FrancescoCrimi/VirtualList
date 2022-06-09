using CiccioSoft.VirtualList.Data;
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
    public class ModelVirtualCollection : VirtualCollection<Model>
    {
        private readonly IModelRepository repo;

        public ModelVirtualCollection() : base()
        {
            //repo = Ioc.Default.GetRequiredService<IModelRepository>();
            repo = new FakeModelRepository(10000);
            count = GetCount();
        }

        protected override Model CreateDummyEntity()
        {
            return new Model(0, "null");
        }

        protected override int GetCount()
        {
            return repo.Count();
        }

        protected override Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            return repo.GetRangeModelsAsync(skip, take, cancellationToken);
        }
    }

    /// <summary>
    /// Collezione Virtuale
    /// per funzionare correttamente impostare la Proprietà CacheLength dell' ItemStackPanel a 0.0
    ///     <ListView.ItemsPanel>
    ///       <ItemsPanelTemplate>
    ///         <ItemsStackPanel Orientation = "Vertical" CacheLength="0.0"/>
    ///       </ItemsPanelTemplate>
    ///     </ListView.ItemsPanel>
    /// Per usare la classe subclassa questa classe implementando i metodi astratti
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class VirtualCollection<T> : IList, IList<T>, INotifyCollectionChanged where T : class
    {
        private readonly ILogger logger;
        private readonly CoreDispatcher dispatcher;
        private readonly int range;
        private readonly int take;
        private readonly IDictionary<int, T> items;
        private readonly List<T> fakelist;
        protected int count;
        private CancellationTokenSource cancellationTokenSource;
        private ConcurrentStack<int> indexStack;
        private readonly ThreadPoolTimer timer = null;

        public VirtualCollection(int range = 20)
        {
            logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger("UwpVirtualList");
            dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher;
            this.range = range;
            take = range * 2;
            items = new ConcurrentDictionary<int, T>();
            fakelist = new List<T>();
            cancellationTokenSource = new CancellationTokenSource();
            indexStack = new ConcurrentStack<int>();
            timer = ThreadPoolTimer.CreatePeriodicTimer(TimerHandler, TimeSpan.FromMilliseconds(500));
        }

        #region abstract method

        protected abstract T CreateDummyEntity();
        protected abstract int GetCount();
        protected abstract Task<List<T>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken);

        #endregion


        #region private region

        private void TimerHandler(ThreadPoolTimer timer)
        {
            if (indexStack.Count > 0)
            {
                indexStack.TryPop(out int idx);
                indexStack.Clear();
                FetchItem(idx);
                //logger.LogWarning("TimerHandler: {0}", idx);
            }
        }

        private void FetchItem(int index)
        {
            if (index < range)
                index = 0;
            else if (index > count - range)
                index = count - take;
            else
                index -= range;

            if (cancellationTokenSource.Token.CanBeCanceled)
                cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                Task task = Task.Run(async () =>
                    await FetchRange(index, cancellationTokenSource.Token));
                logger.LogWarning("Create Task Id:{0} - Index:{1}", task.Id, index);
                task.Wait();
            }
            catch (OperationCanceledException ex)
            {
                logger.LogError("Cancel Task Id:{0}", ((TaskCanceledException)ex.InnerException).Task.Id);
            }
            catch (AggregateException agex)
            {
                logger.LogError("Cancel Task Id:{0}", ((TaskCanceledException)agex.InnerException).Task.Id);
            }
        }

        private async Task FetchRange(int index, CancellationToken cancellationToken)
        {
            //// Aggiungo ritardo solo per test
            //if (cancellationToken.IsCancellationRequested)
            //    cancellationToken.ThrowIfCancellationRequested();
            //await Task.Delay(2000, cancellationToken);
            //logger.LogWarning("FetchRange: {0} - {1}", index, index + take);

            // recupero i dati
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();
            List<T> models = await GetRangeAsync(index, take, cancellationToken);

            // Aggiorno lista interna
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();
            items.Clear();
            for (int i = 0; i < models.Count; i++)
            {
                items.Add(index + i, models[i]);
            }

            // invoco CollectionChanged Replace per singolo item
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();
            foreach (var item in items)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                var eventarg = new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Replace,
                    item.Value,
                    null,
                    item.Key);

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    CollectionChanged?.Invoke(this, eventarg));
            }
        }

        #endregion


        #region Interface Member Implemented 

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        object IList.this[int index]
        {
            get => this[index];
            set => throw new NotImplementedException();
        }

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

        public int Count
        {
            get
            {
                //logger.LogWarning("Count: " + count.ToString());
                return count;
            }

            private set => throw new NotImplementedException();
        }

        public bool IsReadOnly => true;

        public bool IsFixedSize => false;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList)fakelist).GetEnumerator();
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return fakelist.GetEnumerator();
        }

        int IList<T>.IndexOf(T item) => -1;
        int IList.IndexOf(object value) => -1;

        #endregion


        #region Interface Member Not Implemented

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
