using System;

namespace Arch.SystemGroups;

/// <summary>
///     Base interface for all system group aggregates
/// </summary>
public interface ISystemGroupAggregateFactory
{
    internal ISystemGroupAggregate Create(Type systemGroupType);
}