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
    public class IncrementalLoadingWantlist : ObservableCollection<MyWant>, ISupportIncrementalLoading
    {
        private readonly int _pageSize = 10;
        private readonly WantlistService _wantlistService;

        private bool _hasMoreItems;
        public bool HasMoreItems
        {
            get
            {
                return _hasMoreItems;
            }
        }

        private int _currentPage;

        public IncrementalLoadingWantlist(WantlistService wantlistService)
        {
            _wantlistService = wantlistService;

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
            var wants = _wantlistService.GetPagedItems(_currentPage, _pageSize);

            _currentPage += 1;
            UpdateHasMoreItems();

            await dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        () =>
                        {
                            foreach (var want in wants)
                            {
                                Add(want);
                            }
                        });

            resultCount = (uint)wants.Count();

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
            if (_wantlistService.Count() > (_currentPage) * _pageSize)
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
