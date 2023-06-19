namespace Arch.SystemGroups.Tests.ThrottleGroup;

/// <summary>
/// Skips every other update
/// </summary>
public abstract class ThrottleGroupBase : CustomGroupBase<float>
{
    private bool open;
    
    public override void Dispose()
    {
        DisposeInternal();
    }

    public override void Initialize()
    {
        InitializeInternal();
    }

    public override void BeforeUpdate(in float t)
    {
        // Before Update is always called first in the same frame
        open = !open;
        
        if (open)
            BeforeUpdateInternal(in t);
    }

    public override void Update(in float t)
    {
        if (open)
            UpdateInternal(in t);
    }

    public override void AfterUpdate(in float t)
    {
        if (open)
            AfterUpdateInternal(in t);
    }
}