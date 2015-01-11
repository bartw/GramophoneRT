using BeeWee.DiscogsRT.Models;
using Collections;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;

namespace GramophoneApp.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly BeeWee.DiscogsRT.IClient _discogs;
        private readonly Services.IdentityService _identityService;

        private string _query;
        public string Query
        {
            get
            {
                return _query;
            }
            set
            {
                _query = value;
                RaisePropertyChanged(() => Query);
            }
        }

        private IncrementalLoadingSearch _results;
        public IncrementalLoadingSearch Results
        {
            get
            {
                return _results;
            }
            private set
            {
                _results = value;
                RaisePropertyChanged(() => Results);
            }
        }

        private Result _selectedResult;
        public Result SelectedResult
        {
            get
            {
                return _selectedResult;
            }
            set
            {
                _selectedResult = value;
                RaisePropertyChanged(() => SelectedResult);
            }
        }

        public RelayCommand SearchCommand { get; private set; }

        public SearchViewModel(INavigationService navigationService, BeeWee.DiscogsRT.IClient discogs, Services.IdentityService identityService)
        {
            _navigationService = navigationService;
            _discogs = discogs;
            _identityService = identityService;

            Results = new IncrementalLoadingSearch(_identityService.TokenKey, _identityService.TokenSecret, _discogs);

            this.PropertyChanged += SearchViewModel_PropertyChanged;

            SearchCommand = new RelayCommand(Search);
        }

        private void SearchViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedResult")
            {
                if (SelectedResult != null)
                {
                    //_navigationService.Navigate<ReleasePage>(SelectedResult.id);
                    SelectedResult = null;
                }
            }
        }

        private void Search()
        {
            Results.Clear();

            if (!string.IsNullOrEmpty(Query))
            {
                SearchQuery searchQuery = new SearchQuery();

                searchQuery.Query = Query;
                searchQuery.Type = SearchItemType.Release;

                Results.Search(searchQuery);
            }
        }
    }
}
