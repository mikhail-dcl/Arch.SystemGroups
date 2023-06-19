using System;

namespace Arch.SystemGroups;

/// <summary>
/// Indicates that the group was not injected into the builder
/// but there are systems included in it.
/// </summary>
public class GroupNotFoundException : Exception
{
    private readonly Type _type;

    public GroupNotFoundException(Type type)
    {
        _type = type;
    }
    
    public override string Message => $"Group {_type} was not injected into the builder";
}