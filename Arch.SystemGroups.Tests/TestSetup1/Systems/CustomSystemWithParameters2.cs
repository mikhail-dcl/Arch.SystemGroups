using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;

namespace Arch.SystemGroups.Tests.TestSetup1.Systems;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(CustomSystemWithParameters1))]
[UpdateBefore(typeof(CustomSystem1))]
public partial class CustomSystemWithParameters2 : BaseSystem<TestWorld, float>
{
    public readonly CustomClass1 CustomClass1;

    public CustomSystemWithParameters2(TestWorld world, CustomClass1 customClass1) : base(world)
    {
        CustomClass1 = customClass1;
    }
}