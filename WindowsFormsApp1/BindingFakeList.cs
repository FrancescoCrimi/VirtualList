using CiccioSoft.VirtualList.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace WindowsFormsApp1
{
    public class BindingFakeList : IList<Model>, IBindingList
    {
        private readonly List<Model> fakes;
        private readonly int count;

        public BindingFakeList()
        {
            fakes = new List<Model>();
            count = 10000;
            for (uint i = 0; i < count; i++)
            {
                fakes.Add(new Model(i, "null") { Id = i });
            }
        }

        public Model this[int index]
        {
            get
            {
                var item = fakes[index];
                return item;
            }
            set => throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get
            {
                var item = this[index];
                return item;
            }

            set => throw new NotImplementedException();
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public IEnumerator<Model> GetEnumerator()
        {
            return fakes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList)fakes).GetEnumerator();
        }

        public int IndexOf(Model item)
        {
            //int idx = fakes.IndexOf(item);
            //return idx;
            return -1;
        }

        public int IndexOf(object value)
        {
            //int idx = IndexOf((Model)value);
            //return idx;
            return -1;
        }





        public bool IsReadOnly => false;

        public bool IsFixedSize => false;

        // read only
        public bool AllowEdit => false;

        public bool AllowNew => false;

        public bool AllowRemove => false;

        public bool IsSorted => false;

        public ListSortDirection SortDirection => ListSortDirection.Ascending;

        public PropertyDescriptor SortProperty => null;

        public bool SupportsChangeNotification => true;

        public bool SupportsSearching => false;

        public bool SupportsSorting => false;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public event ListChangedEventHandler ListChanged;

        public void Add(Model item)
        {
            throw new NotImplementedException();
        }

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void AddIndex(PropertyDescriptor property)
        {
            // Not supported
        }

        public object AddNew() => throw new NotImplementedException();

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(Model item)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Model[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Find(PropertyDescriptor property, object key)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, Model item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Model item)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void RemoveIndex(PropertyDescriptor property)
        {
            // Not supported
        }

        public void RemoveSort()
        {
            throw new NotImplementedException();
        }
    }
}
