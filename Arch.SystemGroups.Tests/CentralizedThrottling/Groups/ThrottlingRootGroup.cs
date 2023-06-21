using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Throttling;

namespace Arch.SystemGroups.Tests.CentralizedThrottling.Groups;

[ThrottlingEnabled]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class ThrottlingRootGroup
{
}