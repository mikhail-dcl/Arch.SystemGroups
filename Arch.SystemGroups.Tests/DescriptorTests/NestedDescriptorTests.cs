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
            systemGroupWorld.GenerateDescriptors().FirstOrDefault(x => x.Name == nameof(SimulationSystemGroup));
        var root = simulationSystemGroup.SubDescriptors.First();
        var firstLevel = root.SubDescriptors.First();
        var secondLevel = firstLevel.SubDescriptors.First();
        
        Assert.That(root.Name, Is.EqualTo(nameof(RootGroup)));
        Assert.True(root.IsGroup);
        
        Assert.That(firstLevel.Name, Is.EqualTo(nameof(NestedGroup1)));
        Assert.True(firstLevel.IsGroup);
        
        Assert.That(secondLevel.Name, Is.EqualTo(nameof(NestedGroup2)));
        Assert.True(secondLevel.IsGroup);
        
        Assert.True(secondLevel.SubDescriptors.First().IsSystem);
        Assert.That(secondLevel.SubDescriptors.First().Name, Is.EqualTo(nameof(SystemInNestedGroup)));
    }
}