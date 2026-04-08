using A2_Chinook_EFandLINQ.ViewModels;
using System.Windows;

namespace A2_Chinook_EFandLINQ
{

    // Scaffolding Command:
    // Scaffold-DbContext -Connection "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Chinook;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False" -Provider Microsoft.EntityFrameworkCore.SqlServer -Tables Artist, Album, Track, Genre, Customer, Invoice, InvoiceLine -OutputDir Models\Generated -ContextDir Data\Generated -Namespace A2_Chinook_EFandLINQ -ContextNamespace A2_Chinook_EFandLINQ -Force

    // Image References:
    // all symbols from this website: https://fonts.google.com/icons?selected=Material+Symbols+Outlined:close:FILL@0

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = mainViewModel;
        }
    }
}