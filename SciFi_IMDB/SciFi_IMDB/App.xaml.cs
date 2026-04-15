using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SciFi_IMDB.Services;
using SciFi_IMDB.ViewModels;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows;

namespace SciFi_IMDB
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; } = null!;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to load data: {ex.Message}",
                    "Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }

            var mainVM = ServiceProvider.GetRequiredService<MainViewModel>();
            var nav = ServiceProvider.GetRequiredService<INavigationService>() as NavigationService;
            nav.SetMainViewModel(mainVM);
            nav.NavigateTo<HomeViewModel>();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void LoadData()
        {
            using var scope = ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ImdbContext>();

            // ⭐ ACTORS — FULL LOAD (NO HYBRID)
            var actorsVM = scope.ServiceProvider.GetRequiredService<SciFiActorsViewModel>();

            var flatData = (from n in db.Names
                            where n.PrimaryProfession.Contains("actor") || n.PrimaryProfession.Contains("actress")
                            join p in db.Principals on n.NameId equals p.NameId
                            join t in db.Titles on p.TitleId equals t.TitleId
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
                    KnownForTitles = group.Select(r => r.MovieTitle).Distinct().ToList()
                })
                .ToList();

            actorsVM.Actors = new ObservableCollection<Name>(actorNames);

            // ⭐ MOVIES — HYBRID LOAD (FIRST PAGE ONLY)
            var moviesVM = scope.ServiceProvider.GetRequiredService<SciFiMoviesViewModel>();
            var firstMoviePage = LoadMoviesPage(0, 50);
            moviesVM.Movies = new ObservableCollection<Title>(firstMoviePage);

            // ⭐ SHOWS — HYBRID LOAD (FIRST PAGE ONLY)
            var showsVM = scope.ServiceProvider.GetRequiredService<SciFiShowsViewModel>();
            var firstShowPage = LoadShowsPage(0, 50);
            showsVM.Shows = new ObservableCollection<Title>(firstShowPage);
        }

        // ⭐ HYBRID LOADING METHODS
        public List<Title> LoadMoviesPage(int pageIndex, int pageSize)
        {
            using var scope = ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ImdbContext>();

            return db.Titles
                .Where(t => t.TitleType == "movie")
                .Include(t => t.Genres)
                .Where(t => t.Genres.Any(g => g.Name == "Sci-Fi"))
                .OrderBy(t => t.TitleId)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public List<Title> LoadShowsPage(int pageIndex, int pageSize)
        {
            using var scope = ServiceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ImdbContext>();

            return db.Titles
                .Where(t => t.TitleType == "tvSeries" || t.TitleType == "tvMiniSeries")
                .Include(t => t.Genres)
                .Where(t => t.Genres.Any(g => g.Name == "Sci-Fi"))
                .OrderBy(t => t.TitleId)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();
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
