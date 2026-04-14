using SciFi_IMDB.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
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

        public int TotalActors => _allActors?.Count ?? 0;
        public ObservableCollection<Name> Actors
        {
            get => _actors;
            set
            {
                _actors = value;
                System.Diagnostics.Debug.WriteLine($"ViewModel: Actors property set. Count: {value?.Count ?? 0}");

                if (_allActors == null)
                    _allActors = value?.ToList();
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
            _selectedSearch = "Actor Name";
            OnPropertyChanged(nameof(SelectedSearch));
            ApplyLinqSort();
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




        private void ApplyLinqSort()
        {
            // Sort the full list, then reassign Artists so UI updates
            if (_allActors == null) return;
            IEnumerable<Name> sorted = _allActors;

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                switch (SelectedSearch)
                {
                    case "Actor Name":
                        sorted = sorted.Where(a =>
                        a.PrimaryName != null &&
                        a.PrimaryName.Contains(
                            _searchText,
                            StringComparison.OrdinalIgnoreCase
                            )
                        );
                        break;

                    case "Known For Movie":
                        // Note: This assumes you added a "KnownForTitles" property to your Name.cs class!
                        sorted = sorted.Where(a =>
                        a.KnownForTitles != null &&
                        a.KnownForTitles.Any(title => title.Contains(_searchText, StringComparison.OrdinalIgnoreCase))
                        );
                        break;
                }
            }

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

            Actors = new ObservableCollection<Name>(sorted);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // [CallerMemberName] lets you call OnPropertyChanged() with no args
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
