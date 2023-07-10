using System;
using System.Collections.Generic;

namespace Arch.SystemGroups;

/// <summary>
///     Contains the list of system groups of the same type
/// </summary>
internal class SystemGroupAggregate : ISystemGroupAggregate<SystemGroupAggregate.None>
{
    internal class Factory : ISystemGroupAggregate<None>.IFactory
    {
        internal static readonly Factory Instance = new();
        
        public ISystemGroupAggregate<None> Create(Type systemGroupType) => new SystemGroupAggregate(systemGroupType);
    }

    private readonly List<SystemGroup> _systemGroups = new(16);

    /// <summary>
    ///     For debugging purpose only
    /// </summary>
    internal readonly Type GroupType;

    public SystemGroupAggregate(Type groupType)
    {
        GroupType = groupType;
    }

    public int Count => _systemGroups.Count;

    public void TriggerUpdate()
    {
        for (var i = 0; i < _systemGroups.Count; i++)
        {
            _systemGroups[i].Update();
        }
    }

    public void Add(in None none, SystemGroup systemGroup)
    {
        _systemGroups.Add(systemGroup);
    }

    public void Remove(SystemGroup systemGroup)
    {
        _systemGroups.Remove(systemGroup);
    }

    internal struct None
    {
    }
}