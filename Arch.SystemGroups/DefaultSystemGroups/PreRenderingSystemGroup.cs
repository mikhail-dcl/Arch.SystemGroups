using System.Collections.Generic;
using Arch.SystemGroups.Throttling;
using Arch.SystemGroups.UnityBridge;
using JetBrains.Annotations;

namespace Arch.SystemGroups.DefaultSystemGroups
{
    /// <summary>
    /// Updates at the beginning of the PostLateUpdate phase of the player loop.
    /// </summary>
    public class PreRenderingSystemGroup : SystemGroup
    {
        internal PreRenderingSystemGroup(List<ExecutionNode<float>> nodes, [CanBeNull] ISystemGroupThrottler throttler,
            [CanBeNull] ISystemGroupExceptionHandler exceptionHandler) : base(nodes, throttler, exceptionHandler)
        {
        }

        internal static readonly PreRenderingSystemGroup Empty = new(null, null, null);

        public override void Update()
        {
            Update(TimeProvider.GetInfo());
        }
    }
}