using CiccioSoft.VirtualList.Data;
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
    public class NewModelVirtualList : NewUwpVirtualList<Model>
    {

        //private readonly int count;
        private readonly FakeModelRepository repo;

        public NewModelVirtualList()
        {
            //count = 10000;
            repo = new FakeModelRepository(count);
        }

        protected override Model CreateDummyEntity()
        {
            return new Model(0, "");
        }

        protected override int GetCount()
        {
            return count;
        }

        protected override Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            return repo.GetRangeModelsAsync(skip, take, cancellationToken);
        }
    }

    public abstract class NewUwpVirtualList<T> : IList<T>, IList, INotifyCollectionChanged, IItemsRangeInfo where T : class
    {
        private readonly ILogger logger;
        private readonly CoreDispatcher dispatcher;
        private CancellationTokenSource cancellationTokenSource = null;
        private readonly IDictionary<int, T> items;
        private readonly List<T> fakelist;
        protected int count;

        public NewUwpVirtualList()
        {
            logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger("NewUwpVirtualList");
            dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher;
            cancellationTokenSource = new CancellationTokenSource();
            items = new ConcurrentDictionary<int, T>();
            fakelist = new List<T>();
            count = 10000;
        }

        #region abstract method

        protected abstract T CreateDummyEntity();
        protected abstract int GetCount();
        protected abstract Task<List<T>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken);

        #endregion


        #region private method

        private async Task FetchRange(int skip, int take, CancellationToken cancellationToken)
        {
            //if (cancellationToken.IsCancellationRequested)
            //    cancellationToken.ThrowIfCancellationRequested();
            ////await Task.Delay(10, cancellationToken);
            //Thread.Sleep(10);

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

            foreach (var item in items)
            {
                if (cancellationToken.IsCancellationRequested)
                    cancellationToken.ThrowIfCancellationRequested();

                var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                                                                     item.Value,
                                                                     null,
                                                                     item.Key);

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    CollectionChanged?.Invoke(this, eventArgs));
            }
        }

        #endregion


        #region Interface Implemented

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public async void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            try
            {
                if (cancellationTokenSource.Token.CanBeCanceled)
                    cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = new CancellationTokenSource();
                var asdf = trackedItems.ToArray()[0];
                //await Task.Run(async() =>
                await FetchRange(asdf.FirstIndex, (int)asdf.Length, cancellationTokenSource.Token);
                //, cancellationTokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
                logger.LogError(ex.Message);
            }
        }

        public object this[int index]
        {
            get
            {
                if (items.ContainsKey(index))
                {
                    return items[index];
                }
                else
                    return CreateDummyEntity();
            }

            set => throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            return -1;
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IList)fakelist).GetEnumerator();
        }

        public bool IsReadOnly => false;

        bool IList.IsFixedSize => false;

        #endregion


        #region Interface Not Implemented

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

        T IList<T>.this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        int IList.Add(object value) => throw new NotImplementedException();

        public void Clear() => throw new NotImplementedException();

        bool IList.Contains(object value) => throw new NotImplementedException();

        void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();

        void IList.Insert(int index, object value) => throw new NotImplementedException();

        void IList.Remove(object value) => throw new NotImplementedException();

        public void RemoveAt(int index) => throw new NotImplementedException();

        int IList<T>.IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        void IList<T>.Insert(int index, T item) => throw new NotImplementedException();

        void ICollection<T>.Add(T item) => throw new NotImplementedException();

        bool ICollection<T>.Contains(T item) => throw new NotImplementedException();

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

        bool ICollection<T>.Remove(T item) => throw new NotImplementedException();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => throw new NotImplementedException();

        #endregion


        public void Dispose()
        {
            if (cancellationTokenSource.Token.CanBeCanceled)
                cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        }
    }
}
