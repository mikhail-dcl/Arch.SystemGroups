using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Descriptors;
using Arch.SystemGroups.Tests.NestedGroups;
using Arch.SystemGroups.Tests.TestSetup1;
using NSubstitute;

namespace Arch.SystemGroups.Tests.DescriptorTests;

public class DescriptorTests
{
    private ArchSystemsWorldBuilder<TestWorld> _worldBuilder;
    
    [SetUp]
    public void SetUp()
    {
        _worldBuilder = new ArchSystemsWorldBuilder<TestWorld>(new TestWorld(), Substitute.For<IPlayerLoop>());
        SystemGroups.InitSystemGroupSystemTest.InjectToWorld(ref _worldBuilder);
        PhysicsSystemGroupSystemTest.InjectToWorld(ref _worldBuilder);
        PostPhysicsSystemGroupSystemTest.InjectToWorld(ref _worldBuilder);
        PostRenderingSystemGroupSystemTest.InjectToWorld(ref _worldBuilder);
        PresentationSystemGroupSystemTest.InjectToWorld(ref _worldBuilder);
        SimulationSystemGroupSystemTest.InjectToWorld(ref _worldBuilder);
    }
    
    [Test]
    [TestCase(nameof(InitializationSystemGroup))]
    [TestCase(nameof(PhysicsSystemGroup))]
    [TestCase(nameof(PostPhysicsSystemGroup))]
    [TestCase(nameof(PostRenderingSystemGroup))]
    [TestCase(nameof(SimulationSystemGroup))]
    public void CreatesSystemDescriptorInEachGroup(string group)
    {
        var systemGroupWorld = _worldBuilder.Finish();
        var descriptor = systemGroupWorld.GenerateDescriptors();
        Assert.That(descriptor.Where(x => x.Name == group).Count(), Is.EqualTo(1));
    }
    
    [Test]
    public void CreatesAllSystemDescriptors()
    {
        var systemGroupWorld = _worldBuilder.Finish();
        var descriptor = systemGroupWorld.GenerateDescriptors();
        Assert.That(descriptor.Count, Is.EqualTo(6));
    }
}