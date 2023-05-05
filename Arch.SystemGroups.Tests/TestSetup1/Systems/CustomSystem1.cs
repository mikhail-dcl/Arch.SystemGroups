using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1.Groups;

namespace Arch.SystemGroups.Tests.TestSetup1.Systems;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateBefore(typeof(CustomGroup1))]
public partial class CustomSystem1 : BaseSystem<TestWorld, float>
{
    public bool IsInitialized { get; private set; }
    public bool IsDisposed { get; private set; }

    public CustomSystem1(TestWorld world) : base(world)
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