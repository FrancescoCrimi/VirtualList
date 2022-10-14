using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
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
using Windows.UI.Core;
using Windows.UI.Xaml.Data;

namespace CiccioSoft.VirtualList.Uwp.Collection
{
    public abstract class VirtualRangeList<T> : IList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged, IItemsRangeInfo where T : class
    {
        private readonly ILogger logger;
        private readonly CoreDispatcher dispatcher;
        private CancellationTokenSource cancellationTokenSource = null;
        private readonly IDictionary<int, T> items;
        private readonly List<T> fakelist;
        private readonly T dummyObject;
        private int count = 0;
        private int FirstIndex;
        private int LastIndex;

        public VirtualRangeList()
        {
            logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger("VirtualRangeCollection");
            dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher;
            cancellationTokenSource = new CancellationTokenSource();
            items = new ConcurrentDictionary<int, T>();
            fakelist = new List<T>();
            dummyObject = CreateDummyEntity();
            FirstIndex = 0;
            LastIndex = 0;
        }


        #region abstract method

        protected abstract T CreateDummyEntity();
        protected abstract Task<int> GetCountAsync();
        protected abstract Task<List<T>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken);

        #endregion


        #region public method

        public async Task LoadAsync()
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                count = await GetCountAsync();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            });
        }

        #endregion


        #region private method

        private async Task FetchRange(int firstTracked, int lengthTracked, CancellationToken token)
        {
            try
            {
                //if (token.IsCancellationRequested)
                //    token.ThrowIfCancellationRequested();
                //await Task.Delay(10, token);

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
                logger.LogWarning("FetchRange First: {0} Length: {1}", firstTracked, lengthTracked);
                var list = await GetRangeAsync(firstTracked, lengthTracked, token);

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
                items.Clear();
                for (int i = 0; i < list.Count; i++)
                {
                    items.Add(firstTracked + i, list[i]);
                }

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();
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
                        //logger.LogWarning("CollectionChanged Replace: {0}", item.Key);
                    }
                });
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
        public event PropertyChangedEventHandler PropertyChanged;

        public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            int firstVisible = visibleRange.FirstIndex;
            int lastVisible = visibleRange.LastIndex;
            int lengthVisible = (int)visibleRange.Length;
            logger.LogWarning("VisibleRange First: {0} Length: {1}", firstVisible, lengthVisible);

            // se visibleRangeLength è minore di 2 esci
            if (lengthVisible < 2) return;

            // verifico se il range visibile rientra nel range già fetchato
            if (firstVisible < FirstIndex || lastVisible > LastIndex)
            {
                // trovo la lunghezza totale di righe da estrarre
                int lengthTracked = lengthVisible * 3;

                // prima riga da estrarre
                int firstTracked;

                // il range si trova all'inizio
                if (firstVisible < lengthVisible * 1)
                    firstTracked = 0;

                // il range si trova alla fine
                else if (firstVisible >= count - lengthVisible * 2)
                    firstTracked = count - lengthTracked;

                // il range si trova nel mezzo
                else
                    firstTracked = firstVisible - lengthVisible * 1;

                //valorizzo variabli globali firstindex e lastindex;
                FirstIndex = firstTracked;
                LastIndex = firstTracked + lengthTracked - 1;

                if (cancellationTokenSource.Token.CanBeCanceled)
                    cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = new CancellationTokenSource();

                Task.Run(async () => await FetchRange(firstTracked, lengthTracked, cancellationTokenSource.Token));
                //logger.LogWarning("FetchRange First: {0} Length: {1}", firstTracked, lengthTracked);
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
