using System;
using nocscienceat.CudManager2.Models;
using System.Collections.Generic;

namespace nocscienceat.CudManager2;

public class CudManager<TKey, TSourceItem, TSync2Item> : AbstractCudManager<TKey, TSourceItem, TSync2Item>
{
    private readonly ICudDataAdapter<TKey, TSourceItem, TSync2Item> _cudDataAdapter;
    private bool _throwOnDuplicateKeys;

    public CudManager(ICudDataAdapter<TKey, TSourceItem, TSync2Item> cudDataAdapter, IEnumerable<TSourceItem> sourceItems, IEnumerable<TSync2Item> sync2Items) : base(sourceItems, sync2Items)
    {
        _cudDataAdapter = cudDataAdapter;
    }

    public bool ThrowOnDuplicateKeys
    {
        set => _throwOnDuplicateKeys = value;
    }

    public override void CheckItems()
    {
        if (itemsChecked) return;
        foreach (TSync2Item sync2Item in Sync2Items)
        {
            TKey sync2ItemKey = _cudDataAdapter.GetKeyFromSync2Item(sync2Item);
            if (Sync2ItemsDictionary.ContainsKey(sync2ItemKey))
            {
                if (_throwOnDuplicateKeys)
                    throw new ArgumentException("Duplicate key found in sync2 items", nameof(sync2ItemKey));
                continue;
            }
            Sync2ItemsDictionary.Add(sync2ItemKey, sync2Item);
            sync2ItemsCount++;
        }
        Dictionary<TKey, bool> sourceItemKeysDictionary = new Dictionary<TKey, bool>();
        foreach (TSourceItem sourceItem in SourceItems)
        {
            TKey sourceItemKey = _cudDataAdapter.GetKeyFromSourceItem(sourceItem);
            if (sourceItemKeysDictionary.ContainsKey(sourceItemKey))
            {
                if (_throwOnDuplicateKeys)
                    throw new ArgumentException("Duplicate key found in source items", nameof(sourceItemKey));
                continue;
            }
            sourceItemKeysDictionary.Add(sourceItemKey, true);
            sourceItemsCount++;
            if (Sync2ItemsDictionary.ContainsKey(sourceItemKey))
            {
                TSync2Item syncItem = Sync2ItemsDictionary[sourceItemKey];

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

                Sync2ItemsDictionary.Remove(sourceItemKey);
            }
            else
            {
                ToCreate.Add(sourceItem);
            }
        }

        itemsChecked = true;
    }
}