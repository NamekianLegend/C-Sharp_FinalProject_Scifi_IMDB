using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using A2_Chinook_EFandLINQ.Services;
using A2_Chinook_EFandLINQ.ViewModels;
using System.Configuration;
using System.Windows;
using System.Collections.ObjectModel;
using A2_Chinook_EFandLINQ.Data;



namespace A2_Chinook_EFandLINQ
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
    public IServiceProvider ServiceProvider { get; private set; } = null!;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        // Load data from db — wrap in try/catch so startup still proceeds if DB access fails
        try
        {
            LoadData();
        }
        catch (System.Exception ex)
        {
            // Show a message so user can see the error and continue to the UI for troubleshooting
            MessageBox.Show($"Failed to load data: {ex.Message}\n\nCheck database connection and localdb service.", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        var mainViewModel = ServiceProvider.GetRequiredService<MainViewModel>();
        var navService = ServiceProvider.GetRequiredService<INavigationService>() as NavigationService;
        navService.SetMainViewModel(mainViewModel);
        navService.NavigateTo<HomeViewModel>();

        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private void LoadData()
    {
        //Load all data from the database, into the viewmode collections
        using (var scope = ServiceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ChinookContext>();

            var artistsViewModel = scope.ServiceProvider.GetRequiredService<ArtistsViewModel>();
            var tracksViewModel = scope.ServiceProvider.GetRequiredService<TracksViewModel>();
            var albumsViewModel = scope.ServiceProvider.GetRequiredService<AlbumsViewModel>();
            var catalogViewModel = scope.ServiceProvider.GetRequiredService<CatalogViewModel>();
            var ordersViewModel = scope.ServiceProvider.GetRequiredService<OrdersViewModel>();

                var artists = dbContext.Artists.ToList();

            // Load data from the db AND pump it directly into the the ViewModel collections
            artistsViewModel.Artists = new ObservableCollection<Artist>(artists);
            tracksViewModel.Tracks = new ObservableCollection<Track>(dbContext.Tracks
                .Include(t => t.Album).ThenInclude(a => a.Artist)
                .Include(t => t.Genre)
                .ToList()
            );
            albumsViewModel.Albums = new ObservableCollection<Album>(dbContext.Albums.ToList());
            ordersViewModel.Customers = new ObservableCollection<Customer>(dbContext.Customers
                .Include(c => c.Invoices).ThenInclude(i => i.InvoiceLines)
                .ToList());
            catalogViewModel.AllArtists = artists;


            }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Register your services and view models here
        services.AddDbContext<ChinookContext>(options =>
          options.UseSqlServer(ConfigurationManager.ConnectionStrings["ChinookConn"].ConnectionString));

        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<ArtistsViewModel>();
        services.AddSingleton<AlbumsViewModel>();
        services.AddSingleton<TracksViewModel>();
        services.AddSingleton<HomeViewModel>();
            services.AddSingleton<CatalogViewModel>();
            services.AddSingleton<OrdersViewModel>();

        ServiceProvider = services.BuildServiceProvider();
    }
    
    }

}
