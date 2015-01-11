using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using GramophoneApp.MessageModels;
using System;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;

namespace GramophoneApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly BeeWee.DiscogsRT.IClient _discogs;
        private readonly Services.IdentityService _identityService;
        private readonly Services.WantlistService _wantlistService;
        private readonly Services.CollectionService _collectionService;

        private bool isLoggingIn;
        public bool IsLoggingIn
        {
            get { return this.isLoggingIn; }
            set { this.Set(ref this.isLoggingIn, value, "IsLoggingIn"); }
        }

        private bool isLoggedIn;
        public bool IsLoggedIn
        {
            get { return this.isLoggedIn; }
            set 
            { 
                this.Set(ref this.isLoggedIn, value, "IsLoggedIn");
                RaisePropertyChanged(() => MenuAvailable);
            }
        }

        private bool isSyncing;
        public bool IsSyncing
        {
            get { return this.isSyncing; }
            set 
            { 
                this.Set(ref this.isSyncing, value, "IsSyncing");
                RaisePropertyChanged(() => MenuAvailable);
            }
        }

        public bool MenuAvailable
        {
            get { return IsLoggedIn && !IsSyncing; }
        }

        private string _loginUrl;
        public string LoginUrl
        {
            get { return this._loginUrl; }
            set { this.Set(ref this._loginUrl, value, "LoginUrl"); }
        }

        private string _requestTokenKey;
        private string _requestTokenSecret;

        public RelayCommand LoginCommand { get; private set; }
        public RelayCommand AboutCommand { get; private set; }
        public RelayCommand SearchCommand { get; private set; }
        public RelayCommand WantlistCommand { get; private set; }
        public RelayCommand CollectionCommand { get; private set; }
        public RelayCommand SyncCommand { get; private set; }
        public RelayCommand LogoutCommand { get; private set; }

        public MainViewModel(INavigationService navigationService, BeeWee.DiscogsRT.IClient discogs, Services.IdentityService identityService, Services.WantlistService wantlistService, Services.CollectionService collectionService)
        {
            ////if (IsInDesignMode) { Code runs in Blend --> create design time data. }
            ////else { Code runs "for real" }

            _navigationService = navigationService;
            _discogs = discogs;
            _identityService = identityService;
            _wantlistService = wantlistService;
            _collectionService = collectionService;

            LoginCommand = new RelayCommand(Login);
            AboutCommand = new RelayCommand(About);
            SearchCommand = new RelayCommand(Search);
            WantlistCommand = new RelayCommand(Wantlist);
            CollectionCommand = new RelayCommand(Collection);
            SyncCommand = new RelayCommand(Sync);
            LogoutCommand = new RelayCommand(Logout);

            IsLoggingIn = false;
            IsLoggedIn = _identityService.IsLoggedIn;

            IsSyncing = true;

            _wantlistService.InitAsync();
            _collectionService.InitAsync();
            
            IsSyncing = false;
        }

        private async void Login()
        {
            var filter = new HttpBaseProtocolFilter();
            var cookieCollection = filter.CookieManager.GetCookies(new Uri("https://www.discogs.com"));
            foreach (var cookie in cookieCollection)
            {
                filter.CookieManager.DeleteCookie(cookie);
            }

            var requestToken = await _discogs.GetOAuthRequestAsync();

            if (requestToken != null && !string.IsNullOrEmpty(requestToken.Key) && !string.IsNullOrEmpty(requestToken.Secret))
            {
                _requestTokenKey = requestToken.Key;
                _requestTokenSecret = requestToken.Secret;
                LoginUrl = requestToken.Uri;
                MessengerInstance.Register<string>(this, HandlePinMessage);
                IsLoggingIn = true;
            }
        }

        private void About()
        {
            _navigationService.NavigateTo("About");
        }

        private void Search()
        {
            _navigationService.NavigateTo("Search");
        }

        private void Wantlist()
        {
            MessengerInstance.Send<WantlistMessage>(new WantlistMessage());
            _navigationService.NavigateTo("Wantlist");
        }

        private void Collection()
        {
            MessengerInstance.Send<CollectionMessage>(new CollectionMessage());
            _navigationService.NavigateTo("Collection");
        }

        private async void Sync()
        {
            await SyncLists();
        }

        private void Logout()
        {
            _identityService.Logout();
            IsLoggedIn = false;
        }

        private async void HandlePinMessage(string pin)
        {
            MessengerInstance.Unregister<string>(this, HandlePinMessage);

            if (!string.IsNullOrEmpty(_requestTokenKey) && !string.IsNullOrEmpty(_requestTokenSecret) && !string.IsNullOrEmpty(pin))
            {
                var accessToken = await _discogs.GetOAuthAccesAsync(_requestTokenKey, _requestTokenSecret, pin);

                if (accessToken != null && !string.IsNullOrEmpty(accessToken.Key) && !string.IsNullOrEmpty(accessToken.Secret))
                {
                    _identityService.Authorize(accessToken.Key, accessToken.Secret);

                    var identity = await _discogs.GetIdentityAsync(_identityService.TokenKey, _identityService.TokenSecret);

                    if (identity != null && identity.Data != null && !string.IsNullOrEmpty(identity.Data.username))
                    {
                        _identityService.Identify(identity.Data.username);
                    }

                    IsLoggedIn = true;
                    IsLoggingIn = false;

                    await SyncLists();
                }
            }

            LoginUrl = null;
        }

        private async Task SyncLists()
        {
            IsSyncing = true;

            await _wantlistService.SyncAsync(_discogs, _identityService.TokenKey, _identityService.TokenSecret, _identityService.Username);
            await _collectionService.SyncAsync(_discogs, _identityService.TokenKey, _identityService.TokenSecret, _identityService.Username);

            IsSyncing = false;
        }
    }
}
