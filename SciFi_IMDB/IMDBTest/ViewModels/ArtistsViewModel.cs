using A2_Chinook_EFandLINQ.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace A2_Chinook_EFandLINQ.ViewModels
{
    public class ArtistsViewModel : INotifyPropertyChanged
    {
        private List<Artist>? _allArtists;
        private ObservableCollection<Artist>? _artists;
        private string _searchText = string.Empty;
        private string _selectedSort = "ID Ascending";
        private string _selectedSearch = "Artist Name";

        public int TotalArtists => _allArtists?.Count ?? 0;
        public ObservableCollection<Artist> Artists
        {
            get => _artists;
            set
            {
                _artists = value;
                if (_allArtists == null)
                    _allArtists = value?.ToList();
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalArtists));
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
            _selectedSearch = "Artist Name";
            OnPropertyChanged(nameof(SelectedSearch));
            ApplyLinqSort();
        });


        public List<string> SearchOptions { get; } = new List<string>
        {
            "Artist Name", "Album Title"
        };
        public List<string> SortOptions { get; } = new List<string>
        {
            "Name A-Z", "Name Z-A", "ID Ascending", "ID Descending", "Album Count Ascending", "Album Count Descending", "Track Count Ascending", "Track Count Descending"
        };

        private void ApplyLinqSort()
        {
            // Sort the full list, then reassign Artists so UI updates
            if (_allArtists == null) return;
            IEnumerable<Artist> sorted = _allArtists;

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                switch (SelectedSearch)
                {
                    case "Artist Name":
                        sorted = sorted.Where(a => 
                        a.Name != null &&
                        a.Name.Contains(
                            _searchText, 
                            StringComparison.OrdinalIgnoreCase
                            )
                        );
                        break;
                    case "Album Title":
                        sorted = sorted.Where(a => 
                            a.Albums != null &&
                            a.Albums.Any(al => 
                                al.Title != null &&
                                al.Title.Contains(
                                    _searchText, 
                                    StringComparison.OrdinalIgnoreCase
                                    )
                                )
                            );
                        break;
                }
            }

            switch (SelectedSort)
            {
                case "Name A-Z":
                    sorted = sorted.OrderBy(a => a.Name);
                    break;
                case "Name Z-A":
                    sorted = sorted.OrderByDescending(a => a.Name);
                    break;
                case "ID Ascending":
                    sorted = sorted.OrderBy(a => a.ArtistId);
                    break;
                case "ID Descending":
                    sorted = sorted.OrderByDescending(a => a.ArtistId);
                    break;
                case "Album Count Ascending":
                    sorted = sorted.OrderBy(a => a.Albums?.Count ?? 0);
                    break;
                case "Album Count Descending":
                    sorted = sorted.OrderByDescending(a => a.Albums?.Count ?? 0);
                    break;
                case "Track Count Ascending":
                    sorted = sorted.OrderBy(a => a.TrackCount);
                    break;
                case "Track Count Descending":
                    sorted = sorted.OrderByDescending(a => a.TrackCount);
                    break;
            }

            Artists = new ObservableCollection<Artist>(sorted);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // [CallerMemberName] lets you call OnPropertyChanged() with no args
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}