using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;

namespace CiccioSoft.VirtualList.Uwp.Collection
{
    public abstract class VirtualRangeList<T> : IList<T>, IList, INotifyCollectionChanged, IItemsRangeInfo where T : class
    {
        private readonly ILogger logger;
        private CancellationTokenSource cancellationTokenSource = null;
        private readonly CoreDispatcher dispatcher;
        private readonly IDictionary<int, T> items;
        private readonly List<T> fakelist;
        protected int count;
        //private readonly int cacheLength;
        private int FirstIndex;
        private int LastIndex;

        public VirtualRangeList()
        {
            logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger("VirtualRangeCollection");
            cancellationTokenSource = new CancellationTokenSource();
            dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher;
            items = new ConcurrentDictionary<int, T>();
            fakelist = new List<T>();
            //cacheLength = 1;
            FirstIndex = 0;
            LastIndex = 0;
        }


        #region abstract method

        protected abstract T CreateDummyEntity();
        protected abstract int GetCount();
        protected abstract Task<List<T>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken);

        #endregion


        #region private method

        private async Task FetchRange(Range range, CancellationToken token)
        {
            try
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
                await Task.Delay(10, token);

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
                logger.LogWarning("FetchRange First: {0} Length: {1}", range.FirstTracked, range.LengthTracked);
                var list = await GetRangeAsync(range.FirstTracked, range.LengthTracked, token);


                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
                items.Clear();
                for (int i = 0; i < list.Count; i++)
                {
                    items.Add(range.FirstTracked + i, list[i]);
                }

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                for (int i = 0; i < range.LengthTracked; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                        //return;
                    }

                    int idx = range.FirstTracked + i;
                    var eventArgs = new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Replace,
                        items[idx],
                        null,
                        idx);
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => CollectionChanged?.Invoke(this, eventArgs));
                    //logger.LogWarning("CollectionChanged Replace: {0}", idx);
                }
            }
            catch (OperationCanceledException ex)
            {
                logger.LogError(ex.Message);
            }
            catch (AggregateException tcex)
            {
                logger.LogError("Cancel Task Id:{0}", ((TaskCanceledException)tcex.InnerException).Task.Id);
            }
        }

        #endregion


        #region interface member implemented

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            Range range = new Range();
            range.FirstVisible = visibleRange.FirstIndex;
            range.LastVisible = visibleRange.LastIndex;
            range.LengthVisible = (int)visibleRange.Length;
            logger.LogWarning("VisibleRange First: {0} Length: {1}", range.FirstVisible, range.LengthVisible);

            // se visibleRangeLength è minore di 2 esci
            if (range.LengthVisible < 2) return;

            // verifico se il range visibile rientra nel range già fetchato
            if (range.FirstVisible < FirstIndex || range.LastVisible > LastIndex)
            {
                // il range si trova all'inizio
                if (range.FirstVisible < range.LengthVisible * 1)
                {
                    // workaround x bug ListView visible/tracked +1 al alla prima richiesta 
                    range.LengthVisible++;
                    range.LastVisible++;                    

                    range.FirstTracked = 0;
                    range.LengthTracked = range.LengthVisible * 3;
                    range.LastTracked = range.LengthTracked - 1;
                    FirstIndex = range.FirstTracked;
                    LastIndex = range.LastTracked;
                }
                // il range si trova alla fine
                else if (range.FirstVisible >= count - range.LengthVisible * 2)
                {
                    range.FirstTracked = count - range.LengthVisible * 3;
                    range.LengthTracked = range.LengthVisible * 3;
                    range.LastTracked = count - 1;
                    FirstIndex = range.FirstTracked;
                    LastIndex = range.LastTracked;
                }
                // il range si trova nel mezzo
                else
                {
                    range.FirstTracked = range.FirstVisible - range.LengthVisible * 1;
                    range.LengthTracked = range.LengthVisible * 3;
                    range.LastTracked = range.FirstTracked + range.LengthVisible * 3 - 1;
                    FirstIndex = range.FirstTracked;
                    LastIndex = range.LastTracked;
                }

                if (cancellationTokenSource.Token.CanBeCanceled)
                    cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = new CancellationTokenSource();

                Task.Run(async () => await FetchRange(range, cancellationTokenSource.Token));
                //logger.LogWarning("FetchRange First: {0} Length: {1}", range.FirstTracked, range.LengthTracked);
            }
        }

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
