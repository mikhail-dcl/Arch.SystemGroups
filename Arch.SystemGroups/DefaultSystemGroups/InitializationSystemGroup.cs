using System;
using System.Collections.Generic;
using Arch.System;
using UnityEngine;

namespace Arch.SystemGroups.DefaultSystemGroups;

/// <summary>
/// Updates at the end of the Initialization phase of the player loop
/// </summary>
public class InitializationSystemGroup : SystemGroup
{
    public InitializationSystemGroup(List<ISystem<float>> systems) : base(systems)
    {
    }
    
    internal static readonly InitializationSystemGroup Empty = new (new List<ISystem<float>>());

    internal override void Update()
    {
        Update(Time.deltaTime);
    }
}