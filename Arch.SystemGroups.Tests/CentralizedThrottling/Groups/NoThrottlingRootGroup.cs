using Arch.SystemGroups.DefaultSystemGroups;

namespace Arch.SystemGroups.Tests.CentralizedThrottling.Groups;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class NoThrottlingRootGroup
{
}