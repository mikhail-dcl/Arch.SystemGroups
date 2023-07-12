using System;
using System.Collections.Generic;
using Arch.SystemGroups.DefaultSystemGroups;

namespace Arch.SystemGroups;

/// <summary>
/// Caches the system group aggregates by type of system group, produced for each <see cref="ISystemGroupAggregateFactory"/>
/// </summary>
internal class SystemGroupAggregateCache
{
    private readonly Dictionary<Type, ISystemGroupAggregate> _aggregates = new (SystemGroupsUtils.Count);

    internal void GetAllAggregates<TAggregate, TData>(List<TAggregate> results) where TAggregate : ISystemGroupAggregate<TData>
    {
        foreach (var aggregate in _aggregates.Values)
            results.Add((TAggregate) aggregate);
    }

    public int Count => _aggregates.Count;

    public ISystemGroupAggregate<T> Add<T>(Type systemGroupType, ISystemGroupAggregate<T>.IFactory factory)
    {
        var aggregate = factory.Create(systemGroupType);
        _aggregates.Add(systemGroupType, aggregate);
        return aggregate;
    }

    public void Remove(IPlayerLoop playerLoop, Type systemGroupType, SystemGroup systemGroup)
    {
        if (_aggregates.TryGetValue(systemGroupType, out var aggregate))
        {
            aggregate.Remove(systemGroup);
            if (aggregate.Count == 0)
            {
                _aggregates.Remove(systemGroupType);
                playerLoop.RemoveAggregate(aggregate);
            }
        }
    } 
    
    public bool TryGetValue(Type systemGroupType, out ISystemGroupAggregate aggregate) => _aggregates.TryGetValue(systemGroupType, out aggregate);
    
    public bool TryGetValue<T>(Type systemGroupType, out ISystemGroupAggregate<T> aggregate)
    {
        if (_aggregates.TryGetValue(systemGroupType, out var value))
        {
            aggregate = (ISystemGroupAggregate<T>) value;
            return true;
        }

        aggregate = default;
        return false;
    }
}