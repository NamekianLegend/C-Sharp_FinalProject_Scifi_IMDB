using A2_Chinook_EFandLINQ.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace A2_Chinook_EFandLINQ.ViewModels
{
    public class OrdersViewModel : INotifyPropertyChanged
    {
        private List<Customer>? _allCustomers;
        private ObservableCollection<Customer>? _customers;
        private string _searchText = string.Empty;
        private string _selectedSort = "ID Ascending";
        private string _selectedSearch = "Order Name";

        public int TotalCustomers => _allCustomers?.Count ?? 0;
        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set
            {
                _customers = value;
                if (_allCustomers == null)
                    _allCustomers = value?.ToList();
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalCustomers));
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

        public ICommand ClearCommand => new RelayCommand(_ =>
        {
            _selectedSearch = "Order Name";
            OnPropertyChanged(nameof(SelectedSearch));
            _searchText = string.Empty;
            OnPropertyChanged(nameof(SearchText));
            _selectedSort = "ID Ascending";
            OnPropertyChanged(nameof(SelectedSort));
            Customers = new ObservableCollection<Customer>(_allCustomers ?? new());
        });


        public List<string> SearchOptions { get; } = new List<string>
        {
            "Name", "Country", "Email"
        };

        public List<string> SortOptions { get; } = new List<string>
        {
            "Name A-Z", "Name Z-A", "ID Ascending", "ID Descending", "Invoice Cost Ascending", "Invoice Cost Descending"
        };


        private void ApplyLinqSort()
        {
            // Sort the full list, then reassign Albums to update the UI
            if (_allCustomers == null) return;
            IEnumerable<Customer> sorted = _allCustomers;

            if (!string.IsNullOrWhiteSpace(_searchText))
            {
                switch (SelectedSearch)
                {
                    case "Name":
                        sorted = sorted.Where(or => or.FirstName +" "+ or.LastName != null &&
                                                   (or.FirstName +" "+ or.LastName).Contains(_searchText, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "Country":
                        sorted = sorted.Where(or => or.Country != null &&
                                                    or.Country.Contains(_searchText, StringComparison.OrdinalIgnoreCase));
                        break;
                    case "Email":
                        sorted = sorted.Where(or => or.Email != null &&
                                                    or.Email.Contains(_searchText, StringComparison.OrdinalIgnoreCase));
                        break;
                }
            }


            switch (SelectedSort)
            {
                case "Name A-Z":
                    sorted = sorted.OrderBy(a => a.FirstName+a.LastName);
                    break;
                case "Name Z-A":
                    sorted = sorted.OrderByDescending(a => a.FirstName + a.LastName);
                    break;
                case "ID Ascending":
                    sorted = sorted.OrderBy(a => a.CustomerId);
                    break;
                case "ID Descending":
                    sorted = sorted.OrderByDescending(a => a.CustomerId);
                    break;
                case "Invoice Cost Ascending":
                    sorted = sorted.OrderBy(a => a.Invoices.Sum(i => i.Total));
                    break;
                case "Invoice Cost Descending":
                    sorted = sorted.OrderByDescending(a => a.Invoices.Sum(i => i.Total));
                    break;
            }

            Customers = new ObservableCollection<Customer>(sorted);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
