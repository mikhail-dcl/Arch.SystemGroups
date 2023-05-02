/*using System;
using System.Collections.Generic;
using Arch.System;

namespace Arch.SystemGroups.DefaultSystemGroups;

/// <summary>
/// Background system group is not synchronized with Unity System Loop and can be run independently
/// from any thread
/// </summary>
public class BackgroundSystemGroup : SystemGroup
{
    public BackgroundSystemGroup(List<ISystem<float>> systems) : base(systems)
    {
    }
    
    internal static readonly BackgroundSystemGroup Empty = new (new List<ISystem<float>>());

    internal override void Update()
    {
        throw new NotImplementedException(
            $"{nameof(BackgroundSystemGroup)} must be updated explicitly by calling {nameof(Update)}(float dt) method");
    }

    public new void Update(float dt)
    {
        base.Update(dt);
    }
}*/