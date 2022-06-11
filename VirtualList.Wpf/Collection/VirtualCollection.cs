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
using System.Windows.Threading;

namespace CiccioSoft.VirtualList.Wpf.Collection
{
    public abstract class VirtualCollection<T> : IList<T>, IList, INotifyCollectionChanged where T : class
    {
        private readonly ILogger logger;
        private readonly Dispatcher dispatcher;
        private CancellationTokenSource cancellationTokenSource;
        private Timer timer;
        ConcurrentStack<int> indexStack;
        private readonly IDictionary<int, T> ditems;
        private readonly T dummyModel;
        private readonly int range;
        private readonly int take;
        private int count;
        private int skip_to_fetch;

        public VirtualCollection(int range = 20)
        {
            logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger("VirtualCollectiont");
            dispatcher = System.Windows.Application.Current.Dispatcher;
            cancellationTokenSource = new CancellationTokenSource();
            timer = new Timer(TimerCalback, null, 50, 100);
            indexStack = new ConcurrentStack<int>();
            ditems = new ConcurrentDictionary<int, T>();
            dummyModel = CreateDummyEntity();
            this.range = range;
            take = range * 2;
        }

        private void TimerCalback(object? state)
        {
            if (indexStack.Count > 0)
            {
                var dt = DateTime.Now;
                logger.LogWarning("time timer: {0} {1}", dt.ToLongTimeString(), dt.Millisecond.ToString());
                indexStack.TryPop(out int idx);
                indexStack.Clear();
                Task.Run(async () => await FetchItem(idx));
            }
        }

        #region abstract method

        protected abstract T CreateDummyEntity();
        protected abstract int GetCount();
        protected abstract Task<List<T>> GetRangeAsync(int intskip, int size, CancellationToken cancellationToken);

        #endregion


        #region private method

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
            ditems.Clear();
            for (int i = 0; i < models.Count; i++)
            {
                ditems.Add(skip + i, models[i]);
            }

            if (!cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            dispatcher.Invoke(() =>
                CollectionChanged?.Invoke(
                    this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))
                );
        }

        private async Task FetchItem(int index)
        {
            if (index != 0)
            {
                // se l'indice non si trova all'interno della precedente richiesta
                if (index < skip_to_fetch || index >= skip_to_fetch + take)
                {
                    int skip;
                    if (index < range)
                        skip = 0;
                    else if (index > count - take)
                        skip = count - take;
                    else
                        skip = index - range;

                    skip_to_fetch = skip;

                    cancellationTokenSource.Cancel();
                    cancellationTokenSource.Dispose();
                    cancellationTokenSource = new CancellationTokenSource();

                    try
                    {
                        //await FetchRange(skip, cancellationTokenSource.Token);
                        await Task.Run(async () => await FetchRange(skip, cancellationTokenSource.Token), cancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException ex)
                    {
                        logger.LogError("Suca: " + ex.Message);
                    }

                }
            }
        }

        #endregion


        #region interface member Implemented

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public T this[int index]
        {
            get
            {
                if (ditems.ContainsKey(index))
                    return ditems[index];
                else
                {
                    //Task.Run(async () => await FetchItem(index));
                    indexStack.Push(index);
                    return dummyModel;
                }
            }
            set => throw new NotImplementedException();
        }

        object? IList.this[int index]
        {
            get
            {
                var obj = this[index];
                //logger.LogWarning("Index: {index} Id: {Id}", index, (obj as Model)!.Id);
                return obj;
            }
            set => throw new NotImplementedException();
        }

        public int Count
        {
            get
            {
                return count;
            }
            private set => throw new NotImplementedException();
        }

        public bool IsReadOnly => true;

        public bool IsFixedSize => false;

        public IEnumerator<T> GetEnumerator()
        {
            return new List<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            count = GetCount();
            Task.Run(() => dispatcher.InvokeAsync(() => FetchRange(0, cancellationTokenSource.Token)));
            return ((IList)new List<T>()).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return ditems.FirstOrDefault(x => x.Value == item).Key;
            //int idx = items.IndexOf(item);
            //return idx + skip_fetched;
        }

        int IList.IndexOf(object? value)
        {
            return IndexOf((T)value!);
        }

        #endregion


        #region interface member not implemented

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

        void ICollection<T>.Add(T item) => throw new NotImplementedException();

        int IList.Add(object? value) => throw new NotImplementedException();

        public void Clear() => throw new NotImplementedException();

        bool ICollection<T>.Contains(T item)
        {
            throw new NotImplementedException();
        }

        bool IList.Contains(object? value)
        {
            throw new NotImplementedException();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();

        void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();

        void IList<T>.Insert(int index, T item) => throw new NotImplementedException();

        void IList.Insert(int index, object? value) => throw new NotImplementedException();

        bool ICollection<T>.Remove(T item) => throw new NotImplementedException();

        void IList.Remove(object? value) => throw new NotImplementedException();

        public void RemoveAt(int index) => throw new NotImplementedException();

        #endregion
    }
}
