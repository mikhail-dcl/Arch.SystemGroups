using System;

namespace Arch.SystemGroups.Throttling
{
    /// <summary>
    ///     Indicates that the system or the group can throttle
    ///     <para>If the group is marked by this attribute all its direct and transitive children will inherit it</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ThrottlingEnabledAttribute : Attribute
    {
    }
}