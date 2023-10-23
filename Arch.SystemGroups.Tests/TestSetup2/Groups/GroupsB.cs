using Arch.SystemGroups.DefaultSystemGroups;

namespace Arch.SystemGroups.Tests.TestSetup2.Groups;

[UpdateInGroup(typeof(PostRenderingSystemGroup))]
public partial class GroupBA
{
}

[UpdateInGroup(typeof(PostRenderingSystemGroup))]
[UpdateAfter(typeof(GroupBA))]
public partial class GroupBB
{
}

[UpdateInGroup(typeof(GroupBA))]
// This dependency should not be taken into consideration even as
// this group does not belong to the GroupBB hierarchy.
// [UpdateAfter(typeof(GroupBB))] no tolerance
public partial class GroupBAA
{
}

[UpdateInGroup(typeof(GroupBA))]
[UpdateAfter(typeof(GroupBAA))]
// This dependency should not be taken into consideration even as
// this group does not belong to the A hierarchy.
// [UpdateBefore(typeof(GroupAA))] no tolerance
public partial class GroupBAB
{
}

[UpdateInGroup(typeof(PostRenderingSystemGroup))]
// Will be never created as it is empty
public partial class GroupBC
{
    
}