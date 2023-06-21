using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;
using Arch.SystemGroups.Throttling;

namespace Arch.SystemGroups.Tests.CentralizedThrottling.Systems;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[ThrottlingEnabled]
public partial class ThrottlingRootSystem : CentralizedThrottlingTestSystemBase
{
    internal ThrottlingRootSystem(TestWorld world) : base(world)
    {
    }
}