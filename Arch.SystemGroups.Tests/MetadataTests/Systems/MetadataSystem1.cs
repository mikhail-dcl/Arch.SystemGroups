using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.MetadataTests.Systems;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[Custom1("TestValue1")]
[Custom2(100, 200.5f)]
[Custom2(1, 2f)]
public partial class MetadataSystem1 : PlayerLoopSystem<TestWorld>
{
    internal MetadataSystem1(TestWorld world) : base(world)
    {
    }
}