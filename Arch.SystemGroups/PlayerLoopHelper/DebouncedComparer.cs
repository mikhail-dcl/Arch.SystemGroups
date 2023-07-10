using System.Collections.Generic;

namespace Arch.SystemGroups;

internal class DebouncedComparer<T> : IComparer<T>
{
    private readonly IComparer<T> _inner;

    public DebouncedComparer(IComparer<T> inner)
    {
        _inner = inner;
    }
    
    public int Compare(T x, T y)
    {
        var result = _inner.Compare(x, y);
        return result == 0 ? 1 : result;
    }
}