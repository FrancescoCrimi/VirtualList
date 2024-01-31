using CiccioSoft.VirtualList.Sample.Uwp.Database;
using CiccioSoft.VirtualList.Sample.Uwp.Domain;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace CiccioSoft.VirtualList.Sample.Uwp.Collection
{
    public class FakeCollection : ICollection<Model>,
                                  IEnumerable<Model>,
                                  IEnumerable,
                                  IList<Model>,
                                  IReadOnlyCollection<Model>,
                                  IReadOnlyList<Model>,
                                  ICollection,
                                  IList,
                                  INotifyCollectionChanged,
                                  INotifyPropertyChanged,
                                  IItemsRangeInfo
    {
        private readonly ILogger logger;
        private List<Model> fakelist;
        private int count = 0;
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";

        public FakeCollection()
        {
            logger = Ioc.Default.GetRequiredService<ILoggerFactory>().CreateLogger<FakeCollection>();
            fakelist = SampleDataService.ReadFromFile("SampleData.json");
            count = fakelist.Count;
        }

        private void OnNotifyCollectionReset()
        {
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CountString));
            //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            logger.LogWarning("Evento Collection Reset");
        }

        public Task LoadAsync(string str = "")
        {
            OnNotifyCollectionReset();
            return Task.CompletedTask;
        }


        #region Implemented Method Interface

        public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            logger.LogWarning("RangeChange {0} - {1}", visibleRange.FirstIndex, visibleRange.LastIndex);
        }

        public Model this[int index]
        {
            get
            {
                var item = fakelist[index];
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

        public bool IsReadOnly => true;

        public bool IsFixedSize => false;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        IEnumerator<Model> IEnumerable<Model>.GetEnumerator() => fakelist.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IList)fakelist).GetEnumerator();

        int IList<Model>.IndexOf(Model item) => -1;

        int IList.IndexOf(object value) => -1;

        public void Dispose() { }

        #endregion


        #region interface member not implemented

        void ICollection<Model>.Add(Model item) => throw new NotImplementedException();
        int IList.Add(object value) => throw new NotImplementedException();
        void ICollection<Model>.Clear() => throw new NotImplementedException();
        void IList.Clear() => throw new NotImplementedException();
        bool ICollection<Model>.Contains(Model item) => throw new NotImplementedException();
        bool IList.Contains(object value) => throw new NotImplementedException();
        void ICollection<Model>.CopyTo(Model[] array, int arrayIndex) => throw new NotImplementedException();
        void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();
        void IList<Model>.Insert(int index, Model item) => throw new NotImplementedException();
        void IList.Insert(int index, object value) => throw new NotImplementedException();
        bool ICollection<Model>.Remove(Model item) => throw new NotImplementedException();
        void IList.Remove(object value) => throw new NotImplementedException();
        void IList<Model>.RemoveAt(int index) => throw new NotImplementedException();
        void IList.RemoveAt(int index) => throw new NotImplementedException();
        bool ICollection.IsSynchronized => throw new NotImplementedException();
        object ICollection.SyncRoot => throw new NotImplementedException();

        #endregion
    }
}

