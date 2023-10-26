using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.DescriptorTests
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class InitSystemGroupSystemTest : BaseSystem<TestWorld, float>
    {
        public InitSystemGroupSystemTest(TestWorld world) : base(world)
        {
        }
    }
}