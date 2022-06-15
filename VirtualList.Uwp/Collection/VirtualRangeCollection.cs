using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;

namespace CiccioSoft.VirtualList.Uwp
{
    /// <summary>
    /// Collezione Virtuale che usa l'interfaccia IItemsRangeInfo
    /// </summary>
    public abstract class VirtualRangeCollection<T> : IList<T>, IList, INotifyCollectionChanged, IItemsRangeInfo where T : class
    {
        private readonly ILogger logger;
        private readonly CoreDispatcher dispatcher;
        private CancellationTokenSource cancellationTokenSource = null;
        private readonly IDictionary<int, T> items;
        private readonly List<T> fakelist;
        protected int count;
        private readonly int cacheLength;
        private int FirstIndex;
        private int LastIndex;

        public VirtualRangeCollection()
        {
            logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger("VirtualRangeCollection");
            dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher;
            cancellationTokenSource = new CancellationTokenSource();
            items = new ConcurrentDictionary<int, T>();
            fakelist = new List<T>();
            cacheLength = 1;
            FirstIndex = 0;
            LastIndex = 0;
        }


        #region abstract method

        protected abstract T CreateDummyEntity();
        protected abstract int GetCount();
        protected abstract Task<List<T>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken);

        #endregion


        #region private method

        private async Task FetchRangeHandle(int skip, int take)
        {
            if (cancellationTokenSource.Token.CanBeCanceled)
                cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await FetchRange(skip, take, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
                logger.LogError(ex.Message);
            }
            catch (AggregateException agex)
            {
                logger.LogError("Cancel Task Id:{0}", ((TaskCanceledException)agex.InnerException).Task.Id);
            }
        }

        private async Task FetchRange(int skip, int take, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(10, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();
            var list = await GetRangeAsync(skip, take, cancellationToken);


            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();
            items.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                items.Add(skip + i, list[i]);
            }

            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                foreach (var item in items)
                {
                    var eventArgs = new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Replace,
                        item.Value,
                        null,
                        item.Key);
                    CollectionChanged?.Invoke(this, eventArgs);
                }
            });
        }

        #endregion


        #region interface member implemented

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            //var asdf = trackedItems.ToArray()[0];
            //int trackedItemsFirst = asdf.FirstIndex;
            //int trackedItemsLength = (int)asdf.Length;
            //logger.LogWarning("TrackedItems First: {0} Length: {1}", trackedItemsFirst, trackedItemsLength);

            int firstVisibleRange = visibleRange.FirstIndex;
            int lastVisibleRange = visibleRange.LastIndex;
            int lengthVisibleRange = (int)visibleRange.Length;

            // implementazione Cache
            // con il valore cacheLength si intende specificare la quantità di cache prima e dopo 
            // il range visibile prendendo come base la lunghezza del range.
            // per esempio con una length = 10 e cacheLength = 1 si avra un totale di 30 con il range
            // visibleRange al centro ovvero "length + (lengh * cacheLength) * 2

            // se visibleRangeLength è minore di 2 esci
            if (lengthVisibleRange < 2) return;
            logger.LogWarning("VisibleRange First: {0} Length: {1}", firstVisibleRange, lengthVisibleRange);

            // verifico se il range visibile rientra nel range già fetchato
            if (firstVisibleRange < FirstIndex || lastVisibleRange > LastIndex)
            {
                // trovo la lunghezza totale ri righe da estrarre ovvero lengthToFetch
                int lengthToFetch = lengthVisibleRange + (lengthVisibleRange * cacheLength) * 2;

                // verifico se mi trovo all'inizio o alla fine della collezione e trovo firstToFetch
                int firstToFetch;
                if (firstVisibleRange < lengthVisibleRange * cacheLength)
                    firstToFetch = 0;
                else if (firstVisibleRange > count - (lengthVisibleRange + lengthVisibleRange * cacheLength))
                    firstToFetch = count - lengthToFetch;
                else
                    firstToFetch = firstVisibleRange - lengthVisibleRange * cacheLength;
                logger.LogWarning("FetchRange First: {0} Length: {1}", firstToFetch, lengthToFetch);

                //valorizzo variabli globali firstindex e lastindex;
                FirstIndex = firstToFetch;
                LastIndex = firstToFetch + lengthToFetch - 1;

                Task.Run(async () => await FetchRangeHandle(firstToFetch, lengthToFetch));
            }
        }

        public T this[int index]
        {
            get
            {
                if (items.ContainsKey(index))
                    return items[index];
                else
                    return CreateDummyEntity();
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
                return count;
            }
        }

        public int IndexOf(T item) => -1;

        int IList.IndexOf(object value) => -1;

        public IEnumerator<T> GetEnumerator() => fakelist.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IList)fakelist).GetEnumerator();

        public bool IsReadOnly => true;

        bool IList.IsFixedSize => false;

        public void Dispose()
        {
            if (cancellationTokenSource.Token.CanBeCanceled)
                cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }

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
