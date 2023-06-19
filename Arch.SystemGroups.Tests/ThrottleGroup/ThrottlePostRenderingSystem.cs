using Arch.System;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.ThrottleGroup;

[UpdateInGroup(typeof(ThrottlePostRenderingGroup))]
public partial class ThrottlePostRenderingSystem : BaseSystem<TestWorld, float>
{
    public ThrottlePostRenderingSystem(TestWorld world) : base(world)
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