using SciFi_IMDB.ViewModels;
using System.Collections.ObjectModel;

namespace SciFi_IMDB.Tests;

[TestClass]
public class ActorsViewModelTests
{
    [TestMethod]
    public void SearchText_WhenChanged_FiltersActorsByName()
    {
        // create the VM and give it some "fake" starting data
        var viewModel = new SciFiActorsViewModel();
        var fakeData = new List<Name>
        {
            new Name { PrimaryName = "Keanu Reeves" },
            new Name { PrimaryName = "Mark Hamill" },
            new Name { PrimaryName = "Carrie Fisher" }
        };

        // set the property to trigger the internal _allActors initialization
        viewModel.Actors = new ObservableCollection<Name>(fakeData);

        // simulate the user choosing "Actor Name" and typing "Keanu"
        viewModel.SelectedSearch = "Actor Name";
        viewModel.SearchText = "Keanu";

        // the collection should now only have 1 item
        Assert.HasCount(1, viewModel.Actors, "The list should have filtered down to 1 actor.");
        Assert.AreEqual("Keanu Reeves", viewModel.Actors[0].PrimaryName);
    }

    [TestMethod]
    public void ClearCommand_WhenExecuted_ResetsFilters()
    {
        // simulate some search values
        var viewModel = new SciFiActorsViewModel();
        viewModel.SearchText = "Some Search";
        viewModel.SelectedSort = "Name Z-A";

        // simulate hitting clear
        viewModel.ClearCommand.Execute(null);

        // check if properties went back to defaults
        Assert.AreEqual(string.Empty, viewModel.SearchText);
        Assert.AreEqual("ID Ascending", viewModel.SelectedSort);
        Assert.AreEqual("Actor Name", viewModel.SelectedSearch);
    }

    [TestMethod]
    public void SelectedSort_WhenChanged_RaisesPropertyChangedEvent()
    {
        var viewModel = new SciFiActorsViewModel();
        bool eventRaised = false;

        // setup the event listener
        viewModel.PropertyChanged += (sender, e) =>
        {
            if (e.PropertyName == nameof(viewModel.SelectedSort))
            {
                eventRaised = true;
            }
        };

        // simulate changing the sort
        viewModel.SelectedSort = "Name A-Z";

        // check the sort happened via eventRaised
        Assert.IsTrue(eventRaised, "The PropertyChanged event was not raised for SelectedSort.");
    }



}
