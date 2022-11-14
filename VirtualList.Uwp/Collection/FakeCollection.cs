using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using CiccioSoft.VirtualList.Data.Domain;
using CiccioSoft.VirtualList.Data.Infrastructure;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using Windows.UI.Xaml.Data;

namespace CiccioSoft.VirtualList.Uwp
{
    internal class FakeCollection : IList<Model>, IList, IItemsRangeInfo, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly ILogger logger;
        private int count = 0;
        private List<Model> fakes;
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public FakeCollection()
        {
            logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger("FakeCollection");
        }

        private void OnNotifyCollectionReset()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CountString));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            logger.LogWarning("Evento Collection Reset");
        }


        #region Public Method

        public Task LoadAsync(string str = "")
        {
            fakes = SampleGenerator.Generate(1000000);
            count = fakes.Count;
            OnNotifyCollectionReset();
            return Task.CompletedTask;
        }

        #endregion


        #region Implemented Method Interface

        public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            logger.LogWarning("RangeChange {0} - {1}", visibleRange.FirstIndex, visibleRange.LastIndex);
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

        object IList.this[int index]
        {
            get => this[index];
            set => throw new NotImplementedException();
        }

        public int Count => count;

        public IEnumerator<Model> GetEnumerator() => fakes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IList)fakes).GetEnumerator();

        public int IndexOf(Model item) => -1;

        int IList.IndexOf(object value) => -1;

        public bool IsReadOnly => true;

        public bool IsFixedSize => false;

        #endregion


        #region Not Implemented Method Interface

        void ICollection<Model>.Add(Model item) => throw new NotImplementedException();

        int IList.Add(object value) => throw new NotImplementedException();

        public void Clear() => throw new NotImplementedException();

        bool ICollection<Model>.Contains(Model item) => throw new NotImplementedException();

        bool IList.Contains(object value) => throw new NotImplementedException();

        void ICollection<Model>.CopyTo(Model[] array, int arrayIndex) => throw new NotImplementedException();

        void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();

        void IList<Model>.Insert(int index, Model item) => throw new NotImplementedException();

        void IList.Insert(int index, object value) => throw new NotImplementedException();

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        bool ICollection<Model>.Remove(Model item) => throw new NotImplementedException();

        void IList.Remove(object value) => throw new NotImplementedException();

        public void RemoveAt(int index) => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}
