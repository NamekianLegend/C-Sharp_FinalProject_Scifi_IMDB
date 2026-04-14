using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using SciFi_IMDB.Services;
using SciFi_IMDB.ViewModels;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls.Primitives;



namespace SciFi_IMDB
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
            // Load all data from the database, into the viewmodel collecitons
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ImdbContext>();

                // Actors
                var actorsViewModel = scope.ServiceProvider.GetRequiredService<SciFiActorsViewModel>();
                var actorNames = dbContext.Names
                    .Where(n => n.PrimaryProfession.Contains("actor") || n.PrimaryProfession.Contains("actress"))
                    .Take(500)
                    .ToList();

                actorsViewModel.Actors = new ObservableCollection<Name>(actorNames);


                

                // Movies
                var moviesViewModel = scope.ServiceProvider.GetRequiredService<SciFiMoviesViewModel>();
                var movies = dbContext.Titles
                    .Where(t => t.TitleType == "movie")
                    .Include(t => t.Genres)
                    .Where(t => t.Genres.Any(g => g.Name == "Sci-Fi"))
                    .Take(500)
                    .ToList();

                moviesViewModel.Movies = new ObservableCollection<Title>(movies);

                // TV Shows
                var showsViewModel = scope.ServiceProvider.GetRequiredService<SciFiShowsViewModel>();
                var shows = dbContext.Titles
                    .Where(t => t.TitleType == "tvSeries" || t.TitleType == "tvMiniSeries")
                    .Include(t => t.Genres)
                    .Where(t => t.Genres.Any(g => g.Name == "Sci-Fi"))
                    .Take(500)
                    .ToList();

                showsViewModel.Shows = new ObservableCollection<Title>(shows);
            }
        }


        private void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ImdbContext>(options =>
                 options.UseSqlServer(ConfigurationManager.ConnectionStrings["IMDBConn"].ConnectionString));

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<SciFiActorsViewModel>();
            services.AddSingleton<SciFiMoviesViewModel>();
            services.AddSingleton<SciFiShowsViewModel>();
            services.AddSingleton<HomeViewModel>();

            ServiceProvider = services.BuildServiceProvider();
        }

    }

}
