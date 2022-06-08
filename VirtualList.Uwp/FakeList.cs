using CiccioSoft.VirtualList.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace CiccioSoft.VirtualList.Uwp
{
    internal class FakeList : IList<Model>, IList, IItemsRangeInfo
    {
        private readonly ILogger logger;
        private readonly int count;
        private readonly List<Model> fakes;

        public FakeList()
        {
            logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger("FakeList");
            count = 10000;
            fakes = new FakeModelRepository(count).GetModels();
        }

        public object this[int index]
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

        public IEnumerator GetEnumerator()
        {
            logger.LogWarning("Enumerator");
            return ((IList)fakes).GetEnumerator();
        }

        public int IndexOf(object value)
        {
            logger.LogWarning("IndexOf");
            return -1;
        }

        public bool IsReadOnly => true;

        public bool IsFixedSize => false;


        #region Not Implemented

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

        Model IList<Model>.this[int index]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        void ICollection<Model>.Add(Model item)
        {
            throw new NotImplementedException();
        }

        int IList.Add(object value)
        {
            throw new NotImplementedException();
        }

        bool ICollection<Model>.Contains(Model item)
        {
            throw new NotImplementedException();
        }

        bool IList.Contains(object value)
        {
            throw new NotImplementedException();
        }

        void ICollection<Model>.CopyTo(Model[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        void IList<Model>.Insert(int index, Model item)
        {
            throw new NotImplementedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        bool ICollection<Model>.Remove(Model item)
        {
            throw new NotImplementedException();
        }

        void IList.Remove(object value)
        {
            throw new NotImplementedException();
        }

        void IList<Model>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        int IList<Model>.IndexOf(Model item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<Model> IEnumerable<Model>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            var aa = trackedItems.ToArray();  
            var ccc = trackedItems[0];
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}
