﻿using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.ComponentModel;

namespace CiccioSoft.VirtualList.Wpf.Collection
{
    public abstract class VirtualCollection<T> : IList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged where T : class
    {
        private readonly ILogger logger;
        private readonly Dispatcher dispatcher;
        private CancellationTokenSource cancellationTokenSource;
        private readonly ConcurrentStack<int> indexStack;
        private readonly Timer timer;
        private readonly IDictionary<int, T> items;
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
            dispatcher = System.Windows.Application.Current.Dispatcher;
            cancellationTokenSource = new CancellationTokenSource();
            timer = new Timer(TimerHandler, null, 50, 50);
            indexStack = new ConcurrentStack<int>();
            items = new ConcurrentDictionary<int, T>();
            dummyObject = CreateDummyEntity();
            this.range = range;
            take = range * 2;
            count = GetCount();
            index_to_fetch = int.MaxValue;
        }

        #region abstract method

        protected abstract T CreateDummyEntity();
        protected abstract int GetCount();
        protected abstract Task<List<T>> GetRangeAsync(int intskip, int size, CancellationToken cancellationToken);

        #endregion


        #region private method

        private void TimerHandler(object? state)
        {
            if (!indexStack.IsEmpty)
            {
                while (!indexStack.IsEmpty)
                {
                    indexStack.TryPop(out int index);

                    // trick per datagrid
                    if (index != 0)
                    {
                        indexStack.Clear();

                        logger.LogWarning("timer: {0} {1} index: {2}",
                                          DateTime.Now.ToLongTimeString(),
                                          DateTime.Now.Millisecond.ToString(),
                                          index.ToString());

                        if (index < index_to_fetch || index >= index_to_fetch + take)
                        {
                            if (index < range)
                                index = 0;
                            else if (index > count - range)
                                index = count - take;
                            else
                                index -= range;
                            index_to_fetch = index;
                            logger.LogWarning("timer: {0} {1} skip: {2}",
                                              DateTime.Now.ToLongTimeString(),
                                              DateTime.Now.Millisecond.ToString(),
                                              index.ToString());
                            Task.Run(async () => await FetchItem(index));
                        }
                        else
                            logger.LogWarning("Indice già fetchato");
                    }
                }
            }
        }

        private async Task FetchItem(int skip)
        {
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            try
            {
                await Task.Run(async () => await FetchRange(skip, cancellationTokenSource.Token), cancellationTokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
                logger.LogError(ex.Message);
            }
        }

        private async Task FetchRange(int skip, CancellationToken cancellationToken)
        {
            //// Aggiungo ritardo solo per test
            //if (!cancellationToken.IsCancellationRequested)
            //    cancellationToken.ThrowIfCancellationRequested();
            //await Task.Delay(50, cancellationToken);

            // recupero i dati
            if (!cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();
            List<T> models = await GetRangeAsync(skip, take, cancellationToken);

            logger.LogWarning("FetchData: {0} - {1}", skip, take + skip - 1);

            if (!cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();
            items.Clear();
            for (int i = 0; i < models.Count; i++)
            {
                items.Add(skip + i, models[i]);
            }

            if (!cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            dispatcher.Invoke(() =>
                CollectionChanged?.Invoke(
                    this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)));

            //dispatcher.Invoke(() =>
            //{
            //    foreach (var item in items)
            //    {
            //        if (cancellationToken.IsCancellationRequested)
            //            cancellationToken.ThrowIfCancellationRequested();

            //        var eventarg = new NotifyCollectionChangedEventArgs(
            //            NotifyCollectionChangedAction.Replace,
            //            item.Value,
            //            null,
            //            item.Key);

            //        CollectionChanged?.Invoke(this, eventarg);
            //    }
            //});
        }

        #endregion


        #region interface member Implemented

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public T this[int index]
        {
            get
            {
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

        object? IList.this[int index]
        {
            get => this[index];
            set => throw new NotImplementedException();
        }

        public int Count => count;

        public IEnumerator<T> GetEnumerator()
        {
            return new List<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList)new List<T>()).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return items.FirstOrDefault(x => x.Value == item).Key;
        }

        int IList.IndexOf(object? value)
        {
            return IndexOf((T)value!);
        }

        public bool IsReadOnly => true;

        public bool IsFixedSize => false;

        #endregion


        #region interface member not implemented

        void ICollection<T>.Add(T item) => throw new NotImplementedException();

        int IList.Add(object? value) => throw new NotImplementedException();

        public void Clear() => throw new NotImplementedException();

        bool ICollection<T>.Contains(T item) => throw new NotImplementedException();

        bool IList.Contains(object? value) => throw new NotImplementedException();

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

        void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();

        void IList<T>.Insert(int index, T item) => throw new NotImplementedException();

        void IList.Insert(int index, object? value) => throw new NotImplementedException();

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        bool ICollection<T>.Remove(T item) => throw new NotImplementedException();

        void IList.Remove(object? value) => throw new NotImplementedException();

        public void RemoveAt(int index) => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

        #endregion
    }
}
