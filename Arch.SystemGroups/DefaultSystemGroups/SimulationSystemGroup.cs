using System.Collections.Generic;
using Arch.System;
using Arch.SystemGroups.Throttling;
using Arch.SystemGroups.UnityBridge;
using JetBrains.Annotations;

namespace Arch.SystemGroups.DefaultSystemGroups;

/// <summary>
/// Updates at the end of the Update phase of the player loop
/// </summary>
public class SimulationSystemGroup : SystemGroup
{
    internal SimulationSystemGroup(List<ExecutionNode<float>> systems, [CanBeNull] ISystemGroupThrottler throttler) : base(systems, throttler)
    {
    }
    
    internal static readonly SimulationSystemGroup Empty = new (null, null);

    internal override void Update()
    {
        Update(TimeProvider.GetInfo());
    }
}