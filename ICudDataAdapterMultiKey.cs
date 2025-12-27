using nocscienceat.CudManager2.Models;
using System.Collections.Generic;

namespace nocscienceat.CudManager2;

public interface ICudDataAdapterMultiKey<out TKey, in TSourceItem, in TSync2Item>
{
    ComparisonResult Compare(TSourceItem sourceItem, TSync2Item sync2Item);
    IEnumerable<TKey> GetKeysFromSourceItem(TSourceItem sourceItem);
    TKey GetKeyFromSync2Item(TSync2Item sync2Item);
}