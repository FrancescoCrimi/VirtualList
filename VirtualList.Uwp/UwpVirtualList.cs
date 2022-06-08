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
using Windows.UI.Xaml;

namespace CiccioSoft.VirtualList.Uwp
{
    public class ModelVirtualList : UwpVirtualList<Model>
    {
        private readonly IModelRepository modelRepository;

        public ModelVirtualList(IModelRepository modelRepository)
            : base()
        {
            this.modelRepository = modelRepository;
            count = GetCount();
        }

        protected override Model CreateDummyEntity()
        {
            return new Model(0, "null");
        }

        protected override int GetCount()
        {
            return modelRepository.Count();
        }

        protected override Task<List<Model>> GetRangeAsync(int skip, int take, CancellationToken cancellationToken)
        {
            return modelRepository.GetRangeModelsAsync(skip, take, cancellationToken);
        }
    }

    public abstract class UwpVirtualList<T> : IList, INotifyCollectionChanged where T : class
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

        public UwpVirtualList(int range = 20)
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
                logger.LogWarning("Pippo: {0}", idx);
            }
        }

        private async Task FetchRange(int index, CancellationToken cancellationToken)
        {
            //// Aggiungo ritardo solo per test
            //if (cancellationToken.IsCancellationRequested)
            //    cancellationToken.ThrowIfCancellationRequested();
            //await Task.Delay(1000, cancellationToken);

            // recupero i dati
            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();
            List<T> models = await GetRangeAsync(index, take, cancellationToken);

            // Aggiorno lista interna
            items.Clear();
            for (int i = 0; i < models.Count; i++)
            {
                items.Add(index + i, models[i]);
            }

            // invoco CollectionChanged Replace per singolo item
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

        private void FetchItem(int index)
        {
            if (index < range)
                index = 0;
            else if (index > count - range)
                index = count - range * 2;
            else
                index = index - range;

            if (cancellationTokenSource.Token.CanBeCanceled)
                cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            logger.LogWarning("Create thread FetchRange index: {0}", index);

            try
            {
                Task.Run(async () =>
                    //await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    await FetchRange(index, cancellationTokenSource.Token)
                    //    .AsAsyncAction()
                    //)
                    );
            }
            catch (OperationCanceledException ex)
            {
                logger.LogError(ex.Message);
            }

        }

        #endregion


        #region Interface Member Implemented 

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        object IList.this[int index]
        {
            get
            {
                //logger.LogWarning("Index: {0}", index);
                if (items.ContainsKey(index))
                {
                    return items[index];
                }
                else
                {
                    //FetchItem(index);
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

        public bool IsReadOnly => false;

        public bool IsFixedSize => false;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList)fakelist).GetEnumerator();
        }

        int IList.IndexOf(object value)
        {
            return -1;
        }

        #endregion


        #region Interface Member Not Implemented

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

        int IList.Add(object value) => throw new NotImplementedException();

        void IList.Clear() => throw new NotImplementedException();

        bool IList.Contains(object value) => throw new NotImplementedException();

        void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();

        void IList.Insert(int index, object value) => throw new NotImplementedException();

        void IList.Remove(object value) => throw new NotImplementedException();

        void IList.RemoveAt(int index) => throw new NotImplementedException();

        #endregion
    }
}
