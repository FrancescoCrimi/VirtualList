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
        private readonly IDictionary<int, T> _items;
        private readonly List<T> _fakelist;
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
            _fakelist = new List<T>();
            _dummy = CreateDummyEntity();

            _tokenSource = new CancellationTokenSource();
            FirstIndex = 0;
            LastIndex = 0;
        }

        public async Task LoadAsync(string searchString)
        {
            _searchString = searchString;
            _count = await GetCountAsync(searchString);
            _items.Clear();

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
                LastIndex = 0 + lengthToFetch - 1;
                var token = NewToken();
                await Task.Run(async () => await FetchRange(FirstIndex, lengthToFetch, token), token);
            }
        }


        #region abstract method

        protected abstract T CreateDummyEntity();
        protected abstract Task<int> GetCountAsync(string searchString);
        protected abstract Task<List<T>> GetRangeAsync(string searchString, int skip, int take, CancellationToken token);

        #endregion


        #region private method

        private CancellationToken NewToken()
        {
            if (_tokenSource.Token.CanBeCanceled)
                _tokenSource.Cancel();
            _tokenSource.Dispose();
            _tokenSource = new CancellationTokenSource();
            return _tokenSource.Token;
        }

        private async Task FetchRange(int skip, int take, CancellationToken token)
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
                _logger.LogDebug("FetchRange: {0} - {1}", skip, skip + take - 1);
                var models = await GetRangeAsync(_searchString, skip, take, token);

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                // Aggiorno lista interna
                _items.Clear();
                for (var i = 0; i < models.Count; i++)
                {
                    _items.Add(skip + i, models[i]);
                }

                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                // invoco CollectionChanged Replace per singolo item
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    foreach (var item in _items)
                    {
                        if (token.IsCancellationRequested)
                            token.ThrowIfCancellationRequested();
                        var eventArgs = new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Replace,
                            item.Value,
                            null,
                            item.Key);
                        CollectionChanged?.Invoke(this, eventArgs);
                    }
                });
            }
            catch (TaskCanceledException tcex)
            {
                _logger.LogDebug("FetchRange: {0} - {1} {2} Id:{3}", skip, skip + take - 1, tcex.Message, tcex.Task.Id);
            }
            catch (OperationCanceledException ocex)
            {
                _logger.LogDebug(ocex.Message);
            }
        }

        #endregion


        #region interface member implemented

        public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            var firstVisible = visibleRange.FirstIndex;
            var lastVisible = visibleRange.LastIndex;
            var lengthVisible = (int)visibleRange.Length;
            _logger.LogDebug("VisibleRange: {0} - {1}", firstVisible, lastVisible);

            // se visibleRangeLength è minore di 2 esci
            if (lengthVisible < 2) return;

            // verifico se il range visibile rientra nel range già fetchato
            if (firstVisible < FirstIndex || lastVisible > LastIndex)
            {
                // trovo la lunghezza totale di righe da estrarre
                var lengthToFetch = lengthVisible * 3;

                // prima riga da estrarre
                int firstToFetch;

                // il range si trova all'inizio
                if (firstVisible < lengthVisible * 1)
                    firstToFetch = 0;

                // il range si trova alla fine
                else if (firstVisible >= _count - lengthVisible * 2)
                    firstToFetch = _count - lengthToFetch;

                // il range si trova nel mezzo
                else
                    firstToFetch = firstVisible - lengthVisible * 1;

                //valorizzo variabli globali firstindex e lastindex;
                FirstIndex = firstToFetch;
                LastIndex = firstToFetch + lengthToFetch - 1;
                Length = lengthVisible;

                var token = NewToken();
                Task.Run(async () => await FetchRange(firstToFetch, lengthToFetch, token), token);
            }
        }

        public T this[int index]
        {
            get
            {
                if (_items.TryGetValue(index, out T item))
                {
                    //_logger.LogDebug("Indexer get real: {0}", index);
                    return item;
                }
                else
                {
                    //_logger.LogDebug("Indexer get dummy: {0}", index);
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

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _fakelist.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IList)_fakelist).GetEnumerator();

        int IList<T>.IndexOf(T item) => -1;

        int IList.IndexOf(object value) => -1;

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
    }
}
