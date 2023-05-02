using Arch.System;
using Arch.SystemGroups.Tests.TestSetup2.Groups;

namespace Arch.SystemGroups.Tests.TestSetup2.Systems;

[UpdateInGroup(typeof(GroupBA))]
// Redundant attributes should not matter
[UpdateBefore(typeof(SystemCGroupBA))]
[UpdateBefore(typeof(SystemBGroupBA))]
[UpdateBefore(typeof(SystemDGroupBA))]
public partial class SystemAGroupBA : BaseSystem<TestWorld2, float>
{
    public SystemAGroupBA(TestWorld2 world) : base(world)
    {
    }
}

[UpdateInGroup(typeof(GroupBA))]
[UpdateAfter(typeof(SystemAGroupBA))]
public partial class SystemBGroupBA : BaseSystem<TestWorld2, float>
{
    public SystemBGroupBA(TestWorld2 world) : base(world)
    {
    }
}

[UpdateInGroup(typeof(GroupBA))]
[UpdateAfter(typeof(SystemBGroupBA))]
[UpdateAfter(typeof(SystemAGroupBA))]
public partial class SystemCGroupBA : BaseSystem<TestWorld2, float>
{
    public SystemCGroupBA(TestWorld2 world) : base(world)
    {
    }
}

[UpdateInGroup(typeof(GroupBA))]
[UpdateAfter(typeof(SystemCGroupBA))]
[UpdateAfter(typeof(SystemBGroupBA))]
[UpdateAfter(typeof(SystemAGroupBA))]
[UpdateAfter(typeof(SystemDGroupBA))] // <-- will be ignored by the generator
public partial class SystemDGroupBA : BaseSystem<TestWorld2, float>
{
    public SystemDGroupBA(TestWorld2 world) : base(world)
    {
    }
}

[UpdateInGroup(typeof(GroupBB))]
[UpdateBefore(typeof(SystemBGroupBB))]
// The following edges will be ignored as belong to a different group
[UpdateAfter(typeof(SystemCGroupBA))]
[UpdateAfter(typeof(SystemBGroupBA))]
[UpdateAfter(typeof(SystemAGroupBA))]
[UpdateAfter(typeof(SystemDGroupBA))]
public partial class SystemAGroupBB : BaseSystem<TestWorld2, float>
{
    public SystemAGroupBB(TestWorld2 world) : base(world)
    {
    }
}

[UpdateInGroup(typeof(GroupBB))]
public partial class SystemBGroupBB : BaseSystem<TestWorld2, float>
{
    public SystemBGroupBB(TestWorld2 world) : base(world)
    {
    }
}

[UpdateInGroup(typeof(GroupBAA))]
[UpdateBefore(typeof(SystemBGroupBAA))]
public partial class SystemAGroupBAA : BaseSystem<TestWorld2, float>
{
    public SystemAGroupBAA(TestWorld2 world) : base(world)
    {
    }
}

[UpdateInGroup(typeof(GroupBAA))]
[UpdateBefore(typeof(SystemCGroupBAA))]
public partial class SystemBGroupBAA : BaseSystem<TestWorld2, float>
{
    public SystemBGroupBAA(TestWorld2 world) : base(world)
    {
    }
}

[UpdateInGroup(typeof(GroupBAA))]
public partial class SystemCGroupBAA : BaseSystem<TestWorld2, float>
{
    public SystemCGroupBAA(TestWorld2 world) : base(world)
    {
    }
}

[UpdateInGroup(typeof(GroupBAB))]
public partial class SystemAGroupBAB : BaseSystem<TestWorld2, float>
{
    public SystemAGroupBAB(TestWorld2 world) : base(world)
    {
    }
}