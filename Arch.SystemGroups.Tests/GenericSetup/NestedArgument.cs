using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.GenericSetup;

public class ParentClass<T>
{
    public class NestedType
    {
        public T Value;
    }
}

public struct ParamGen
{
    public bool Value;
}

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class SystemWithGenericNestedArgument : BaseSystem<TestWorld, float>
{
    private readonly ParentClass<ParamGen>.NestedType _param;

    internal SystemWithGenericNestedArgument(TestWorld world, ParentClass<ParamGen>.NestedType param) : base(world)
    {
        _param = param;
    }
}