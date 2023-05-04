using System;
using Arch.System;

namespace Arch.SystemGroups;

/// <summary>
/// The specified Type must be a SystemGroup.
/// Updating in a group means this system will be automatically updated by the specified ComponentSystemGroup when the group is updated.
/// The system may order itself relative to other systems in the group with UpdateBefore and UpdateAfter. This ordering takes
/// effect when the system group is sorted.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class UpdateInGroupAttribute : Attribute
{
    /// <summary>
    /// Specify the <see cref="SystemGroup"/> or <see cref="Group{T}"/> which the tagged system should be added to. The tagged system
    /// will be updated as part of this system group's Update() method.
    /// </summary>
    /// <param name="groupType">The <see cref="SystemGroup"/> type/</param>
    /// <exception cref="ArgumentNullException">Thrown id the group type is empty.</exception>
    public UpdateInGroupAttribute(Type groupType)
    {
        if (groupType == null)
            throw new ArgumentNullException(nameof(groupType));

        GroupType = groupType;
    }

    /// <summary>
    /// Retrieve the <see cref="SystemGroup"/> type.
    /// </summary>
    public Type GroupType { get; }
}