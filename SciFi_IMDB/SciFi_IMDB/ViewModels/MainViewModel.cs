using SciFi_IMDB.Commands;
using SciFi_IMDB.Services;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SciFi_IMDB.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object _currentViewModel;

        public object CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }
        private readonly INavigationService _navigationService;

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        //Command objects for navigation
        public ICommand NavigateToHomeCommand => new RelayCommand(_ => _navigationService.NavigateTo<HomeViewModel>());
        public ICommand NavigateToMoviesCommand => new RelayCommand(_ => _navigationService.NavigateTo<SciFiMoviesViewModel>());
        public ICommand NavigateToShowsCommand => new RelayCommand(_ => _navigationService.NavigateTo<SciFiShowsViewModel>());
        public ICommand NavigateToActorsCommand => new RelayCommand(_ => _navigationService.NavigateTo<SciFiActorsViewModel>());
        public ICommand NavigateBackCommand => new RelayCommand(_ => _navigationService.GoBack());
        public ICommand ExitCommand => new RelayCommand(_ => Application.Current.Shutdown());


        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
