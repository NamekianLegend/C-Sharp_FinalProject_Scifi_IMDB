using SciFi_IMDB.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace SciFi_IMDB.ViewModels
{
    public class SciFiMoviesViewModel : INotifyPropertyChanged
    {
        private List<Title>? _allMovies;
        private ObservableCollection<Title>? _movies;

        private string _searchText = string.Empty;
        private string _selectedSort = "ID Ascending";
        private string _selectedSearch = "Movie Name";

        private int _pageSize = 50;
        private int _currentPage = 0;

        public int TotalMovies => _allMovies?.Count ?? 0;

        public ObservableCollection<Title> Movies
        {
            get => _movies;
            set
            {
                _movies = value;

                if (_allMovies == null)
                {
                    _allMovies = value?.ToList();
                    ApplyLinqSort();
                    UpdatePage();
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalMovies));
            }
        }

        public string SelectedSort
        {
            get => _selectedSort;
            set
            {
                _selectedSort = value;
                OnPropertyChanged();
                ApplyLinqSort();
                UpdatePage();
            }
        }

        public string SelectedSearch
        {
            get => _selectedSearch;
            set
            {
                _selectedSearch = value;
                OnPropertyChanged();
                ApplyLinqSort();
                UpdatePage();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ApplyLinqSort();
                UpdatePage();
            }
        }

        public ICommand ClearCommand => new RelayCommand(_ =>
        {
            _searchText = string.Empty;
            OnPropertyChanged(nameof(SearchText));

            _selectedSort = "ID Ascending";
            OnPropertyChanged(nameof(SelectedSort));

            _selectedSearch = "Movie Name";
            OnPropertyChanged(nameof(SelectedSearch));

            ApplyLinqSort();
            UpdatePage();
        });

        public ICommand NextPageCommand => new RelayCommand(_ =>
        {
            if (_allMovies == null) return;

            if ((_currentPage + 1) * _pageSize >= _allMovies.Count)
            {
                var nextPage = ((App)Application.Current).LoadMoviesPage(_currentPage + 1, _pageSize);

                if (nextPage.Count > 0)
                    _allMovies.AddRange(nextPage);
            }

            _currentPage++;
            UpdatePage();
        });

        public ICommand PreviousPageCommand => new RelayCommand(_ =>
        {
            if (_currentPage > 0)
            {
                _currentPage--;
                UpdatePage();
            }
        });

        public List<string> SearchOptions { get; } = new List<string>
        {
            "Movie Name"
        };

        public List<string> SortOptions { get; } = new List<string>
        {
            "Name A-Z", "Name Z-A", "ID Ascending", "ID Descending"
        };

        private void ApplyLinqSort()
        {
            if (_allMovies == null) return;

            IEnumerable<Title> sorted = _allMovies;

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                sorted = sorted.Where(a =>
                    a.PrimaryTitle != null &&
                    a.PrimaryTitle.Contains(_searchText, StringComparison.OrdinalIgnoreCase));
            }

            switch (SelectedSort)
            {
                case "Name A-Z":
                    sorted = sorted.OrderBy(a => a.PrimaryTitle);
                    break;
                case "Name Z-A":
                    sorted = sorted.OrderByDescending(a => a.PrimaryTitle);
                    break;
                case "ID Ascending":
                    sorted = sorted.OrderBy(a => a.TitleId);
                    break;
                case "ID Descending":
                    sorted = sorted.OrderByDescending(a => a.TitleId);
                    break;
            }

            _allMovies = sorted.ToList();
        }

        private void UpdatePage()
        {
            if (_allMovies == null) return;

            var page = _allMovies
                .Skip(_currentPage * _pageSize)
                .Take(_pageSize)
                .ToList();

            _movies = new ObservableCollection<Title>(page);
            OnPropertyChanged(nameof(Movies));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
