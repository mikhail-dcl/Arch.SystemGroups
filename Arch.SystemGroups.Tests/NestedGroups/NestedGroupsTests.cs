using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;
using NSubstitute;

namespace Arch.SystemGroups.Tests.NestedGroups;

public class NestedGroupsTests
{
    private ArchSystemsWorldBuilder<TestWorld> _worldBuilder;
    private IUnityPlayerLoopHelper? _loopHelper;

    private SystemInNestedGroup _system;
    
    [SetUp]
    public void SetUp()
    {
        _worldBuilder =
            new ArchSystemsWorldBuilder<TestWorld>(new TestWorld(),
                _loopHelper = Substitute.For<IUnityPlayerLoopHelper>());
        
        _system = SystemInNestedGroup.InjectToWorld(ref _worldBuilder);
    }

    [Test]
    public void BuildsHierarchy()
    {
        var world = _worldBuilder.Finish();

        var simGroup = world.SystemGroups.OfType<SimulationSystemGroup>().First();
        var firstLevel = simGroup.Systems;
        
        CollectionAssert.AreEquivalent(new[] {typeof(RootGroup)}, firstLevel.Select(s => s.GetType()));
        
        var secondLevel = firstLevel.OfType<RootGroup>().First().Systems;
        CollectionAssert.AreEquivalent(new[] {typeof(NestedGroup1)}, secondLevel.Select(s => s.GetType()));

        var thirdLevel = secondLevel.OfType<NestedGroup1>().First().Systems;
        CollectionAssert.AreEquivalent(new[] {typeof(NestedGroup2)}, thirdLevel.Select(s => s.GetType()));
        
        var forthLevel = thirdLevel.OfType<NestedGroup2>().First().Systems;
        CollectionAssert.AreEquivalent(new[] {typeof(SystemInNestedGroup)}, forthLevel.Select(s => s.GetType()));
    } 
}