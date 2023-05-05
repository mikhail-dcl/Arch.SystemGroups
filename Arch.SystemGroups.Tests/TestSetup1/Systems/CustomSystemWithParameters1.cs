using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;

namespace Arch.SystemGroups.Tests.TestSetup1.Systems;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class CustomSystemWithParameters1 : BaseSystem<TestWorld, float>
{
    private readonly string _param1;
    private readonly int _param2;
    
    public bool IsInitialized { get; private set; }
    public bool IsDisposed { get; private set; }

    public CustomSystemWithParameters1(TestWorld world, string param1, int param2) : base(world)
    {
        _param1 = param1;
        _param2 = param2;
    }
    
    public override void Initialize()
    {
        IsInitialized = true;
    }

    public override void Dispose()
    {
        IsDisposed = true;
    }
}