using Arch.System;
using Arch.SystemGroups.Tests.CentralizedThrottling.Groups;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.CentralizedThrottling.Systems;

[UpdateInGroup(typeof(NoThrottlingRootGroup))]
public partial class NoThrottlingNestedSystem : CentralizedThrottlingTestSystemBase
{
    public NoThrottlingNestedSystem(TestWorld world) : base(world)
    {
    }
}