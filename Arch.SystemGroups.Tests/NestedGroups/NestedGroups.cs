using Arch.System;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.NestedGroups;

[UpdateInGroup(typeof(RootGroup))]
public partial class NestedGroup1
{
    
}

[UpdateInGroup(typeof(NestedGroup1))]
public partial class NestedGroup2
{
    
}

[UpdateInGroup(typeof(NestedGroup2))]
public partial class SystemInNestedGroup : BaseSystem<TestWorld, float>
{
    public SystemInNestedGroup(TestWorld world) : base(world)
    {
    }
}