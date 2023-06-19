using System;
using System.Collections.Generic;
using Arch.System;
using Arch.SystemGroups.UnityBridge;

namespace Arch.SystemGroups.DefaultSystemGroups;

/// <summary>
/// Updates at the end of the PostLateUpdate phase of the player loop.
/// </summary>
public class PostRenderingSystemGroup : SystemGroup
{
    internal PostRenderingSystemGroup(List<ISystem<float>> systems) : base(systems)
    {
    }
    
    internal static readonly PostRenderingSystemGroup Empty = new (null);

    internal override void Update()
    {
        Update(Time.DeltaTime);
    }
}