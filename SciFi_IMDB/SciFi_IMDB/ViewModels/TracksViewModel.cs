using A2_Chinook_EFandLINQ.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace A2_Chinook_EFandLINQ.ViewModels
{
    public class TracksViewModel : INotifyPropertyChanged
    {
        private List<Track>? _allTracks;
        private ObservableCollection<Track>? _tracks;
        private string _searchText = string.Empty;
        private string _selectedSort = "ID Ascending";
        private string _selectedSearch = "Track Name";

        public int TotalTracks => _allTracks?.Count ?? 0;

        public ObservableCollection<Track>? Tracks
        {
            get => _tracks;
            set
            {
                _tracks = value;
                if (_allTracks == null)
                    _allTracks = value?.ToList();
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalTracks));
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
            _selectedSort = string.Empty;
            OnPropertyChanged(nameof(SelectedSort));
            _selectedSearch = "Track Name";
            OnPropertyChanged(nameof(SelectedSearch));
            Tracks = new ObservableCollection<Track>(_allTracks ?? new());
        });



        public List<string> SearchOptions { get; } = new List<string>
        {
            "Track Name", "Album Title", "Artist Name", "Genre Name"
        };
        public List<string> SortOptions { get; } = new List<string>
        {
            "Name A-Z", "Name Z-A", "ID Ascending", "ID Descending", "Duration Ascending", "Duration Descending"   
        };

        private void ApplyLinqSort()
        {
            // Sort the full list, then reassign Tracks so UI updates
            if (_allTracks == null) return;
            IEnumerable<Track> sorted = _allTracks;

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                switch(SelectedSearch)
                {
                    case "Track Name":
                        sorted = sorted.Where(t => t.Name != null &&
                                                   t.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "Album Title":
                        sorted = sorted.Where(t => t.Album != null &&
                                                   t.Album.Title.Contains(_searchText, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "Artist Name":
                        sorted = sorted.Where(t => t.Album != null &&
                                                   t.Album.Artist != null &&
                                                   t.Album.Artist.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "Genre Name":
                        sorted = sorted.Where(t => t.Genre != null &&
                                                   t.Genre.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase));
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
                    sorted = sorted.OrderBy(a => a.TrackId);
                    break;
                case "ID Descending":
                    sorted = sorted.OrderByDescending(a => a.TrackId);
                    break;
                case "Duration Ascending":
                    sorted = sorted.OrderBy(a => a.Milliseconds);
                    break;
                case "Duration Descending":
                    sorted = sorted.OrderByDescending(a => a.Milliseconds);
                    break;
            }

            Tracks = new ObservableCollection<Track>(sorted);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // [CallerMemberName] lets you call OnPropertyChanged() with no args
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}