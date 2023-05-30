using Arch.System;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.GenericSetup;

[UpdateInGroup(typeof(Group2OfGenerics))]
public partial class GenericSystem1<T> : BaseSystem<TestWorld, float>
{
    private readonly T _param1;

    public GenericSystem1(TestWorld world, T param1) : base(world)
    {
        _param1 = param1;
    }
}