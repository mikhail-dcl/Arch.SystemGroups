using Arch.System;

namespace Arch.SystemGroups;

/// <summary>
/// Similar to `Arch.System.Group` but with better API that allows pooling
/// </summary>
/// <typeparam name="T"></typeparam>
public class DefaultGroup<T> : CustomGroupBase<T>, ISystem<T>
{
    /// <summary>
    /// Creates an empty group, for auto-generated code only,
    /// Don't invoke it manually
    /// </summary>
    protected DefaultGroup()
    {
    }
    
    /// <summary>
    /// Initialize all systems in the group
    /// </summary>
    public override void Initialize()
    {
        InitializeInternal();
    }

    /// <summary>
    /// Dispose all systems in the group
    /// </summary>
    public override void Dispose()
    {
        DisposeInternal();
    }

    /// <summary>
    /// To comply with Arch.System.ISystem
    /// </summary>
    /// <param name="t">Delta time</param>
    public override void BeforeUpdate(in T t)
    {
        BeforeUpdateInternal(in t);
    }

    /// <summary>
    /// To comply with Arch.System.ISystem
    /// </summary>
    /// <param name="t">Delta time</param>
    public override void Update(in T t)
    {
        UpdateInternal(in t);
    }
    
    /// <summary>
    /// To comply with Arch.System.ISystem
    /// </summary>
    /// <param name="t">Delta time</param>
    public override void AfterUpdate(in T t)
    {
        AfterUpdateInternal(in t);
    }
}