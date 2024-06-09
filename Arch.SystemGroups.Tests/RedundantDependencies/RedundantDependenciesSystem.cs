using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.RedundantDependencies;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(RDSystem2))]
[UpdateBefore(typeof(RDSystem4))]
public partial class RDSystem1 : BaseSystem<TestWorld, float>
{
    public RDSystem1(TestWorld world) : base(world)
    {
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(RDSystem3))]
public partial class RDSystem2 : BaseSystem<TestWorld, float>
{
    public RDSystem2(TestWorld world) : base(world)
    {
    }
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(RDSystem1))]
[UpdateBefore(typeof(RDSystem4))]
[UpdateAfter(typeof(RDSystem2))]
public partial class RDSystem3 : BaseSystem<TestWorld, float>
{
    public RDSystem3(TestWorld world) : base(world)
    {
    }
}

[UpdateAfter(typeof(RDSystem2))]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class RDSystem4 : BaseSystem<TestWorld, float>
{
    public RDSystem4(TestWorld world) : base(world)
    {
    }
}