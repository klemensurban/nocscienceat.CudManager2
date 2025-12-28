using nocscienceat.CudManager2.Models;
using System.Collections.Generic;

namespace nocscienceat.CudManager2;

public class CudManagerMultiKey<TKey, TSourceItem, TSync2Item> : AbstractCudManager<TKey, TSourceItem, TSync2Item>
{
    private readonly ICudDataAdapterMultiKey<TKey, TSourceItem, TSync2Item> _cudDataAdapter;

    public CudManagerMultiKey(ICudDataAdapterMultiKey<TKey, TSourceItem, TSync2Item> cudDataAdapter, IEnumerable<TSourceItem> sourceItems, IEnumerable<TSync2Item> sync2Items) : base(sourceItems, sync2Items)
    {
        _cudDataAdapter = cudDataAdapter;
    }

    public override void CheckItems()
    {
        if (itemsChecked) return;
        foreach (TSync2Item sync2Item in Sync2Items)
        {
            TKey sync2ItemKey = _cudDataAdapter.GetKeyFromSync2Item(sync2Item);
            if (!Sync2ItemsDictionary.ContainsKey(sync2ItemKey))
            {
                Sync2ItemsDictionary.Add(sync2ItemKey, sync2Item);
                sync2ItemsCount++;
            }
        }

        Dictionary<TKey, bool> sourceItemKeysDictionary = new Dictionary<TKey, bool>();
        foreach (TSourceItem sourceItem in SourceItems)
        {
            TKey? syncItemKey = default;
            bool syncItemKeyFound = false;
            IEnumerable<TKey> sourceItemKeys = _cudDataAdapter.GetKeysFromSourceItem(sourceItem);
            bool duplicateSourceItemKey = false;
            foreach (TKey sourceItemKey in sourceItemKeys)
            {
                if (sourceItemKeysDictionary.ContainsKey(sourceItemKey))
                {
                    duplicateSourceItemKey = true;
                    break;
                }
                sourceItemKeysDictionary.Add(sourceItemKey, true);
                if (Sync2ItemsDictionary.ContainsKey(sourceItemKey))
                {
                    syncItemKey = sourceItemKey;
                    syncItemKeyFound = true;
                }
            }
            if (duplicateSourceItemKey)
                continue;
            sourceItemsCount++;
            if (syncItemKeyFound)
            {
                TSync2Item syncItem = Sync2ItemsDictionary[syncItemKey!];

                ComparisonResult<TSync2Item> comparison = _cudDataAdapter.Compare(sourceItem, syncItem);
                switch (comparison)
                {
                    case ComparisonResult<TSync2Item>.IsEqual:
                        InSync.Add(new ItemLink(sourceItem, syncItem));
                        break;

                    case ComparisonResult<TSync2Item>.DiffersBy differsBy:
                        ToUpdate.Add(new ItemLinkUpdate(sourceItem, syncItem, differsBy.SyncItemUpdated, differsBy.Properties));
                        break;
                }

                Sync2ItemsDictionary.Remove(syncItemKey!);
            }
            else
            {
                ToCreate.Add(sourceItem);
            }
        }
        itemsChecked = true;
    }
}