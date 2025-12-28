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

## Key concepts
- `ICudDataAdapter<TKey, TSourceItem, TSync2Item>`: provides key extraction for each side plus a comparison result.
- `ICudDataAdapterMultiKey<TKey, TSourceItem, TSync2Item>`: variant that can work with multiple keys for a source item (TSync2Item, TSync2Item are linked if the key of TSync2Item matches any key of TSourceItem).
- `ComparisonResult`: return `IsEqual` when items match, or `DiffersBy` with a list of differing property names.
- `CudManager` / `CudManagerMultiKey`: orchestrate the comparison and expose the four result sets.

## Installation

dotnet add package nocscienceat.CudManager2