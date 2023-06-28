using System;
using System.Collections.Generic;

namespace Arch.SystemGroups.Metadata;

/// <summary>
/// Dummy attributes info for system groups
/// </summary>
public class SystemGroupAttributesInfo : AttributesInfoBase
{
    public static readonly SystemGroupAttributesInfo Instance = new();
    
    public override Type UpdateInGroup => null;

    public override AttributesInfoBase GroupMetadata => null;

    public override T GetAttribute<T>() => null;

    public override IReadOnlyList<T> GetAttributes<T>() => Array.Empty<T>();
}