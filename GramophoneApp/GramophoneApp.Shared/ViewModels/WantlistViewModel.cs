using Collections;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using GramophoneApp.MessageModels;
using Models;
using System;

namespace GramophoneApp.ViewModels
{
    public class WantlistViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly BeeWee.DiscogsRT.IClient _discogs;
        private readonly Services.WantlistService _wantlistService;

        private IncrementalLoadingWantlist _wantlist;
        public IncrementalLoadingWantlist Wantlist
        {
            get
            {
                return _wantlist;
            }
            private set
            {
                _wantlist = value;
                RaisePropertyChanged(() => Wantlist);
            }
        }

        private MyWant _selectedWant;
        public MyWant SelectedWant
        {
            get
            {
                return _selectedWant;
            }
            set
            {
                _selectedWant = value;
                RaisePropertyChanged(() => SelectedWant);
            }
        }

        public WantlistViewModel(INavigationService navigationService, BeeWee.DiscogsRT.IClient discogs, Services.WantlistService wantlistService)
        {
            _navigationService = navigationService;
            _discogs = discogs;
            _wantlistService = wantlistService;

            Wantlist = new IncrementalLoadingWantlist(_wantlistService);

            this.PropertyChanged += WantlistViewModel_PropertyChanged;

            MessengerInstance.Register<WantlistMessage>
            (
                 this,
                 async (action) => await Wantlist.LoadMoreItemsAsync(20)
            );
        }

        private void WantlistViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedWant")
            {
                if (SelectedWant != null)
                {
                    MessengerInstance.Send<ReleaseMessage>(new ReleaseMessage() { Id = SelectedWant.Id });
                    _navigationService.NavigateTo("Release");
                    SelectedWant = null;
                }
            }
        }
    }
}
