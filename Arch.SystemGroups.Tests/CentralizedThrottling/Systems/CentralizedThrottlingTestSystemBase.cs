using Arch.System;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.CentralizedThrottling.Systems;

public abstract class CentralizedThrottlingTestSystemBase : BaseSystem<TestWorld, float>
{
    public bool UpdateInvoked { get; private set; }
    public bool BeforeUpdateInvoked { get; private set; }
    public bool AfterUpdateInvoked { get; private set; }
    
    protected CentralizedThrottlingTestSystemBase(TestWorld world) : base(world)
    {
    }

    public override void Update(in float t)
    {
        UpdateInvoked = true;
    }
    
    public override void BeforeUpdate(in float t)
    {
        BeforeUpdateInvoked = true;
    }
    
    public override void AfterUpdate(in float t)
    {
        AfterUpdateInvoked = true;
    }
}