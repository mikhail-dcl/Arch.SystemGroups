using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.DisconnectedDependencies;

[UpdateInGroup(typeof(PostRenderingSystemGroup))]
[UpdateAfter(typeof(DDSystem2))] // exception
public partial class DDSystem1 : BaseSystem<TestWorld, float>
{
    public DDSystem1(TestWorld world) : base(world)
    {
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class DDSystem2 : BaseSystem<TestWorld, float>
{
    public DDSystem2(TestWorld world) : base(world)
    {
    }
}

[UpdateInGroup(typeof(DDGroup1))]
public partial class DDSystem1Gr1 : BaseSystem<TestWorld, float>
{
    public DDSystem1Gr1(TestWorld world) : base(world)
    {
    }
}

[UpdateInGroup(typeof(DDGroup2))]
public partial class DDSystem1Gr2 : BaseSystem<TestWorld, float>
{
    public DDSystem1Gr2(TestWorld world) : base(world)
    {
    }
}