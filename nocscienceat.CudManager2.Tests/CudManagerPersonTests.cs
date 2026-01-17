namespace nocscienceat.CudManager2.Tests;

public class CudManagerPersonTests
{
    [Fact]
    public void CudManager_CorrectlyIdentifiesCreateUpdateDeleteInSync()
    {
        // Arrange
        PersonTestAdapter adapter = new();
        CudManager<int, Person, Person> manager = new(adapter, PersonTestData.SourcePersons, PersonTestData.DestinationPersons);

        // Act
        var toCreate = manager.Items2Create.ToList();
        var toUpdate = manager.Items2Update.ToList();
        var inSync = manager.ItemsInSync.ToList();
        var toDelete = manager.Items2Delete.ToList();

        // Assert - Create (IDs: 19, 23)    
        Assert.Equal(2, toCreate.Count);
        Assert.Contains(toCreate, x => x is {Id: 19, Surname: "Hoffmann", GivenName: "Sarah"});
        Assert.Contains(toCreate, x => x is {Id: 23, Surname: "Klein", GivenName: "Stefan"});
    
        // Assert - In Sync (IDs: 3, 21, 17)
        Assert.Equal(3, inSync.Count);
        Assert.Contains(inSync, x => x.SourceItem.Id == 3 && x.Sync2Item.Id == 3);
        Assert.Contains(inSync, x => x.SourceItem.Id == 21 && x.Sync2Item.Id == 21);
        Assert.Contains(inSync, x => x.SourceItem.Id == 17 && x.Sync2Item.Id == 17);

        // Assert - Update (IDs: 8, 5, 14, 7, 12)
        Assert.Equal(5, toUpdate.Count);
        
        var update8 = toUpdate.Single(x => x.SourceItem.Id == 8);
        Assert.Equal("Fischer", update8.Sync2ItemUpdated.Surname);
        Assert.Equal("Thomas", update8.Sync2ItemUpdated.GivenName);
        Assert.Contains("Surname", update8.DifferingProperties);
        Assert.DoesNotContain("GivenName", update8.DifferingProperties);

        var update5 = toUpdate.Single(x => x.SourceItem.Id == 5);
        Assert.Equal("Bauer", update5.Sync2ItemUpdated.Surname);
        Assert.Equal("Michael", update5.Sync2ItemUpdated.GivenName);
        Assert.Contains("Surname", update5.DifferingProperties);
        Assert.Contains("GivenName", update5.DifferingProperties);

        var update14 = toUpdate.Single(x => x.SourceItem.Id == 14);
        Assert.Equal("Wagner", update14.Sync2ItemUpdated.Surname);
        Assert.Equal("Julia", update14.Sync2ItemUpdated.GivenName);
        Assert.DoesNotContain("Surname", update14.DifferingProperties);
        Assert.Contains("GivenName", update14.DifferingProperties);

        var update7 = toUpdate.Single(x => x.SourceItem.Id == 7);
        Assert.Equal("Koch", update7.Sync2ItemUpdated.Surname);
        Assert.Equal("Daniel", update7.Sync2ItemUpdated.GivenName);
        Assert.Contains("Surname", update7.DifferingProperties);
        Assert.Contains("GivenName", update7.DifferingProperties);

        var update12 = toUpdate.Single(x => x.SourceItem.Id == 12);
        Assert.Equal("Richter", update12.Sync2ItemUpdated.Surname);
        Assert.Equal("Laura", update12.Sync2ItemUpdated.GivenName);
        Assert.Contains("Surname", update12.DifferingProperties);
        Assert.Contains("GivenName", update12.DifferingProperties);

        // Assert - Delete (IDs: 25, 9)
        Assert.Equal(2, toDelete.Count);
        Assert.Contains(toDelete, x => x is {Id: 25, Surname: "Neumann", GivenName: "Christian"});
        Assert.Contains(toDelete, x => x is {Id: 9, Surname: "Schneider", GivenName: "Peter"});
    }
}
