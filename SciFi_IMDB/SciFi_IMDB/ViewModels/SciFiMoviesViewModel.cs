using SciFi_IMDB.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
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

        public int TotalMovies => _allMovies?.Count ?? 0;
        public ObservableCollection<Title> Movies
        {
            get => _movies;
            set
            {
                _movies = value;
                System.Diagnostics.Debug.WriteLine($"ViewModel: movies property set. Count: {value?.Count ?? 0}");

                if (_allMovies == null)
                    _allMovies = value?.ToList();
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
            }
        }
        public ICommand ClearCommand => new RelayCommand(_ =>
        {
            _searchText = string.Empty;
            OnPropertyChanged(nameof(SearchText));
            _selectedSort = "ID Ascending";
            OnPropertyChanged(nameof(SelectedSort));
            _selectedSearch = "Name Name";
            OnPropertyChanged(nameof(SelectedSearch));
            ApplyLinqSort();
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
            // Sort the full list, then reassign movies so UI updates
            if (_allMovies == null) return;
            IEnumerable<Title> sorted = _allMovies;

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                switch (SelectedSearch)
                {
                    case "Movie Name":
                        sorted = sorted.Where(a =>
                        a.PrimaryTitle != null &&
                        a.PrimaryTitle.Contains(
                            _searchText,
                            StringComparison.OrdinalIgnoreCase
                            )
                        );
                        break;
                }
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

            Movies = new ObservableCollection<Title>(sorted);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // [CallerMemberName] lets you call OnPropertyChanged() with no args
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}