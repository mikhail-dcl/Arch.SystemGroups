using Arch.SystemGroups.Tests.CentralizedThrottling.Groups;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.CentralizedThrottling.Systems;

[UpdateInGroup(typeof(ImplicitThrottlingNestedGroup))]
public partial class ImplicitThrottlingImplicitNestedSystem : CentralizedThrottlingTestSystemBase
{
    public ImplicitThrottlingImplicitNestedSystem(TestWorld world) : base(world)
    {
    }
}