using System.Collections.Generic;

namespace nocscienceat.CudManager2;

public abstract class AbstractCudManager<TKey, TSourceItem, TSync2Item>
{
    internal readonly Dictionary<TKey, TSync2Item> Sync2ItemsDictionary;
    internal readonly List<TSourceItem> ToCreate;
    internal readonly List<ItemLinkUpdate> ToUpdate;
    internal readonly List<ItemLink> InSync;
    internal readonly IEnumerable<TSourceItem> SourceItems;
    internal readonly IEnumerable<TSync2Item> Sync2Items;
    internal int sourceItemsCount;
    internal int sync2ItemsCount;
    internal bool itemsChecked;

    protected AbstractCudManager(IEnumerable<TSourceItem> sourceItems, IEnumerable<TSync2Item> sync2Items)
    {
        SourceItems = sourceItems;
        Sync2Items = sync2Items;
        Sync2ItemsDictionary = new Dictionary<TKey, TSync2Item>(128);
        ToCreate = new List<TSourceItem>(4);
        ToUpdate = new List<ItemLinkUpdate>(8);
        InSync = new List<ItemLink>(64);
        itemsChecked = false;
        sourceItemsCount = 0;
        sync2ItemsCount = 0;
    }

        
    public class ItemLink
    {
        public TSync2Item Sync2Item { get; }
        public TSourceItem SourceItem { get; }

        public ItemLink(TSourceItem sourceItem, TSync2Item sync2Item)
        {
            Sync2Item = sync2Item;
            SourceItem = sourceItem;
        }

        public override string ToString()
        {
            return SourceItem is not null ? SourceItem.ToString() : base.ToString();
        }
    }

    public class ItemLinkUpdate : ItemLink
    {
        public List<string> DifferingProperties { get; }

        public ItemLinkUpdate(TSourceItem sourceItem, TSync2Item sync2Item, List<string> differingProperties) : base(sourceItem, sync2Item)
        {
            DifferingProperties = differingProperties;
        }
    }

    public IEnumerable<TSourceItem> Items2Create
    {
        get { if (!itemsChecked) CheckItems(); return ToCreate; }
    }

    public int Items2CreateCount
    {
        get { if (!itemsChecked) CheckItems(); return ToCreate.Count; }
    }

    public IEnumerable<ItemLinkUpdate> Items2Update
    {
        get { if (!itemsChecked) CheckItems(); return ToUpdate; }
    }

    public int Items2UpdateCount
    {
        get { if (!itemsChecked) CheckItems(); return ToUpdate.Count; }
    }

    public IEnumerable<ItemLink> ItemsInSync
    {
        get { if (!itemsChecked) CheckItems(); return InSync; }
    }

    public int ItemsInSyncCount
    {
        get { if (!itemsChecked) CheckItems(); return InSync.Count; }
    }

    public IEnumerable<TSync2Item> Items2Delete
    {
        get { if (!itemsChecked) CheckItems(); return Sync2ItemsDictionary.Values; }
    }

    public int Items2DeleteCount
    {
        get { if (!itemsChecked) CheckItems(); return Sync2ItemsDictionary.Count; }
    }

    public int SourceItemsCount
    {
        get { if (!itemsChecked) CheckItems(); return sourceItemsCount; }
    }

    public int Sync2ItemsCount
    {
        get { if (!itemsChecked) CheckItems(); return sync2ItemsCount; }
    }

    public abstract void CheckItems();
}