using A2_Chinook_EFandLINQ.Commands;
using A2_Chinook_EFandLINQ.Services;
using System.Windows.Input;

namespace A2_Chinook_EFandLINQ.ViewModels
{
    public class HomeViewModel
    {
        private readonly INavigationService _navigationService;

        public HomeViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public ICommand NavigateToArtistsCommand => new RelayCommand(_ => _navigationService.NavigateTo<ArtistsViewModel>());
        public ICommand NavigateToAlbumsCommand => new RelayCommand(_ => _navigationService.NavigateTo<AlbumsViewModel>());
        public ICommand NavigateToTracksCommand => new RelayCommand(_ => _navigationService.NavigateTo<TracksViewModel>());
        public ICommand NavigateToOrdersCommand => new RelayCommand(_ => _navigationService.NavigateTo<OrdersViewModel>());
        public ICommand NavigateToCatalogCommand => new RelayCommand(_ => _navigationService.NavigateTo<CatalogViewModel>());

    }
}
