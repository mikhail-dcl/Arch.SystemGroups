using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Descriptors;
using Arch.SystemGroups.Tests.NestedGroups;
using Arch.SystemGroups.Tests.TestSetup1;
using NSubstitute;

namespace Arch.SystemGroups.Tests.DescriptorTests;

public class NestedDescriptorTests
{
    private ArchSystemsWorldBuilder<TestWorld> _worldBuilder;
    
    [SetUp]
    public void SetUp()
    {
        _worldBuilder = new ArchSystemsWorldBuilder<TestWorld>(new TestWorld(), Substitute.For<IPlayerLoop>());
        SystemInNestedGroup.InjectToWorld(ref _worldBuilder);
    }
    [Test]
    public void NestedSystemGroupsAreCreated()
    {
        var systemGroupWorld = _worldBuilder.Finish();
        var simulationSystemGroup =
            systemGroupWorld.GenerateDescriptor().FirstOrDefault(x => x.Name == nameof(SimulationSystemGroup));
        var root = simulationSystemGroup.Groups.First();
        var firstLevel = root.Groups.First();
        var secondLevel = firstLevel.Groups.First();
        
        Assert.That(root.Name, Is.EqualTo(nameof(RootGroup)));
        Assert.That(firstLevel.Name, Is.EqualTo(nameof(NestedGroup1)));
        Assert.That(secondLevel.Name, Is.EqualTo(nameof(NestedGroup2)));
        Assert.That(secondLevel.Systems.First().Name, Is.EqualTo(nameof(SystemInNestedGroup)));
    }
}