using System;
using System.Collections.Generic;
using Arch.System;
using UnityEngine;

namespace Arch.SystemGroups.DefaultSystemGroups;

/// <summary>
/// Updates at the beginning of the FixedUpdate phase of the player loop
/// before all fixed updates
/// </summary>
public class PhysicsSystemGroup : SystemGroup
{
    internal PhysicsSystemGroup(List<ISystem<float>> systems) : base(systems)
    {
    }
    
    internal static readonly PhysicsSystemGroup Empty = new (new List<ISystem<float>>());

    internal override void Update()
    {
        Update(Time.fixedDeltaTime);
    }
}