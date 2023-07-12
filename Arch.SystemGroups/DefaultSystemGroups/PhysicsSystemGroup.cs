using System.Collections.Generic;
using Arch.System;
using Arch.SystemGroups.Throttling;
using Arch.SystemGroups.UnityBridge;
using JetBrains.Annotations;

namespace Arch.SystemGroups.DefaultSystemGroups;

/// <summary>
/// Updates at the beginning of the FixedUpdate phase of the player loop
/// before all fixed updates
/// </summary>
public class PhysicsSystemGroup : SystemGroup
{
    internal PhysicsSystemGroup(List<ExecutionNode<float>> nodes, [CanBeNull] ISystemGroupThrottler throttler, [CanBeNull] ISystemGroupExceptionHandler exceptionHandler) : base(nodes, throttler, exceptionHandler)
    {
    }
    
    internal static readonly PhysicsSystemGroup Empty = new (null, null, null);

    public override void Update()
    {
        Update(TimeProvider.GetFixedInfo());
    }
}