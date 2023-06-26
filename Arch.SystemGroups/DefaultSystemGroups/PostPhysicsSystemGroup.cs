using System.Collections.Generic;
using Arch.SystemGroups.Throttling;
using Arch.SystemGroups.UnityBridge;
using JetBrains.Annotations;

namespace Arch.SystemGroups.DefaultSystemGroups;

/// <summary>
///     Updates at the end of the FixedUpdate phase of the player loop
/// </summary>
public class PostPhysicsSystemGroup : SystemGroup
{
    internal PostPhysicsSystemGroup(List<ExecutionNode<float>> nodes, [CanBeNull] ISystemGroupThrottler throttler,
        [CanBeNull] ISystemGroupExceptionHandler exceptionHandler) : base(nodes, throttler, exceptionHandler)
    {
    }

    internal static readonly PostPhysicsSystemGroup Empty = new(null, null, null);

    internal override void Update()
    {
        Update(TimeProvider.GetFixedInfo());
    }
}