using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.MetadataTests.Systems;

[UpdateInGroup(typeof(Groups.MetadataGroup1))]
[Custom1("TestValue2")]
[Custom2(0, 0f)]
[Custom2(1, 1f)]
[Custom2(2, 2f)]
[Custom2(3, 3f)]
public partial class MetadataSystem2 : PlayerLoopSystem<TestWorld>
{
    internal MetadataSystem2(TestWorld world) : base(world)
    {
    }
}