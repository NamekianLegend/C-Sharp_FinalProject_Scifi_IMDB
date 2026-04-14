using SciFi_IMDB;
using SciFi_IMDB.ViewModels;
using System.Collections.ObjectModel;

namespace SciFi_IMDB.Tests;

[TestClass]
public class ShowViewModelTests
{
    private SciFiShowsViewModel _vm;

    [TestInitialize]
    public void Setup()
    {
        _vm = new SciFiShowsViewModel();

        var mockShows = new List<Title>
        {
            new Title { TitleId = "s01", PrimaryTitle = "Stranger Things" },
            new Title { TitleId = "s02", PrimaryTitle = "The Expanse" },
            new Title { TitleId = "s03", PrimaryTitle = "Black Mirror" }
        };

        _vm.Shows = new ObservableCollection<Title>(mockShows);
    }

    [TestMethod]
    public void SearchText_FiltersShowsCorrectly()
    {
        _vm.SearchText = "Expanse";
        Assert.HasCount(1, _vm.Shows);
        Assert.AreEqual("The Expanse", _vm.Shows[0].PrimaryTitle);
    }

    [TestMethod]
    public void SearchText_EmptyString_ShowsAllResults()
    {
        _vm.SearchText = "Stranger";
        _vm.SearchText = string.Empty;
        Assert.HasCount(3, _vm.Shows);
    }

    [TestMethod]
    public void SelectedSort_NameZA_SortsDescending()
    {
        _vm.SelectedSort = "Name Z-A";
        Assert.AreEqual("The Expanse", _vm.Shows[0].PrimaryTitle);
        Assert.AreEqual("Black Mirror", _vm.Shows[2].PrimaryTitle);
    }

    [TestMethod]
    public void SelectedSort_IDAscending_SortsByTitleId()
    {
        _vm.SelectedSort = "ID Ascending";
        Assert.AreEqual("s01", _vm.Shows[0].TitleId);
        Assert.AreEqual("s03", _vm.Shows[2].TitleId);
    }

    [TestMethod]
    public void ClearCommand_ResetsPropertiesToDefaults()
    {
        _vm.SearchText = "Black";
        _vm.SelectedSort = "Name Z-A";
        _vm.SelectedSearch = "Show Name";

        _vm.ClearCommand.Execute(null);

        Assert.AreEqual(string.Empty, _vm.SearchText);
        Assert.AreEqual("ID Ascending", _vm.SelectedSort);

        Assert.AreEqual("Show Name", _vm.SelectedSearch);
    }

    [TestMethod]
    public void TotalShows_StaysConstantDuringFiltering()
    {
        _vm.SearchText = "Stranger";
        Assert.HasCount(1, _vm.Shows);       // The visible list is 1
        Assert.AreEqual(3, _vm.TotalShows);        // The total count is still 3
    }
}