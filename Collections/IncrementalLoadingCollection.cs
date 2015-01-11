using Models;
using Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Collections
{
    public class IncrementalLoadingCollection : ObservableCollection<MyCollectionItem>, ISupportIncrementalLoading
    {
        private readonly int _pageSize = 10;
        private readonly CollectionService _collectionService;

        private bool _hasMoreItems;
        public bool HasMoreItems
        {
            get
            {
                return _hasMoreItems;
            }
        }

        private int _currentPage;

        public IncrementalLoadingCollection(CollectionService collectionService)
        {
            _collectionService = collectionService;

            Clear();
            UpdateHasMoreItems();
        }

        public new void Clear()
        {
            _hasMoreItems = false;
            _currentPage = 0;
            base.Clear();
        }

        public async Task<uint> GetPagedItems(CoreDispatcher dispatcher)
        {
            uint resultCount = 0;
            var collection = _collectionService.GetPagedItems(_currentPage, _pageSize);

            _currentPage += 1;
            UpdateHasMoreItems();

            await dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        () =>
                        {
                            foreach (var item in collection)
                            {
                                Add(item);
                            }
                        });

            resultCount = (uint)collection.Count();

            return resultCount;
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            var dispatcher = Window.Current.Dispatcher;

            return Task.Run<LoadMoreItemsResult>(
                async () =>
                {
                    uint resultCount = await GetPagedItems(dispatcher);
                    return new LoadMoreItemsResult() { Count = resultCount };

                }).AsAsyncOperation<LoadMoreItemsResult>();
        }

        private void UpdateHasMoreItems()
        {
            if (_collectionService.Count() > (_currentPage) * _pageSize)
            {
                _hasMoreItems = true;
            }
            else
            {
                _hasMoreItems = false;
            }
        }
    }
}
