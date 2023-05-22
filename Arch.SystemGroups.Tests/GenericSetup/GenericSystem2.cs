using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.GenericSetup;

[UpdateInGroup(typeof(Group2OfGenerics))]
public partial class GenericSystem2<TStruct> : BaseSystem<TestWorld, float> where TStruct : struct
{
    private readonly TStruct _param1;

    public GenericSystem2(TestWorld world, TStruct param1) : base(world)
    {
        _param1 = param1;
    }
}