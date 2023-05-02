using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1.Groups;

namespace Arch.SystemGroups.Tests.TestSetup1.Systems;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateBefore(typeof(CustomGroup1))]
public partial class CustomSystem1 : BaseSystem<TestWorld, float>
{
    public CustomSystem1(TestWorld world) : base(world)
    {
    }
}