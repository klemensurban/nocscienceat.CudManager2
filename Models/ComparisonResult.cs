using System.Collections.Generic;

namespace nocscienceat.CudManager2.Models;

public abstract record ComparisonResult<TSync2Item>
{
    private ComparisonResult() {}

    public sealed record DiffersBy : ComparisonResult<TSync2Item>
    {
        public List<string> Properties { get; init; } = new(2);
        public TSync2Item SyncItemUpdated { get; init; } = default!;
    }

    public sealed record IsEqual : ComparisonResult<TSync2Item>;
}