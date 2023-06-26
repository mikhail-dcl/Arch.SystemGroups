using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.ExceptionsHandling;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class ThrowingSystem1 : BaseSystem<TestWorld, float>
{
    public ThrowingSystem1(TestWorld world) : base(world)
    {
    }

    public override void Update(in float t)
    {
        throw new NotSupportedException();
    }
}