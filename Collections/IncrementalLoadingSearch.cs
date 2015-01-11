using BeeWee.DiscogsRT;
using BeeWee.DiscogsRT.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Collections
{
    public class IncrementalLoadingSearch : ObservableCollection<Result>, ISupportIncrementalLoading
    {
        private readonly IClient _discogs;
        private readonly string _tokenKey;
        private readonly string _tokenSecret;

        private bool _hasMoreItems;
        public bool HasMoreItems
        {
            get
            {
                return _hasMoreItems;
            }
        }

        private int _currentPage;

        private SearchQuery _query;

        public IncrementalLoadingSearch(string tokenKey, string tokenSecret, IClient discogs)
        {
            _discogs = discogs;
            _tokenKey = tokenKey;
            _tokenSecret = tokenSecret;
            Clear();
        }

        public new void Clear()
        {
            _hasMoreItems = false;
            _currentPage = 0;
            _query = null;
            base.Clear();
        }

        public async void Search(SearchQuery query)
        {
            _query = query;
            await GetPagedItems(Window.Current.Dispatcher);
        }

        public async Task<uint> GetPagedItems(CoreDispatcher dispatcher)
        {
            uint resultCount = 0;

            if (_query != null)
            {
                _currentPage += 1;

                var response = await _discogs.Search(_tokenKey, _tokenSecret, _query, 50, _currentPage);

                if (response != null && response.Data != null)
                {
                    if (response.Data.pagination != null && response.Data.pagination.pages > _currentPage)
                    {
                        _hasMoreItems = true;
                    }
                    else
                    {
                        _hasMoreItems = false;
                    }

                    if (response.Data.results != null && response.Data.results.Count > 0)
                    {
                        resultCount = (uint)response.Data.results.Count;

                        await dispatcher.RunAsync(
                            CoreDispatcherPriority.Normal,
                            () =>
                            {
                                foreach (Result result in response.Data.results)
                                {
                                    Add(result);
                                }
                            });


                    }
                }
                else
                {
                    _hasMoreItems = false;
                }
            }
            else
            {
                _hasMoreItems = false;
            }

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
    }
}
