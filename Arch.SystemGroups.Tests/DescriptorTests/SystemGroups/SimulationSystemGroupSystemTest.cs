using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.DescriptorTests
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class SimulationSystemGroupSystemTest : BaseSystem<TestWorld, float>
    {
        public SimulationSystemGroupSystemTest(TestWorld world) : base(world)
        {
        }
    }
}