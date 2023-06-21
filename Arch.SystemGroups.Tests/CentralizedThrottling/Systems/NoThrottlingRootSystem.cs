using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.CentralizedThrottling.Systems;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class NoThrottlingRootSystem : CentralizedThrottlingTestSystemBase
{
    public NoThrottlingRootSystem(TestWorld world) : base(world)
    {
    }
}