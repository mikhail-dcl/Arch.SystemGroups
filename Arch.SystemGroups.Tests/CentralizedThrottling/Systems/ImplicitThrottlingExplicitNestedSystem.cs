using Arch.System;
using Arch.SystemGroups.Tests.CentralizedThrottling.Groups;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.CentralizedThrottling.Systems;

[UpdateInGroup(typeof(ThrottlingNestedGroup))]
public partial class ImplicitThrottlingExplicitNestedSystem : CentralizedThrottlingTestSystemBase
{
    public ImplicitThrottlingExplicitNestedSystem(TestWorld world) : base(world)
    {
    }
}