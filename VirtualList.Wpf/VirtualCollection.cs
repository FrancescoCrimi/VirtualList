using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CiccioSoft.VirtualList.Wpf
{
    public abstract class VirtualCollection<T> : IList<T>, IList, INotifyCollectionChanged where T : class
    {
        private readonly ILogger logger;
        private readonly Dispatcher dispatcher;
        private readonly int range;
        private readonly int take;
        private readonly T dummyModel;
        private readonly List<T> items;
        private int count;
        private int skip_fetched;
        private int skip_to_fetch;
        private CancellationTokenSource cancellationTokenSource;

        public VirtualCollection(int range = 20)
        {
            logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger("VirtualCollectiont");
            dispatcher = App.Current.Dispatcher;
            this.range = range;
            take = range * 2;
            dummyModel = CreateDummyEntity();
            items = new List<T>();
            for (int i = 0; i < take; i++)
            {
                items.Add(dummyModel);
            }
            cancellationTokenSource = new CancellationTokenSource();
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
            //await Task.Delay(500, cancellationToken);

            // recupero i dati
            List<T> models = await GetRangeAsync(skip, take, cancellationToken);

            logger.LogWarning("FetchData: {0} - {1}", skip, take + skip - 1);

            if (!cancellationToken.IsCancellationRequested)
            {
                for (int i = 0; i < models.Count; i++)
                {
                    items[i] = models[i];
                }
            }
            else return;

            if (!cancellationToken.IsCancellationRequested)
            {
                skip_fetched = skip;
                dispatcher.Invoke(() =>
                    CollectionChanged?.Invoke(
                        this,
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))
                    );
            }
        }

        private T FetchItem(int index)
        {
            // se l'indice è già fetchato
            if (index >= skip_fetched && index < skip_fetched + take)
            {
                return items[index - skip_fetched];
            }
            else
            {
                // se indice 0 non si trova tra gli item gia fetchati basta ritornare il dummy
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
                        //dispatcher.Invoke(() => FetchRange(skip, cancellationTokenSource.Token));
                        Task.Run(async () => await FetchRange(skip, cancellationTokenSource.Token));
                    }
                }
                return dummyModel;
            }
        }

        #endregion


        #region interface member Implemented

        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        public T this[int index]
        {
            get
            {
                return FetchItem(index);
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
            int idx = items.IndexOf(item);
            return idx + skip_fetched;
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
