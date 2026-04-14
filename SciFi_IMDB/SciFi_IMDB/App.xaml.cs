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
            using (var scope = ServiceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ImdbContext>();

                // Actors
                var actorsViewModel = scope.ServiceProvider.GetRequiredService<SciFiActorsViewModel>();

                // as of right now I believe this loading technique is slowing down the launch of the program, I was trying a few other methods and was having trouble with them so I chose to stay with this for now
                var flatData = (from n in dbContext.Names
                                where n.PrimaryProfession.Contains("actor") || n.PrimaryProfession.Contains("actress")
                                join p in dbContext.Principals on n.NameId equals p.NameId
                                join t in dbContext.Titles on p.TitleId equals t.TitleId
                                where t.Genres.Any(g => g.Name == "Sci-Fi")
                                select new
                                {
                                    n.NameId,
                                    n.PrimaryName,
                                    n.PrimaryProfession,
                                    n.BirthYear,
                                    n.DeathYear,
                                    MovieTitle = t.PrimaryTitle
                                })
                                .Take(2500) 
                                .ToList();  

               
                var actorNames = flatData
                    .GroupBy(row => new { row.NameId, row.PrimaryName, row.PrimaryProfession, row.BirthYear, row.DeathYear })
                    .Select(group => new Name
                    {
                        NameId = group.Key.NameId,
                        PrimaryName = group.Key.PrimaryName,
                        PrimaryProfession = group.Key.PrimaryProfession,
                        BirthYear = group.Key.BirthYear,
                        DeathYear = group.Key.DeathYear,
                        KnownForTitles = group.Select(row => row.MovieTitle).Distinct().ToList()
                    })
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
