using System;
using System.Collections.Generic;

namespace Arch.SystemGroups.Metadata;

/// <summary>
/// Contains information about UpdateInGroup and GroupMetadata only
/// </summary>
public class DefaultGroupAttributesInfo : AttributesInfoBase
{
    public DefaultGroupAttributesInfo(Type updateInGroup, AttributesInfoBase groupMetadata)
    {
        UpdateInGroup = updateInGroup;
        GroupMetadata = groupMetadata;
    }

    public override Type UpdateInGroup { get; }
    public override AttributesInfoBase GroupMetadata { get; }

    public override T GetAttribute<T>() => null;

    public override IReadOnlyList<T> GetAttributes<T>() => Array.Empty<T>();
}