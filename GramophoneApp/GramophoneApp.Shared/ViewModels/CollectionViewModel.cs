using Collections;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using GramophoneApp.MessageModels;
using Models;
using System;

namespace GramophoneApp.ViewModels
{
    public class CollectionViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly BeeWee.DiscogsRT.IClient _discogs;
        private readonly Services.CollectionService _collectionService;

        private IncrementalLoadingCollection _collection;
        public IncrementalLoadingCollection Collection
        {
            get
            {
                return _collection;
            }
            private set
            {
                _collection = value;
                RaisePropertyChanged(() => Collection);
            }
        }

        private MyCollectionItem _selectedCollectionItem;
        public MyCollectionItem SelectedCollectionItem
        {
            get
            {
                return _selectedCollectionItem;
            }
            set
            {
                _selectedCollectionItem = value;
                RaisePropertyChanged(() => SelectedCollectionItem);
            }
        }

        public CollectionViewModel(INavigationService navigationService, BeeWee.DiscogsRT.IClient discogs, Services.CollectionService collectionService)
        {
            _navigationService = navigationService;
            _discogs = discogs;
            _collectionService = collectionService;

            Collection = new IncrementalLoadingCollection(_collectionService);

            this.PropertyChanged += CollectionViewModel_PropertyChanged;

            MessengerInstance.Register<CollectionMessage>
            (
                 this,
                 async (action) => await Collection.LoadMoreItemsAsync(20)
            );
        }

        private void CollectionViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedCollectionItem")
            {
                if (SelectedCollectionItem != null)
                {
                    MessengerInstance.Send<ReleaseMessage>(new ReleaseMessage() { Id = SelectedCollectionItem.Id });
                    _navigationService.NavigateTo("Release");
                    SelectedCollectionItem = null;
                }
            }
        }
    }
}
