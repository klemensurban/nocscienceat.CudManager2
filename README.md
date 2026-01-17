# nocscienceat.CudManager2

Lightweight C# utility for comparing two `IEnumerable` sequences and determining which items should be **created**, **updated**, **deleted**, or are already **in sync**. Targets **.NET Standard 2.0** and uses **C# 10**.

## Features
- Single-key and multi-key comparison flows via `CudManager` and `CudManagerMultiKey`.
- Adapter-based design: plug in your own key extraction and comparison logic with:
  - `ICudDataAdapter<TKey, TSourceItem, TSync2Item>`
  - `ICudDataAdapterMultiKey<TKey, TSourceItem, TSync2Item>`
- Clear outputs:
  - `Items2Create`
  - `Items2Update`
  - `ItemsInSync`
  - `Items2Delete`
- Difference reporting via `ComparisonResult<TSync2Item>.DiffersBy.Properties`.
- Update projection via `ComparisonResult<TSync2Item>.DiffersBy.SyncItemUpdated` and `AbstractCudManager.ItemLinkUpdate.Sync2ItemUpdated`.
- Duplicate key handling with configurable behavior via `ThrowOnDuplicateKeys`.

## Key concepts
- `ICudDataAdapter<TKey, TSourceItem, TSync2Item>`: provides key extraction for each side plus a comparison result.
- `ICudDataAdapterMultiKey<TKey, TSourceItem, TSync2Item>`: variant that can work with multiple keys for a source item (TSync2Item, TSync2Item are linked if the key of TSync2Item matches any key of TSourceItem).
- `ComparisonResult`: return `IsEqual` when items match, or `DiffersBy` with a list of differing property names.
- `CudManager` / `CudManagerMultiKey`: orchestrate the comparison and expose the four result sets.

## Installation

dotnet add package nocscienceat.CudManager2

## Usage (single key) 
```csharp
// implementig the ICudDataAdapter for your types
public sealed class PersonAdapter : ICudDataAdapter<int, PersonDto, PersonEntity>
{
    public int GetKeyFromSourceItem(PersonDto dto) => dto.Id;
    public int GetKeyFromSync2Item(PersonEntity entity) => entity.Id;

    public ComparisonResult Compare<PersonEntity>(PersonDto dto, PersonEntity entity)
    {
        var diffs = new List<string>();
        PersonEntity sync2ItemUpdated = new(); // PersonEntity sync2ItemUpdated = new(entity);  // if a copy constructor is available

        if (dto.Name != entity.Name) 
        {
            diffs.Add(nameof(PersonEntity.Name));
            sync2ItemUpdated.Name = dto.Name;
        }

        if (dto.Email != entity.Email)
        {
            diffs.Add(nameof(PersonEntity.Email));
            sync2ItemUpdated.Email = dto.Email;
        }

        return diffs.Count == 0
            ? new ComparisonResult.IsEqual()
            : new ComparisonResult.DiffersBy { Properties = diffs, SyncItemUpdated = sync2ItemUpdated };
    }
}

// code from the 'synchronization' context

var inboundDtos = _personDtoService.GetAll(); // IEnumerable<PersonDto>
var existingEntities = _personEntityService.GetAll(); // IEnumerable<PersonEntity>

var manager = new CudManager<int, PersonDto, PersonEntity>(
    new PersonAdapter(),
    sourceItems: inboundDtos,
    sync2Items: existingEntities);

// Trigger comparison lazily via the exposed properties
foreach (var toCreate in manager.Items2Create) { /* insert */ }
foreach (var update in manager.Items2Update) { /* update update.Sync2Item with update.SourceItem */ }
foreach (var toDelete in manager.Items2Delete) { /* delete */ }
```


## Applying updates

```csharp
foreach (var update in manager.Items2Update)
{
    // Since the assumed PersonEntityService usually only works with PersonEntity instances (it knows nothing about PersonDto), 
    // we call its hypothetical update method with the original PersonEntity item and the PersonEntity item with the updated Properties as well as the list of differing properties.

    _personEntityService.Update(update.Sync2Item, update.Sync2ItemUpdated, update.DifferingProperties);
    ...
}
```

## Duplicate Key Handling

The `CudManager<TKey, TSourceItem, TSync2Item>` class provides a `ThrowOnDuplicateKeys` property to control how duplicate keys are handled during comparison:

### Default Behavior (`ThrowOnDuplicateKeys = false`)
```csharp
var manager = new CudManager<int, PersonDto, PersonEntity>(
    new PersonAdapter(),
    sourceItems: inboundDtos,
    sync2Items: existingEntities);

// Duplicate keys are silently ignored (subsequent items with the same key are skipped)
```
- **Source items**: Duplicate keys are skipped; only the first item with each key is processed
- **Sync2 items**: Duplicate keys are skipped; only the first item with each key is processed
- **No exception** is thrown; processing continues normally

### Strict Mode (`ThrowOnDuplicateKeys = true`)
```csharp
var manager = new CudManager<int, PersonDto, PersonEntity>(
    new PersonAdapter(),
    sourceItems: inboundDtos,
    sync2Items: existingEntities);

manager.ThrowOnDuplicateKeys = true;

// An ArgumentException is thrown if duplicates are detected
foreach (var toCreate in manager.Items2Create) { /* ... */ }
```
- **Source items**: `ArgumentException` thrown if a duplicate key is found
- **Sync2 items**: `ArgumentException` thrown if a duplicate key is found
- Useful for validating data integrity before synchronization

### Example
```csharp
// Data with duplicate key (ID = 3)
var sourceItems = new List<PersonDto>
{
    new PersonDto { Id = 1, Name = "Alice" },
    new PersonDto { Id = 3, Name = "Bob" },
    new PersonDto { Id = 3, Name = "Bobby" }  // Duplicate!
};

var manager = new CudManager<int, PersonDto, PersonEntity>(
    new PersonAdapter(),
    sourceItems: sourceItems,
    sync2Items: existingEntities);

// Silent handling (default)
manager.ThrowOnDuplicateKeys = false;
var toCreate = manager.Items2Create.ToList();  // Only first item with ID=3 is processed

// Strict mode
manager.ThrowOnDuplicateKeys = true;
var toCreate = manager.Items2Create.ToList();  // Throws ArgumentException: "Duplicate key found in source items"
```

## Notes
- Results are computed lazily when accessing the public properties; repeated access will not recompute.
- Duplicate source keys are skipped by default to avoid ambiguous updates; enable `ThrowOnDuplicateKeys` to detect data quality issues.
- `CudManagerMultiKey` handles multiple keys per source item and always skips duplicates without exception support.

