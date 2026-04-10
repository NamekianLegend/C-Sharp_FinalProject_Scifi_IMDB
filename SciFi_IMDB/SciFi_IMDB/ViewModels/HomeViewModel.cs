using SciFi_IMDB.Commands;
using SciFi_IMDB.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SciFi_IMDB.ViewModels
{
    public class HomeViewModel
    {
        private readonly INavigationService _navigationService;

        public HomeViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public ICommand NavigateToMoviesCommand => new RelayCommand(_ => _navigationService.NavigateTo<SciFiMoviesViewModel>());
        public ICommand NavigateToShowsCommand => new RelayCommand(_ => _navigationService.NavigateTo<SciFiShowsViewModel>());
        public ICommand NavigateToActorsCommand => new RelayCommand(_ => _navigationService.NavigateTo<SciFiActorsViewModel>());
        public ICommand NavigateToHomeCommand => new RelayCommand(_ => _navigationService.NavigateTo<HomeViewModel>());
    }
}
