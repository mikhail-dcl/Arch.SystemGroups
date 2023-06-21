namespace Arch.SystemGroups.Tests.ThrottleGroup;

/// <summary>
/// Skips every other update
/// </summary>
public abstract class ThrottleGroupBase : CustomGroupBase<float>
{
    private readonly int _framesToSkip;
    private int _counter;

    private bool _open;

    protected ThrottleGroupBase(int framesToSkip)
    {
        _framesToSkip = framesToSkip;
        _counter = framesToSkip;
    }

    public override void Dispose()
    {
        DisposeInternal();
    }

    public override void Initialize()
    {
        InitializeInternal();
    }

    public override void BeforeUpdate(in float t, bool throttle)
    {
        // Before Update is always called first in the same frame
        _open = _counter >= _framesToSkip;
        _counter++;
        if (_open)
        {
            BeforeUpdateInternal(in t, throttle);
            _counter = 0;
        }
    }

    public override void Update(in float t, bool throttle)
    {
        if (_open)
            UpdateInternal(in t, throttle);
    }

    public override void AfterUpdate(in float t, bool throttle)
    {
        if (_open)
            AfterUpdateInternal(in t, throttle);
    }
}