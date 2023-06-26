using System;

namespace Arch.SystemGroups;

/// <summary>
///     Provides exceptions handling on the level of the <see cref="SystemGroup" />
/// </summary>
public interface ISystemGroupExceptionHandler
{
    public enum Action : byte
    {
        /// <summary>
        ///     Continue execution of the system
        /// </summary>
        Continue,

        /// <summary>
        ///     Put the system group into the Error state and stops the execution
        /// </summary>
        Suspend,

        /// <summary>
        ///     Dispose the system group and stops the execution
        /// </summary>
        Dispose
    }

    /// <summary>
    ///     Handles the exception thrown by the system group, at some point the execution of the system group
    ///     should be suspended to prevent exceptions flood
    /// </summary>
    /// <param name="exception">Exception</param>
    /// <param name="systemGroupType">System Group Type</param>
    /// <returns>An action to tell the System Group how to behave after <paramref name="exception" /></returns>
    Action Handle(Exception exception, Type systemGroupType);
}