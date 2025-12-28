using nocscienceat.CudManager2.Models;

namespace nocscienceat.CudManager2;

public interface ICudDataAdapter<out TKey, in TSourceItem, TSync2Item>
{
    ComparisonResult<TSync2Item> Compare(TSourceItem sourceItem, TSync2Item sync2Item);
    TKey GetKeyFromSourceItem(TSourceItem sourceItem);
    TKey GetKeyFromSync2Item(TSync2Item sync2Item);
}