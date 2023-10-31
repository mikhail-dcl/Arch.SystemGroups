using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;

namespace Arch.SystemGroups.Tests.DescriptorTests
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class PresentationSystemGroupSystemTest : BaseSystem<TestWorld, float>
    {
        public PresentationSystemGroupSystemTest(TestWorld world) : base(world)
        {
        }
    }
}