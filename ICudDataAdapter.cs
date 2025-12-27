using nocscienceat.CudManager2.Models;

namespace nocscienceat.CudManager2;

public interface ICudDataAdapter<out TKey, in TSourceItem, in TSync2Item>
{
    ComparisonResult Compare(TSourceItem sourceItem, TSync2Item sync2Item);
    TKey GetKeyFromSourceItem(TSourceItem sourceItem);
    TKey GetKeyFromSync2Item(TSync2Item sync2Item);
}