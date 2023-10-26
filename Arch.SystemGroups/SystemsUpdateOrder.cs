using System;

namespace Arch.SystemGroups
{
    // Interface used for constraining generic functions on Attributes
// which control system update, creation, or destruction order
    internal interface ISystemOrderAttribute
    {
        Type SystemType { get; }
    }

    /// <summary>
    /// Apply to a system to specify an update ordering constraint with another system in the same <see cref="SystemGroup"/> or <see cref="DefaultGroup{T}"/>.
    /// </summary>
    /// <remarks>Updating before or after a system constrains the scheduler ordering of these systems within a ComponentSystemGroup.
    /// Both the before and after systems must be a members of the same ComponentSystemGroup.</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class UpdateBeforeAttribute : Attribute, ISystemOrderAttribute
    {
        /// <summary>
        /// Specify a system which the tagged system must update before.
        /// </summary>
        /// <param name="systemType">The target system which the tagged system must update before. This system must be
        /// a member of the same <see cref="SystemGroup"/> or <see cref="DefaultGroup{T}"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if the system type is empty.</exception>
        public UpdateBeforeAttribute(Type systemType)
        {
            SystemType = systemType ?? throw new ArgumentNullException(nameof(systemType));
        }

        /// <summary>
        /// The type of the target system, which the tagged system must update before.
        /// </summary>
        public Type SystemType { get; }
    }

    /// <summary>
    /// Apply to a system to specify an update ordering constraint with another system in the same <see cref="SystemGroup"/> or <see cref="DefaultGroup{T}"/>.
    /// </summary>
    /// <remarks>Updating before or after a system constrains the scheduler ordering of these systems within a ComponentSystemGroup.
    /// Both the before and after systems must be a members of the same ComponentSystemGroup.</remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class UpdateAfterAttribute : Attribute, ISystemOrderAttribute
    {
        /// <summary>
        /// Specify a system which the tagged system must update after.
        /// </summary>
        /// <param name="systemType">The target system which the tagged system must update after. This system must be
        /// a member of the same <see cref="SystemGroup"/> or <see cref="DefaultGroup{T}"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if the system type is empty.</exception>
        public UpdateAfterAttribute(Type systemType)
        {
            SystemType = systemType ?? throw new ArgumentNullException(nameof(systemType));
        }

        /// <summary>
        /// The type of the target system, which the tagged system must update after.
        /// </summary>
        public Type SystemType { get; }
    }
}