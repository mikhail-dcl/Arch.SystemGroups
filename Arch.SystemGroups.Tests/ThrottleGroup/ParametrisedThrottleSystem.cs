using Arch.System;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.ThrottleGroup;

[UpdateInGroup(typeof(ParametrisedThrottleGroup))]
public partial class ParametrisedThrottleSystem : BaseSystem<TestWorld, float>
{
    public ParametrisedThrottleSystem(TestWorld world) : base(world)
    {
    }

    public int UpdateCount { get; private set; }
    public int BeforeUpdateCount { get; private set; }
    public int AfterUpdateCount { get; private set; }

    public override void BeforeUpdate(in float t)
    {
        BeforeUpdateCount++;
    }

    public override void AfterUpdate(in float t)
    {
        AfterUpdateCount++;
    }

    public override void Update(in float t)
    {
        UpdateCount++;
    }
}