using System.Collections.Generic;
using Arch.System;
using UnityEngine;

namespace Arch.SystemGroups.DefaultSystemGroups;

/// <summary>
/// Updates at the end of the Update phase of the player loop
/// </summary>
public class SimulationSystemGroup : SystemGroup
{
    public SimulationSystemGroup(List<ISystem<float>> systems) : base(systems)
    {
    }
    
    internal static readonly SimulationSystemGroup Empty = new (new List<ISystem<float>>());

    internal override void Update()
    {
        Update(Time.deltaTime);
    }
}