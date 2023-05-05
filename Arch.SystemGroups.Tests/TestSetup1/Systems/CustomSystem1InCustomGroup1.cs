using Arch.System;
using Arch.SystemGroups.Tests.TestSetup1.Groups;

namespace Arch.SystemGroups.Tests.TestSetup1.Systems;

[UpdateInGroup(typeof(CustomGroup1))]
public partial class CustomSystem1InCustomGroup1 : BaseSystem<TestWorld, float>
{
    public bool IsInitialized { get; private set; }
    public bool IsDisposed { get; private set; }
    
    public CustomSystem1InCustomGroup1(TestWorld world, double arg1, Action<float, int> f) : base(world)
    {
    }
    
    public override void Initialize()
    {
        IsInitialized = true;
    }

    public override void Dispose()
    {
        IsDisposed = true;
    }
}