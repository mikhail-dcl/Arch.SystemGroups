using Arch.System;
using Arch.SystemGroups.Tests.TestSetup2.Groups;

namespace Arch.SystemGroups.Tests.TestSetup2.Systems;

[UpdateInGroup(typeof(GroupAA))]
[UpdateBefore(typeof(SystemCGroupAA))]
public partial class SystemAGroupAA : BaseSystem<TestWorld2, float>
{
    public SystemAGroupAA(TestWorld2 world) : base(world)
    {
    }
}

[UpdateBefore(typeof(SystemCGroupAA))]
[UpdateAfter(typeof(SystemAGroupAA))]
[UpdateInGroup(typeof(GroupAA))]
public partial class SystemBGroupAA : BaseSystem<TestWorld2, float>
{
    public SystemBGroupAA(TestWorld2 world) : base(world)
    {
    }
}

[UpdateInGroup(typeof(GroupAA))]
public partial class SystemCGroupAA : BaseSystem<TestWorld2, float>
{
    public SystemCGroupAA(TestWorld2 world) : base(world)
    {
    }
}

// Group AB

[UpdateInGroup(typeof(GroupAB))]
// [UpdateBefore(typeof(SystemCGroupAA))] // no tolerance
[UpdateBefore(typeof(SystemCGroupAB))]
public partial class SystemAGroupAB : BaseSystem<TestWorld2, float>
{
    public SystemAGroupAB(TestWorld2 world) : base(world)
    {
    }
}

[UpdateBefore(typeof(SystemCGroupAB))]
[UpdateAfter(typeof(SystemAGroupAB))]
[UpdateInGroup(typeof(GroupAB))]
public partial class SystemBGroupAB : BaseSystem<TestWorld2, float>
{
    public SystemBGroupAB(TestWorld2 world) : base(world)
    {
    }
}

[UpdateInGroup(typeof(GroupAB))]
public partial class SystemCGroupAB : BaseSystem<TestWorld2, float>
{
    public SystemCGroupAB(TestWorld2 world) : base(world)
    {
    }
}