using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CiccioSoft.VirtualList.Wpf.Collection
{
    public abstract class VirtualCollection<T> : IVirtualCollection<T> where T : class
    {
        private readonly ILogger logger;
        private readonly Dispatcher dispatcher;
        private CancellationTokenSource cancellationTokenSource;
        private readonly ConcurrentStack<int> indexStack;
        private readonly Timer timer;
        private readonly IDictionary<int, T> items;
        private readonly List<T> fakelist;
        private readonly T dummyObject;
        private int count = 0;
        private int selectedIndex = -1;
        private readonly int range;
        private readonly int take;
        private int index_to_fetch = 0;
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";

        public VirtualCollection(int range = 20)
        {
            logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger("VirtualCollection");
            dispatcher = System.Windows.Application.Current.Dispatcher;
            cancellationTokenSource = new CancellationTokenSource();
            indexStack = new ConcurrentStack<int>();
            timer = new Timer(TimerHandler, null, 50, 50);
            items = new ConcurrentDictionary<int, T>();
            fakelist = new List<T>();
            dummyObject = CreateDummyEntity();
            this.range = range;
            take = range * 2;
        }

        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                if (selectedIndex != value)
                {
                    selectedIndex = value;
                    dispatcher.Invoke(() =>
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedIndex")));
                }
            }
        }

        protected async Task LoadAsync()
        {
            SelectedIndex = -1;
            index_to_fetch = 0;
            count = await GetCountAsync();
            logger.LogInformation("FetchData: {0} - {1}", 0, take - 1);
            var models = await GetRangeAsync(0, take, NewToken());
            items.Clear();
            for (var i = 0; i < models.Count; i++)
            {
                items.Add(i, models[i]);
            }
            await dispatcher.InvokeAsync(() =>
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
        protected abstract Task<List<T>> GetRangeAsync(int intskip, int size, CancellationToken token);

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

                        //logger.LogInformation("timer: {0} {1} index: {2}",
                        //                      DateTime.Now.ToLongTimeString(),
                        //                      DateTime.Now.Millisecond.ToString(),
                        //                      index.ToString());

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
                            //logger.LogInformation("timer: {0} {1} skip: {2}",
                            //                  DateTime.Now.ToLongTimeString(),
                            //                  DateTime.Now.Millisecond.ToString(),
                            //                  index.ToString());
                            var token = NewToken();
                            Task.Run(async () => await FetchRange(index, token), token);
                        }
                        else
                            logger.LogInformation("Indice già Fetchato: {0}", index);
                    }
                }
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
                logger.LogInformation("FetchRange: {0} - {1}", skip, take + skip - 1);
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

                SelectedIndex = -1;
                await dispatcher.InvokeAsync(() =>
                    CollectionChanged?.Invoke(
                        this,
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)));
            }
            catch (TaskCanceledException tcex)
            {
                logger.LogInformation("FetchRange: {0} - {1} {2} Id:{3}", skip, skip + take - 1, tcex.Message, tcex.Task?.Id);
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

        object? IList.this[int index]
        {
            get => this[index];
            set => throw new NotImplementedException();
        }

        public int Count => count;

        public bool IsReadOnly => true;

        public bool IsFixedSize => false;

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => fakelist.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IList)fakelist).GetEnumerator();
        int IList<T>.IndexOf(T item) => -1;
        int IList.IndexOf(object? value) => -1;

        #endregion


        #region interface member not implemented

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

        void ICollection<T>.Add(T item) => throw new NotImplementedException();
        int IList.Add(object? value) => throw new NotImplementedException();
        void ICollection<T>.Clear() => throw new NotImplementedException();
        void IList.Clear() => throw new NotImplementedException();
        bool ICollection<T>.Contains(T item) => throw new NotImplementedException();
        bool IList.Contains(object? value) => throw new NotImplementedException();
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
        void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();
        void IList<T>.Insert(int index, T item) => throw new NotImplementedException();
        void IList.Insert(int index, object? value) => throw new NotImplementedException();
        bool ICollection<T>.Remove(T item) => throw new NotImplementedException();
        void IList.Remove(object? value) => throw new NotImplementedException();
        void IList<T>.RemoveAt(int index) => throw new NotImplementedException();
        void IList.RemoveAt(int index) => throw new NotImplementedException();

        #endregion
    }
}
