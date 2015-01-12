using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;

namespace GramophoneApp.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            /*
            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models

            }
            else
            */
            {
                // Create run time view services and models
                if (!SimpleIoc.Default.IsRegistered<INavigationService>())
                {
                    SimpleIoc.Default.Register<INavigationService, NavigationService>(true);
                }

                if (!SimpleIoc.Default.IsRegistered<BeeWee.DiscogsRT.IClient>())
                {
                    SimpleIoc.Default.Register<BeeWee.DiscogsRT.IClient>(() =>
                    {
                        return new BeeWee.DiscogsRT.Client(Credentials.UserAgent, Credentials.ConsumerKey, Credentials.ConsumerSecret);
                    });
                }

                if (!SimpleIoc.Default.IsRegistered<Services.IdentityService>())
                {
                    SimpleIoc.Default.Register<Services.IdentityService>(true);
                }

                if (!SimpleIoc.Default.IsRegistered<Services.WantlistService>())
                {
                    SimpleIoc.Default.Register<Services.WantlistService>(true);
                }

                if (!SimpleIoc.Default.IsRegistered<Services.CollectionService>())
                {
                    SimpleIoc.Default.Register<Services.CollectionService>(true);
                }

                var navigationService = ServiceLocator.Current.GetInstance<INavigationService>() as NavigationService;
                navigationService.Configure("Main", typeof(Views.MainPage));
                navigationService.Configure("Search", typeof(Views.SearchPage));
                navigationService.Configure("Wantlist", typeof(Views.WantlistPage));
                navigationService.Configure("Collection", typeof(Views.CollectionPage));
                navigationService.Configure("Release", typeof(Views.ReleasePage));
                navigationService.Configure("About", typeof(Views.AboutPage));
            }

            SimpleIoc.Default.Register<MainViewModel>(true);
            SimpleIoc.Default.Register<SearchViewModel>(true);
            SimpleIoc.Default.Register<WantlistViewModel>(true);
            SimpleIoc.Default.Register<CollectionViewModel>(true);
            SimpleIoc.Default.Register<ReleaseViewModel>(true);
            SimpleIoc.Default.Register<AboutViewModel>(true);
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public SearchViewModel Search
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SearchViewModel>();
            }
        }

        public WantlistViewModel Wantlist
        {
            get
            {
                return ServiceLocator.Current.GetInstance<WantlistViewModel>();
            }
        }

        public CollectionViewModel Collection
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CollectionViewModel>();
            }
        }

        public ReleaseViewModel Release
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ReleaseViewModel>();
            }
        }

        public AboutViewModel About
        {
            get
            {
                return ServiceLocator.Current.GetInstance<AboutViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
