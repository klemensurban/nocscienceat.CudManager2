using nocscienceat.CudManager2.Models;

namespace nocscienceat.CudManager2.Tests;

public sealed class PersonTestAdapter : ICudDataAdapter<int, Person, Person>
{
    public int GetKeyFromSourceItem(Person source) => source.Id;
    public int GetKeyFromSync2Item(Person entity) => entity.Id;

    public ComparisonResult<Person> Compare(Person source, Person entity)
    {
        var diffs = new List<string>();
        var updated = entity;

        if (source.Surname != entity.Surname)
        {
            diffs.Add(nameof(Person.Surname));
            updated = updated with { Surname = source.Surname };
        }

        if (source.GivenName != entity.GivenName)
        {
            diffs.Add(nameof(Person.GivenName));
            updated = updated with { GivenName = source.GivenName };
        }

        return diffs.Count == 0
            ? new ComparisonResult<Person>.IsEqual()
            : new ComparisonResult<Person>.DiffersBy
            {
                Properties = diffs,
                SyncItemUpdated = updated
            };
    }
}