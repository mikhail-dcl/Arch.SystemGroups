using System.Collections.Generic;
using Arch.System;
using UnityEngine;

namespace Arch.SystemGroups.DefaultSystemGroups;

/// <summary>
/// Updates at the end of the PreLateUpdate phase of the player loop.
/// </summary>
public class PresentationSystemGroup : SystemGroup
{
    internal PresentationSystemGroup(List<ISystem<float>> systems) : base(systems)
    {
    }
    
    internal static readonly PresentationSystemGroup Empty = new (null);

    internal override void Update()
    {
        Update(Time.deltaTime);
    }
}