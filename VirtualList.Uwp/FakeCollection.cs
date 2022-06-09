using CiccioSoft.VirtualList.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace CiccioSoft.VirtualList.Uwp
{
    internal class FakeCollection : IList<Model>, IList, IItemsRangeInfo, INotifyCollectionChanged
    {
        private readonly ILogger logger;
        private readonly int count;
        private readonly List<Model> fakes;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public FakeCollection()
        {
            logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger("FakeList");
            count = 10000;
            fakes = new FakeModelRepository(count).GetModels();
        }

        #region Public Method

        public void OnNotifyCollectionReset()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            logger.LogWarning("Evento Collection Reset");
        }

        #endregion


        #region Implemented Method Interface

        public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            var aa = trackedItems.ToArray();
            var ccc = trackedItems[0];
            logger.LogWarning("RangeChange {0} - {1}", visibleRange.FirstIndex, visibleRange.LastIndex);
        }

        object IList.this[int index]
        {
            get => this[index];
            set => throw new NotImplementedException();
        }

        public Model this[int index]
        {
            get
            {
                var item = fakes[index];
                logger.LogWarning("Index: {0}", index);
                return item;
            }
            set => throw new NotImplementedException();
        }

        public int Count
        {
            get
            {
                //logger.LogWarning("Count: {0}", count);
                return count;
            }

            private set => throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator() => ((IList)fakes).GetEnumerator();

        IEnumerator<Model> IEnumerable<Model>.GetEnumerator() => fakes.GetEnumerator();

        public int IndexOf(object value) => -1;

        int IList<Model>.IndexOf(Model item) => -1;

        public bool IsReadOnly => true;

        public bool IsFixedSize => false;

        #endregion


        #region Not Implemented Method Interface

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

        void ICollection<Model>.Add(Model item) => throw new NotImplementedException();

        int IList.Add(object value) => throw new NotImplementedException();

        bool ICollection<Model>.Contains(Model item) => throw new NotImplementedException();

        bool IList.Contains(object value) => throw new NotImplementedException();

        void ICollection<Model>.CopyTo(Model[] array, int arrayIndex) => throw new NotImplementedException();

        void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();

        void IList<Model>.Insert(int index, Model item) => throw new NotImplementedException();

        void IList.Insert(int index, object value) => throw new NotImplementedException();

        bool ICollection<Model>.Remove(Model item) => throw new NotImplementedException();

        void IList.Remove(object value) => throw new NotImplementedException();

        public void RemoveAt(int index) => throw new NotImplementedException();

        public void Clear() => throw new NotImplementedException();

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}
