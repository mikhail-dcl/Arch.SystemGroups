using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.DescriptorTests
{
    [UpdateInGroup(typeof(PostPhysicsSystemGroup))]
    public partial class PostPhysicsSystemGroupSystemTest : BaseSystem<TestWorld, float>
    {
        public PostPhysicsSystemGroupSystemTest(TestWorld world) : base(world)
        {
        }
    }
}