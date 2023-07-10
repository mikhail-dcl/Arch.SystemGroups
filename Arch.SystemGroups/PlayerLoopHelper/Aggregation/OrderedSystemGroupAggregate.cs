using System.Collections.Generic;

namespace Arch.SystemGroups;

/// <summary>
/// Executes system groups in a specific order
/// </summary>
public class OrderedSystemGroupAggregate<T> : ISystemGroupAggregate<T>
{
    private readonly SortedList<T, SystemGroup> _sortedList;

    /// <summary>
    /// Creates a new instance of OrderedSystemGroupAggregate with the specified comparer
    /// </summary>
    /// <param name="comparer">Comparer should never return 0 for different system groups as it is forbidden by <see cref="SortedList{TKey,TValue}"/>.
    /// Specify "debounceEqualValues" to force it</param>
    /// <param name="debounceEqualValues">If True overrides the behaviour of comparer in a way that it never returns 0</param>
    /// <param name="initialCapacity">Initial capacity of the underlying collection</param>
    public OrderedSystemGroupAggregate(IComparer<T> comparer, bool debounceEqualValues = false, int initialCapacity = 16)
    {
        if (debounceEqualValues)
            comparer = new DebouncedComparer<T>(comparer);

        _sortedList = new SortedList<T, SystemGroup>(initialCapacity, comparer);
    }
    
    /// <summary>
    /// <inheritdoc cref="ISystemGroupAggregate.Count"/>
    /// </summary>
    public int Count => _sortedList.Count;

    internal IList<SystemGroup> Values => _sortedList.Values;

    /// <summary>
    /// <inheritdoc cref="ISystemGroupAggregate.TriggerUpdate"/>
    /// </summary>
    public void TriggerUpdate()
    {
        for (var i = 0; i < _sortedList.Values.Count; i++)
        {
            var systemGroup = _sortedList.Values[i];
            systemGroup.Update();
        }
    }

    /// <summary>
    /// <inheritdoc cref="ISystemGroupAggregate{T}.Add"/>
    /// </summary>
    public void Add(in T data, SystemGroup systemGroup)
    {
        _sortedList.Add(data, systemGroup);
    }

    /// <summary>
    /// <inheritdoc cref="ISystemGroupAggregate.Remove"/>
    /// </summary>
    public void Remove(SystemGroup systemGroup)
    {
        var index = _sortedList.Values.IndexOf(systemGroup);
        if (index > -1)
            _sortedList.RemoveAt(index);
    }
}