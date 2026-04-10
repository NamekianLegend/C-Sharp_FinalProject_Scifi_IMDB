using Microsoft.EntityFrameworkCore;
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

                var mainViewModel = ServiceProvider.GetRequiredService<MainViewModel>();
                var navService = ServiceProvider.GetRequiredService<INavigationService>() as NavigationService;
                navService.SetMainViewModel(mainViewModel);
                navService.NavigateTo<HomeViewModel>();

                var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();

        }


        private void ConfigureServices(IServiceCollection services)
        {

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
