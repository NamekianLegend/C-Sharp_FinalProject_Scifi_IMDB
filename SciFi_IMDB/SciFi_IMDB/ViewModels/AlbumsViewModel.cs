using A2_Chinook_EFandLINQ.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace A2_Chinook_EFandLINQ.ViewModels
{
    public class AlbumsViewModel : INotifyPropertyChanged
    {
        private List<Album>? _allAlbums;
        private ObservableCollection<Album>? _albums;
        private string _searchText = string.Empty;
        private string _selectedSort = "ID Ascending";
        private string _selectedSearch = "Album Title";

        public int TotalAlbums => _allAlbums?.Count ?? 0;

        public ObservableCollection<Album> Albums
        {
            get => _albums;
            set
            {
                _albums = value;
                if (_allAlbums == null)
                    _allAlbums = value?.ToList();
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalAlbums));
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
            _selectedSearch = "Album Title";
            OnPropertyChanged(nameof(SelectedSearch));
            _searchText = string.Empty;
            OnPropertyChanged(nameof(SearchText));
            _selectedSort = "ID Ascending";
            OnPropertyChanged(nameof(SelectedSort));
            Albums = new ObservableCollection<Album>(_allAlbums ?? new());
        });

        public List<string> SearchOptions { get; } = new List<string>
        {
            "Album Title", "Artist Name", "Track Name"
        };

        public List<string> SortOptions { get; } = new List<string>
        {
            "Name A-Z", "Name Z-A", "ID Ascending", "ID Descending"
        };

        private void ApplyLinqSort()
        {
            // Sort the full list, then reassign Albums to update the UI
            if (_allAlbums == null) return;
            IEnumerable<Album> sorted = _allAlbums;

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                switch (SelectedSearch)
                {
                    case "Album Title":
                        sorted = sorted.Where(al => al.Title != null &&
                                                   al.Title.Contains(_searchText, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "Artist Name":
                        sorted = sorted.Where(al => al.Artist != null &&
                                                    al.Artist.Name != null &&
                                                    al.Artist.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "Track Name":
                        sorted = sorted.Where(al => al.Tracks != null &&
                                                    al.Tracks.Any(t => t.Name != null &&
                                                                        t.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase)));
                        break;
                }
            }


            switch (SelectedSort)
            {
                case "Name A-Z":
                    sorted = sorted.OrderBy(a => a.Title);
                    break;
                case "Name Z-A":
                    sorted = sorted.OrderByDescending(a => a.Title);
                    break;
                case "ID Ascending":
                    sorted = sorted.OrderBy(a => a.AlbumId);
                    break;
                case "ID Descending":
                    sorted = sorted.OrderByDescending(a => a.AlbumId);
                    break;
            }

            Albums = new ObservableCollection<Album>(sorted);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
