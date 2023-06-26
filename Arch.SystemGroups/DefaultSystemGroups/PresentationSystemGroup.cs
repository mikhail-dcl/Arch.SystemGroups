using System.Collections.Generic;
using Arch.System;
using Arch.SystemGroups.Throttling;
using Arch.SystemGroups.UnityBridge;
using JetBrains.Annotations;

namespace Arch.SystemGroups.DefaultSystemGroups;

/// <summary>
/// Updates at the end of the PreLateUpdate phase of the player loop.
/// </summary>
public class PresentationSystemGroup : SystemGroup
{
    internal PresentationSystemGroup(List<ExecutionNode<float>> systems, [CanBeNull] ISystemGroupThrottler throttler,
        [CanBeNull] ISystemGroupExceptionHandler exceptionHandler) : base(systems, throttler, exceptionHandler)
    {
    }
    
    internal static readonly PresentationSystemGroup Empty = new (null, null, null);

    internal override void Update()
    {
        Update(TimeProvider.GetInfo());
    }
}