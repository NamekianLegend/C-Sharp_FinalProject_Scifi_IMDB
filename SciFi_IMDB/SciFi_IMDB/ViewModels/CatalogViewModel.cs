using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace A2_Chinook_EFandLINQ.ViewModels
{
    public class CatalogViewModel : INotifyPropertyChanged
    {
        private List<Artist> _allArtists = new();
        private char _selectedLetter = 'A';   // default so the list isn't empty on first load
        private Artist? _selectedArtist;
        private Album? _selectedAlbum;

        public ObservableCollection<char> Alphabet { get; } =
            new ObservableCollection<char>("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray());

        // Populated by App.xaml.cs after DB load, same pattern as the other ViewModels
        public List<Artist> AllArtists
        {
            get => _allArtists;
            set
            {
                _allArtists = value ?? new();
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilteredArtists));
            }
        }

        public IEnumerable<Artist> FilteredArtists =>
            _allArtists
                .Where(a => a.Name != null &&
                            a.Name.StartsWith(_selectedLetter.ToString(),
                                              StringComparison.OrdinalIgnoreCase))
                .OrderBy(a => a.Name);

        public char SelectedLetter
        {
            get => _selectedLetter;
            set
            {
                _selectedLetter = value;
                SelectedArtist = null;   // clear downstream selections
                SelectedAlbum = null;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilteredArtists));
            }
        }

        public Artist? SelectedArtist
        {
            get => _selectedArtist;
            set
            {
                _selectedArtist = value;
                SelectedAlbum = null;   // clear track panel when artist changes
                OnPropertyChanged();
            }
        }

        public Album? SelectedAlbum
        {
            get => _selectedAlbum;
            set { _selectedAlbum = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null!)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}