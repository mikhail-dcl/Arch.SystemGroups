using Arch.SystemGroups.Throttling;

namespace Arch.SystemGroups.Tests.CentralizedThrottling.Groups;

[UpdateInGroup(typeof(NoThrottlingRootGroup))]
[ThrottlingEnabled]
public partial class ThrottlingNestedGroup
{
}