using System.Collections.Generic;

namespace nocscienceat.CudManager2.Models;

public abstract record ComparisonResult
{
    private ComparisonResult() {}

    public sealed record DiffersBy : ComparisonResult
    {
        public List<string> Properties { get; init; } = new(2);
    }

    public sealed record IsEqual : ComparisonResult;
}