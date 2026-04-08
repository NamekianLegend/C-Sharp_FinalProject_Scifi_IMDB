using System.Windows;
using SciFi_IMDB.ViewModels;

namespace SciFi_IMDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Scaffolding command:
    /// Scaffold-DbContext -Connection "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=IMDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"` -Provider Microsoft.EntityFrameworkCore.SqlServer ` -Tables Titles, Genres, Title_Genres, Names, Principals, Ratings ` -OutputDir Models\Generated ` -ContextDir Data\Generated ` -Namespace SciFi_IMDB ` -ContextNamespace SciFi_IMDB ` -Force
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}

// Astronaut image: Image by <a href="https://pixabay.com/users/archipix-36936035/?utm_source=link-attribution&utm_medium=referral&utm_campaign=image&utm_content=8184040">ArchiPix</a> from <a href="https://pixabay.com//?utm_source=link-attribution&utm_medium=referral&utm_campaign=image&utm_content=8184040">Pixabay</a>
// Movie icon image: <a href="https://www.flaticon.com/free-icons/movies" title="movies icons">Movies icons created by Freepik - Flaticon</a>
// Home icon image: <a href="https://www.flaticon.com/free-icons/home" title="home icons">Home icons created by Vectors Market - Flaticon</a>
// Actor icon image: <a href="https://www.flaticon.com/free-icons/actor" title="actor icons">Actor icons created by Freepik - Flaticon</a>
// TV show icon image: <a href="https://www.flaticon.com/free-icons/tv" title="tv icons">Tv icons created by Freepik - Flaticon</a>