// Copyright (c) 2024 Francesco Crimi francrim@gmail.com
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;

namespace CiccioSoft.VirtualList.Uwp
{
    /// <summary>
    /// Collezione Virtuale
    /// 
    /// per funzionare correttamente impostare la Proprietà CacheLength dell'ItemStackPanel a 0.0 cosi
    ///
    ///     <ListView.ItemsPanel>
    ///       <ItemsPanelTemplate>
    ///         <ItemsStackPanel Orientation = "Vertical" CacheLength="0.0"/>
    ///       </ItemsPanelTemplate>
    ///     </ListView.ItemsPanel>
    ///     
    /// Per usare la classe subclassa questa classe implementando i metodi astratti
    /// </summary>
    public abstract class VirtualRangeCollection<T> : IVirtualRangeCollection<T> where T : class
    {
        private readonly ILogger _logger;
        private readonly CoreDispatcher _dispatcher;
        private readonly ConcurrentDictionary<int, T> _items;
        private readonly T _dummy;
        private CancellationTokenSource _tokenSource;
        private int _count = 0;
        private string _searchString = "";
        private int FirstIndex;
        private int LastIndex;
        private int Length;
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";

        public VirtualRangeCollection(ILogger logger = null)
        {
            _logger = logger;
            _dispatcher = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().Dispatcher;
            _items = new ConcurrentDictionary<int, T>();
            _dummy = CreateDummyEntity();
            _tokenSource = new CancellationTokenSource();
            FirstIndex = 0;
            LastIndex = 0;
        }


        #region interface member implemented

        public async Task LoadAsync(string searchString)
        {
            _searchString = searchString;
            _items.Clear();
            _count = await GetCountAsync(searchString);

            await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(CountString));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(IndexerName));
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            });

            if (Length > 0)
            {
                var lengthToFetch = Length * 3;
                FirstIndex = 0;
                LastIndex = lengthToFetch - 1;

                var token = NewToken();
                _ = FetchRange(FirstIndex, lengthToFetch, token);
            }
        }

        public void RangesChanged(ItemIndexRange visibleRange,
                                  IReadOnlyList<ItemIndexRange> trackedItems)
        {
            var visibleFirst = visibleRange.FirstIndex;
            var visibleLast = visibleRange.LastIndex;
            var visibleLength = (int)visibleRange.Length;
            _logger.LogTrace("VisibleRange: {first} - {last}", visibleFirst, visibleLast);

            // se visibleRangeLength è minore di 2 esci
            if (visibleLength < 2) return;

            // verifico se il range visibile rientra nel range già fetchato
            if (visibleFirst < FirstIndex || visibleLast > LastIndex)
            {
                // trovo la lunghezza totale di righe da estrarre
                var lengthToFetch = visibleLength * 3;

                // prima riga da estrarre
                int firstToFetch;

                // il range si trova all'inizio
                if (visibleFirst < visibleLength * 1)
                    firstToFetch = 0;

                // il range si trova alla fine
                else if (visibleFirst >= _count - visibleLength * 2)
                    firstToFetch = _count - lengthToFetch;

                // il range si trova nel mezzo
                else
                    firstToFetch = visibleFirst - visibleLength * 1;

                //valorizzo variabli globali firstindex e lastindex;
                FirstIndex = firstToFetch;
                LastIndex = firstToFetch + lengthToFetch - 1;
                Length = visibleLength;

                var token = NewToken();
                _ = FetchRange(firstToFetch, lengthToFetch, token);
            }
        }

        public T this[int index]
        {
            get
            {
                if (_items.TryGetValue(index, out T item))
                {
                    _logger.LogTrace("Indexer get real: {index}", index);
                    return item;
                }
                else
                {
                    _logger.LogTrace("Indexer get dummy: {index}", index);
                    return _dummy;
                }
            }
            set => throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get => this[index];
            set => throw new NotImplementedException();
        }

        public int Count => _count;

        public bool IsReadOnly => true;

        public bool IsFixedSize => false;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _items.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_items.Values).GetEnumerator();

        int IList<T>.IndexOf(T item) => ((IList<T>)_items.Values).IndexOf(item);

        int IList.IndexOf(object value) => ((IList)_items.Values).IndexOf(value);

        public void Dispose()
        {
            if (_tokenSource.Token.CanBeCanceled)
                _tokenSource.Cancel();
            _tokenSource.Dispose();
        }

        #endregion


        #region interface member not implemented

        void ICollection<T>.Add(T item) => throw new NotImplementedException();
        int IList.Add(object value) => throw new NotImplementedException();
        void ICollection<T>.Clear() => throw new NotImplementedException();
        void IList.Clear() => throw new NotImplementedException();
        bool ICollection<T>.Contains(T item) => throw new NotImplementedException();
        bool IList.Contains(object value) => throw new NotImplementedException();
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => throw new NotImplementedException();
        void ICollection.CopyTo(Array array, int index) => throw new NotImplementedException();
        void IList<T>.Insert(int index, T item) => throw new NotImplementedException();
        void IList.Insert(int index, object value) => throw new NotImplementedException();
        bool ICollection<T>.Remove(T item) => throw new NotImplementedException();
        void IList.Remove(object value) => throw new NotImplementedException();
        void IList<T>.RemoveAt(int index) => throw new NotImplementedException();
        void IList.RemoveAt(int index) => throw new NotImplementedException();
        bool ICollection.IsSynchronized => throw new NotImplementedException();
        object ICollection.SyncRoot => throw new NotImplementedException();

        #endregion


        #region abstract method

        protected abstract T CreateDummyEntity();
        protected abstract Task<int> GetCountAsync(string searchString);
        protected abstract Task<List<T>> GetRangeAsync(string searchString,
                                                       int skip,
                                                       int take,
                                                       CancellationToken token);

        #endregion


        #region private method

        private async Task FetchRange(int skip, int take, CancellationToken token)
        {
            try
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                // pulisco la lista interna
                _items.Clear();

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                // recupero i dati
                _logger.LogDebug("FetchRange: {from} - {to}", skip, skip + take - 1);
                var models = await GetRangeAsync(_searchString, skip, take, token);

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                // Aggiorno lista interna
                for (var i = 0; i < models.Count; i++)
                {
                    _items.TryAdd(skip + i, models[i]);
                }

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                // invoco CollectionChanged Replace per singolo item
                foreach (var item in _items)
                {
                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();
                    var eventArgs = new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Replace,
                        item.Value,
                        null,
                        item.Key);
                    await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        CollectionChanged?.Invoke(this, eventArgs);
                    });
                }
            }
            catch (TaskCanceledException)
            {
                _logger?.LogDebug("TaskCanceled: {from} - {to}", skip, skip + take - 1);
            }
            catch (OperationCanceledException)
            {
                _logger?.LogDebug("OperationCanceled: {from} - {to}", skip, skip + take - 1);
            }
        }

        private CancellationToken NewToken()
        {
            if (_tokenSource.Token.CanBeCanceled)
                _tokenSource.Cancel();
            _tokenSource.Dispose();
            _tokenSource = new CancellationTokenSource();
            return _tokenSource.Token;
        }

        #endregion
    }
}
