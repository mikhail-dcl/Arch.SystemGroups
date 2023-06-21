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
        var firstLevel = simGroup.Nodes;
        
        AssertHelpers.AssertNodesEquivalency(firstLevel, typeof(RootGroup));
        
        var secondLevel = firstLevel.Find<RootGroup>().Nodes;
        AssertHelpers.AssertNodesEquivalency(secondLevel, typeof(NestedGroup1));

        var thirdLevel = secondLevel.Find<NestedGroup1>().Nodes;
        AssertHelpers.AssertNodesEquivalency(thirdLevel, typeof(NestedGroup2));
        
        var forthLevel = thirdLevel.Find<NestedGroup2>().Nodes;
        AssertHelpers.AssertNodesEquivalency(forthLevel, typeof(SystemInNestedGroup));
    } 
}