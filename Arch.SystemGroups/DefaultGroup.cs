namespace Arch.SystemGroups;

/// <summary>
/// Similar to `Arch.System.Group` but with better API that allows pooling
/// </summary>
/// <typeparam name="T"></typeparam>
public class DefaultGroup<T> : CustomGroupBase<T>
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
    public override void BeforeUpdate(in T t, bool throttle)
    {
        BeforeUpdateInternal(in t, throttle);
    }

    /// <summary>
    /// To comply with Arch.System.ISystem
    /// </summary>
    public override void Update(in T t, bool throttle)
    {
        UpdateInternal(in t, throttle);
    }
    
    /// <summary>
    /// To comply with Arch.System.ISystem
    /// </summary>
    public override void AfterUpdate(in T t, bool throttle)
    {
        AfterUpdateInternal(in t, throttle);
    }
}