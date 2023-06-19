using System;
using System.Collections.Generic;
using Arch.System;
using Arch.SystemGroups.UnityBridge;

namespace Arch.SystemGroups.DefaultSystemGroups;

/// <summary>
/// Updates at the end of the Initialization phase of the player loop
/// </summary>
public class InitializationSystemGroup : SystemGroup
{
    internal InitializationSystemGroup(List<ISystem<float>> systems) : base(systems)
    {
    }
    
    internal static readonly InitializationSystemGroup Empty = new (null);

    internal override void Update()
    {
        Update(Time.DeltaTime);
    }
}