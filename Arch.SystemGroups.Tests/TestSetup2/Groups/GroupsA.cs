using Arch.SystemGroups.DefaultSystemGroups;

namespace Arch.SystemGroups.Tests.TestSetup2.Groups;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateBefore(typeof(GroupAB))]
public partial class GroupAA
{
    
}

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(GroupAA))]
public partial class GroupAB
{
    
}

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(GroupAB))]
public partial class GroupAC
{
    
}

