using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.GenericSetup;

[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateBefore(typeof(Group1OfGenerics))]
public partial class GenericSystem0<T> : BaseSystem<TestWorld, float> where T : struct
{
    private readonly T _param1;

    public GenericSystem0(TestWorld world, T param1) : base(world)
    {
        _param1 = param1;
    }
}