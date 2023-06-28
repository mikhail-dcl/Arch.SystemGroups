using Arch.SystemGroups.DefaultSystemGroups;

namespace Arch.SystemGroups.Tests.MetadataTests.Groups;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[Custom3(typeof(string))]
[Custom4("TestValue1", "TestValue2")]
public partial class MetadataGroup1
{
    
}