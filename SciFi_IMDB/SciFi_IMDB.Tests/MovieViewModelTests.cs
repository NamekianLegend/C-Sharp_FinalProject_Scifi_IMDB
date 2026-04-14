using SciFi_IMDB.ViewModels;
using System.Collections.ObjectModel;

namespace SciFi_IMDB.Tests;

[TestClass]
public class MovieViewModelTests
{
    private SciFiMoviesViewModel _vm;
    private List<Title> _testData;

    [TestInitialize]
    public void Setup()
    {
        _vm = new SciFiMoviesViewModel();
        _testData = new List<Title>
        {
            new Title { TitleId = "t01", PrimaryTitle = "Inception" },
            new Title { TitleId = "t02", PrimaryTitle = "The Matrix" },
            new Title { TitleId = "t03", PrimaryTitle = "Interstellar" }
        };

        _vm.Movies = new ObservableCollection<Title>(_testData);
    }

    [TestMethod]
    public void SearchText_FiltersMoviesByTitle()
    {
        _vm.SearchText = "Matrix";

        Assert.HasCount(1, _vm.Movies);
        Assert.AreEqual("The Matrix", _vm.Movies[0].PrimaryTitle);
    }

    [TestMethod]
    public void SearchText_IsCaseInsensitive()
    {
        _vm.SearchText = "INCEPTION";

        Assert.HasCount(1, _vm.Movies);
        Assert.AreEqual("Inception", _vm.Movies[0].PrimaryTitle);
    }

    [TestMethod]
    public void SelectedSort_NameAZ_SortsAlphabetically()
    {
        _vm.SelectedSort = "Name A-Z";

        Assert.AreEqual("Inception", _vm.Movies[0].PrimaryTitle);    // I comes before T
        Assert.AreEqual("The Matrix", _vm.Movies[2].PrimaryTitle);  // T comes last
    }

    [TestMethod]
    public void SelectedSort_IDDescending_SortsCorrectly()
    {
        _vm.SelectedSort = "ID Descending";

        Assert.AreEqual("t03", _vm.Movies[0].TitleId); // Highest ID first
        Assert.AreEqual("t01", _vm.Movies[2].TitleId); // Lowest ID last
    }

    [TestMethod]
    public void TotalMovies_ReflectsOriginalListCount()
    {
        _vm.SearchText = "Inception"; // Filter it down to 1

        Assert.AreEqual(3, _vm.TotalMovies); // Total should still be 3 (the master list)
    }

    [TestMethod]
    public void ClearCommand_ResetsAllFieldsAndData()
    {
        _vm.SearchText = "Interstellar";
        _vm.SelectedSort = "Name Z-A";

        _vm.ClearCommand.Execute(null);

        Assert.AreEqual(string.Empty, _vm.SearchText);
        Assert.AreEqual("ID Ascending", _vm.SelectedSort);
        Assert.HasCount(3, _vm.Movies); // List should be full again
    }

    [TestMethod]
    public void SearchText_RaisesPropertyChangedEvent()
    {
        bool fired = false;
        _vm.PropertyChanged += (s, e) => {
            if (e.PropertyName == "SearchText") fired = true;
        };

        _vm.SearchText = "Blade Runner";

        Assert.IsTrue(fired, "PropertyChanged event was not raised for SearchText");
    }
}
