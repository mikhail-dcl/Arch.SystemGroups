using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.GenericSetup;

[UpdateInGroup(typeof(Group1OfGenerics))]
public partial class GenericSystem3<T1, T2, T3> : BaseSystem<TestWorld, float>
    where T1 : class where T2: IEquatable<T2>, new() where T3 : unmanaged, Enum
{
    private readonly T1 _t1;
    private readonly T2 _t2;
    private readonly T3 _en;

    public GenericSystem3(TestWorld world, T1 t1, T2 t2, T3 en) : base(world)
    {
        _t1 = t1;
        _t2 = t2;
        _en = en;
    }
}