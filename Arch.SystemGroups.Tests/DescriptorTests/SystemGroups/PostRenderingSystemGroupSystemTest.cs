using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.DescriptorTests
{
    [UpdateInGroup(typeof(PostRenderingSystemGroup))]
    public partial class PostRenderingSystemGroupSystemTest : BaseSystem<TestWorld, float>
    {
        public PostRenderingSystemGroupSystemTest(TestWorld world) : base(world)
        {
        }
    }
}