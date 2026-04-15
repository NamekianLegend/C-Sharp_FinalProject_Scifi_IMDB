using SciFi_IMDB.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SciFi_IMDB.ViewModels
{
    public class SciFiActorsViewModel : INotifyPropertyChanged
    {
        private List<Name>? _allActors;
        private ObservableCollection<Name>? _actors;

        private string _searchText = string.Empty;
        private string _selectedSort = "ID Ascending";
        private string _selectedSearch = "Actor Name";

        // ⭐ Pagination fields
        private int _pageSize = 50;
        private int _currentPage = 0;

        public int TotalActors => _allActors?.Count ?? 0;

        public ObservableCollection<Name> Actors
        {
            get => _actors;
            set
            {
                _actors = value;

                if (_allActors == null)
                {
                    _allActors = value?.ToList();
                    ApplyLinqSort();
                    UpdatePage();
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalActors));
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

            _selectedSearch = "Actor Name";
            OnPropertyChanged(nameof(SelectedSearch));

            ApplyLinqSort();
            UpdatePage();
        });

        // ⭐ Pagination Commands
        public ICommand NextPageCommand => new RelayCommand(_ =>
        {
            if (_allActors == null) return;

            if ((_currentPage + 1) * _pageSize < _allActors.Count)
            {
                _currentPage++;
                UpdatePage();
            }
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
            "Actor Name",
            "Known For Movie"
        };

        public List<string> SortOptions { get; } = new List<string>
        {
            "Name A-Z", "Name Z-A", "ID Ascending", "ID Descending"
        };

        // ⭐ Apply searching + sorting
        private void ApplyLinqSort()
        {
            if (_allActors == null) return;

            IEnumerable<Name> sorted = _allActors;

            // Searching
            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                switch (SelectedSearch)
                {
                    case "Actor Name":
                        sorted = sorted.Where(a =>
                            a.PrimaryName != null &&
                            a.PrimaryName.Contains(_searchText, StringComparison.OrdinalIgnoreCase));
                        break;

                    case "Known For Movie":
                        sorted = sorted.Where(a =>
                            a.KnownForTitles != null &&
                            a.KnownForTitles.Any(title =>
                                title.Contains(_searchText, StringComparison.OrdinalIgnoreCase)));
                        break;
                }
            }

            // Sorting
            switch (SelectedSort)
            {
                case "Name A-Z":
                    sorted = sorted.OrderBy(a => a.PrimaryName);
                    break;
                case "Name Z-A":
                    sorted = sorted.OrderByDescending(a => a.PrimaryName);
                    break;
                case "ID Ascending":
                    sorted = sorted.OrderBy(a => a.NameId);
                    break;
                case "ID Descending":
                    sorted = sorted.OrderByDescending(a => a.NameId);
                    break;
            }

            _allActors = sorted.ToList();
        }

        // ⭐ Pagination logic
        private void UpdatePage()
        {
            if (_allActors == null) return;

            var page = _allActors
                .Skip(_currentPage * _pageSize)
                .Take(_pageSize)
                .ToList();

            _actors = new ObservableCollection<Name>(page);
            OnPropertyChanged(nameof(Actors));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
