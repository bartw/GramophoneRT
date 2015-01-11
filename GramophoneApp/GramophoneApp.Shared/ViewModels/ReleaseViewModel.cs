using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using GramophoneApp.MessageModels;
using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GramophoneApp.ViewModels
{
    public class ReleaseViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly BeeWee.DiscogsRT.IClient _discogs;
        private readonly Services.IdentityService _identityService;
        private readonly Services.WantlistService _wantlistService;
        private readonly Services.CollectionService _collectionService;

        private MyRelease _release;
        public MyRelease Release
        {
            get
            {
                return _release;
            }
            set
            {
                _release = value;
                RaisePropertyChanged(() => Release);
            }
        }

        private bool _inWantlist;
        public bool InWantlist 
        { 
            get
            {
                return _inWantlist;
            }
            private set
            {
                _inWantlist = value;
                RaisePropertyChanged(() => InWantlist);
            }
        }

        private int _copiesInCollection;
        public int CopiesInCollection 
        { 
            get
            {
                return _copiesInCollection;
            }
            private set
            {
                _copiesInCollection = value;
                RaisePropertyChanged(() => CopiesInCollection);
            }
        }

        public ReleaseViewModel(INavigationService navigationService, BeeWee.DiscogsRT.IClient discogs, Services.IdentityService identityService, Services.WantlistService wantlistService, Services.CollectionService collectionService)
        {
            _navigationService = navigationService;
            _discogs = discogs;
            _identityService = identityService;
            _wantlistService = wantlistService;
            _collectionService = collectionService;

            MessengerInstance.Register<ReleaseMessage>
            ( 
                 this, 
                 async (action) => await GetReleaseAsync(action) 
            );
        }

        private async Task GetReleaseAsync(ReleaseMessage message)
        {
            if (message != null)
            {
                var response = await _discogs.GetReleaseAsync(message.Id.ToString());

                if (response != null && response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var release = response.Data;

                    if (release != null)
                    {
                        Release = new MyRelease(release);

                        InWantlist = _wantlistService.InWantlist(Release.Id);
                        CopiesInCollection = _collectionService.CopiesInCollection(Release.Id);
                    }
                    else
                    {
                        InWantlist = false;
                        CopiesInCollection = 0;
                    }
                
                }
            }
        }
    }
}
