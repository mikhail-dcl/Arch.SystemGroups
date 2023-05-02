using Arch.System;

namespace Arch.SystemGroups.Tests.TestSetup1.Systems;

/// <summary>
/// No code generation should be done for this system.
/// </summary>
public class DisconnectedSystem : BaseSystem<TestWorld, float>
{
    public DisconnectedSystem(TestWorld world) : base(world)
    {
    }
}