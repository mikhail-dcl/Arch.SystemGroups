using System.Collections.Generic;
using Arch.System;
using Arch.SystemGroups.UnityBridge;

namespace Arch.SystemGroups.DefaultSystemGroups;

/// <summary>
/// Updates at the end of the Update phase of the player loop
/// </summary>
public class SimulationSystemGroup : SystemGroup
{
    internal SimulationSystemGroup(List<ISystem<float>> systems) : base(systems)
    {
    }
    
    internal static readonly SimulationSystemGroup Empty = new (null);

    internal override void Update()
    {
        Update(Time.DeltaTime);
    }
}