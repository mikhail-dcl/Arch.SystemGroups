using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.DescriptorTests
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    public partial class PhysicsSystemGroupSystemTest : BaseSystem<TestWorld, float>
    {
        public PhysicsSystemGroupSystemTest(TestWorld world) : base(world)
        {
        }
    }
}