using System.Reflection;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;
using Arch.SystemGroups.Tests.TestSetup1.Groups;
using Arch.SystemGroups.Tests.TestSetup1.Systems;
using NSubstitute;

namespace Arch.SystemGroups.Tests.RedundantDependencies;

public class RedundantDependenciesTests
{
    private ArchSystemsWorldBuilder<TestWorld> _worldBuilder;
    private IPlayerLoop? _playerLoop;
    
    private RDSystem1 _rdSystem1;
    private RDSystem2 _rdSystem2;
    private RDSystem3 _rdSystem3;
    private RDSystem4 _rdSystem4;

    [SetUp]
    public void SetUp()
    {
        _worldBuilder =
            new ArchSystemsWorldBuilder<TestWorld>(new TestWorld(),
                _playerLoop = Substitute.For<IPlayerLoop>());

        _rdSystem3 = RDSystem3.InjectToWorld(ref _worldBuilder);
        _rdSystem1 = RDSystem1.InjectToWorld(ref _worldBuilder);
        _rdSystem4 = RDSystem4.InjectToWorld(ref _worldBuilder);
        _rdSystem2 = RDSystem2.InjectToWorld(ref _worldBuilder);
    }

    [Test]
    public void Sorts()
    {
        var world = _worldBuilder.Finish();

        var systemGroup = world.SystemGroups.OfType<SimulationSystemGroup>().First();
        var nodes = systemGroup.Nodes;
        
        AssertHelpers.AssertNodesEquality(nodes, typeof(RDSystem1), typeof(RDSystem2), typeof(RDSystem3), typeof(RDSystem4));
    }

    [Test]
    [TestCase(typeof(RDSystem1), new[] { typeof(RDSystem2), typeof(RDSystem4) }, new Type[0])]
    [TestCase(typeof(RDSystem2), new[] { typeof(RDSystem3) }, new Type[0])]
    [TestCase(typeof(RDSystem3), new[] { typeof(RDSystem4) }, new[] { typeof(RDSystem1), typeof(RDSystem2) })]
    [TestCase(typeof(RDSystem4), new Type[0], new[] { typeof(RDSystem2) })]
    public void RespectsAllEdges(Type system, Type[] updateBefore, Type[] updateAfter)
    {
        var map = new Dictionary<Type, List<Type>>();
        
        // find method by reflection
        var addEdgesMethod = system.GetMethod("AddEdges", BindingFlags.NonPublic | BindingFlags.Static);
        addEdgesMethod!.Invoke(null, new object[] { map });
        
        // Check update before
        if (updateBefore.Length > 0)
        {
            Assert.That(map.TryGetValue(system, out var before), Is.True);
            Assert.That(before, Is.EquivalentTo(updateBefore));
        }
        
        // Check update after
        if (updateAfter.Length > 0)
        {
            foreach (var after in updateAfter)
            {
                Assert.That(map.TryGetValue(after, out var afterList), Is.True);
                Assert.That(afterList, Contains.Item(system));
            }
        }
    }
}