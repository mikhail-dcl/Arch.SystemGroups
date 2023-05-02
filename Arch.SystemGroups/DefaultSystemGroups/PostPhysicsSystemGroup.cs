using System.Collections.Generic;
using Arch.System;
using UnityEngine;

namespace Arch.SystemGroups.DefaultSystemGroups;

/// <summary>
/// Updates at the end of the FixedUpdate phase of the player loop
/// </summary>
public class PostPhysicsSystemGroup : SystemGroup
{
    public PostPhysicsSystemGroup(List<ISystem<float>> systems) : base(systems)
    {
    }
    
    internal static readonly PostPhysicsSystemGroup Empty = new (new List<ISystem<float>>());

    internal override void Update()
    {
        Update(Time.fixedDeltaTime);
    }
}