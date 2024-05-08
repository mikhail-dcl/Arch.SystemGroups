using System;
using Arch.SystemGroups.UnityBridge;

namespace Arch.SystemGroups.Throttling
{
    /// <summary>
    ///     Provides a way to throttle systems in the root system group, reused for different system groups
    /// </summary>
    public interface ISystemGroupThrottler
    {
        /// <summary>
        ///     Called when the system group begins to update within the Unity Player Loop
        /// </summary>
        /// <param name="systemGroupType">Type of the system group</param>
        /// <param name="timeInfo">Information about time</param>
        bool ShouldThrottle(Type systemGroupType, in TimeProvider.Info timeInfo);

        /// <summary>
        ///     Called when the whole system group finishes its update, irrespective of whether it was throttled or not
        /// </summary>
        /// <param name="systemGroupType">Type of the system group</param>
        /// <param name="wasThrottled">The execution was throttled</param>
        void OnSystemGroupUpdateFinished(Type systemGroupType, bool wasThrottled);
    }

    /// <summary>
    ///     Throttler dedicated to the system groups based on non-fixed updates
    /// </summary>
    public interface IUpdateBasedSystemGroupThrottler : ISystemGroupThrottler
    {
    }

    /// <summary>
    ///     Throttler dedicated to the system groups based on fixed updates
    /// </summary>
    public interface IFixedUpdateBasedSystemGroupThrottler : ISystemGroupThrottler
    {
    }
}