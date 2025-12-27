# nocscienceat.CudManager2

Lightweight C# utility for comparing two `IEnumerable` sequences and determining which items should be created, updated, deleted, or are already in sync. Targets .NET Standard 2.0 and uses C# 10, so it works across modern .NET runtimes.

## Features
- Single-key and multi-key comparison flows via `CudManager` and `CudManagerMultiKey`.
- Adapter-based design: plug in your own key extraction and comparison logic with `ICudDataAdapter` / `ICudDataAdapterMultiKey`.
- Clear outputs: `Items2Create`, `Items2Update`, `ItemsInSync`, and `Items2Delete`.
- Difference reporting via `ComparisonResult.DiffersBy.Properties`.

## Installation
```bash
# from your project directory
 dotnet add package nocscienceat.CudManager2
```

## Key concepts
- `ICudDataAdapter<TKey, TSourceItem, TSync2Item>`: provides key extraction for each side plus a comparison result.
- `ICudDataAdapterMultiKey<TKey, TSourceItem, TSync2Item>`: variant that can return multiple keys for a source item.
- `ComparisonResult`: return `IsEqual` when items match, or `DiffersBy` with a list of differing property names.
- `CudManager` / `CudManagerMultiKey`: orchestrate the comparison and expose the four result sets.

## Usage (single key)
```csharp
public sealed class PersonAdapter : ICudDataAdapter<int, PersonDto, PersonEntity>
{
    public int GetKeyFromSourceItem(PersonDto dto) => dto.Id;
    public int GetKeyFromSync2Item(PersonEntity entity) => entity.Id;

    public ComparisonResult Compare(PersonDto dto, PersonEntity entity)
    {
        var diffs = new List<string>();
        if (dto.Name != entity.Name) diffs.Add(nameof(PersonEntity.Name));
        if (dto.Email != entity.Email) diffs.Add(nameof(PersonEntity.Email));

        return diffs.Count == 0
            ? new ComparisonResult.IsEqual()
            : new ComparisonResult.DiffersBy { Properties = diffs };
    }
}

var manager = new CudManager<int, PersonDto, PersonEntity>(
    new PersonAdapter(),
    sourceItems: inboundDtos,
    sync2Items: existingEntities);

// Trigger comparison lazily via the exposed properties
foreach (var toCreate in manager.Items2Create) { /* insert */ }
foreach (var update in manager.Items2Update) { /* update update.Sync2Item with update.SourceItem */ }
foreach (var toDelete in manager.Items2Delete) { /* delete */ }
```



## Applying updates (abstract example)
Inside the `Items2Update` loop you receive both the source and sync-side items plus a list of differing property names. Use that to selectively update fields before persisting:
```csharp
foreach (var update in manager.Items2Update)
{
    var source = update.SourceItem;
    var target = update.Sync2Item;
    var differing = update.DifferingProperties;

    if (differing.Contains(nameof(PersonEntity.Name)))
        target.Name = source.Name;
    if (differing.Contains(nameof(PersonEntity.Email)))
        target.Email = source.Email;

    // persist the changes, e.g. via your DbContext or repository
    ...
}
```

## Notes
- Results are computed lazily when accessing the public properties; repeated access will not recompute.
- Duplicate source keys are skipped to avoid ambiguous updates.
- `Items2Delete` exposes the remaining sync-side items after matches have been removed.

