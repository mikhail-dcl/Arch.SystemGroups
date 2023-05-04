using System;
using System.Collections.Generic;
using Arch.System;
using UnityEngine;

namespace Arch.SystemGroups.DefaultSystemGroups;

/// <summary>
/// Updates at the end of the PostLateUpdate phase of the player loop.
/// </summary>
public class PostRenderingSystemGroup : SystemGroup
{
    internal PostRenderingSystemGroup(List<ISystem<float>> systems) : base(systems)
    {
    }
    
    internal static readonly PostRenderingSystemGroup Empty = new (new List<ISystem<float>>());

    internal override void Update()
    {
        Update(Time.deltaTime);
    }
}