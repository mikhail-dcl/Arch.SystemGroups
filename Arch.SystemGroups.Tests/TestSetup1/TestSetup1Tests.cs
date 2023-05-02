using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1.Groups;
using Arch.SystemGroups.Tests.TestSetup1.Systems;
using NSubstitute;

namespace Arch.SystemGroups.Tests.TestSetup1;

public class TestSetup1Tests
{
    private ArchSystemsWorldBuilder<TestWorld> _worldBuilder;
    private IUnityPlayerLoopHelper? _loopHelper;

    [SetUp]
    public void SetUp()
    {
        _worldBuilder = new ArchSystemsWorldBuilder<TestWorld>(new TestWorld(), _loopHelper = Substitute.For<IUnityPlayerLoopHelper>());
        
        _worldBuilder
            .AddCustomSystem1()
            .AddCustomSystemWithParameters1("test", 1)
            .AddCustomSystemWithParameters2(new CustomClass1 {Value = 200})
            .AddCustomSystem1InCustomGroup1(1.0, (f, i) => { });
    }

    [Test]
    public void Sorts()
    {
        var world = _worldBuilder.Finish();

        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<InitializationSystemGroup, CustomSystem1, CustomGroup1>(world);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<InitializationSystemGroup, CustomSystemWithParameters2, CustomSystem1>(world);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<InitializationSystemGroup, CustomSystemWithParameters1, CustomSystemWithParameters2>(world);
    }

    [Test]
    public void BuildsHierarchy()
    {
        var world = _worldBuilder.Finish();
        
        // In the initialization group there should be 3 systems and 1 group
        var initGroup = world.SystemGroups.OfType<InitializationSystemGroup>().First();
        var firstLevelSystems = initGroup.Systems;
        
        CollectionAssert.AreEquivalent(new[]
        {
            typeof(CustomSystemWithParameters1),
            typeof(CustomSystemWithParameters2),
            typeof(CustomSystem1),
            typeof(CustomGroup1)
        }, firstLevelSystems.Select(system => system.GetType()));
        
        var customGroup1 = firstLevelSystems.OfType<CustomGroup1>().First();
        CollectionAssert.AreEquivalent(new[]
        {
            typeof(CustomSystem1InCustomGroup1)
        }, customGroup1.Systems.Select(system => system.GetType()));
    }

    [Test]
    public void UnusedSystemGroupsAreEmpty()
    {
        var world = _worldBuilder.Finish();

        void AssertIsEmpty<T>() where T : SystemGroup
        {
            var group = world.SystemGroups.OfType<T>().First();
            Assert.That(group.Systems.Count, Is.EqualTo(0));
        }
        
        AssertIsEmpty<PhysicsSystemGroup>();
        AssertIsEmpty<PostPhysicsSystemGroup>();
        AssertIsEmpty<PresentationSystemGroup>();
        AssertIsEmpty<PostRenderingSystemGroup>();
        AssertIsEmpty<SimulationSystemGroup>();
    }
    
    [Test]
    public void InvokesPlayerLoopHelper()
    {
        _worldBuilder.Finish();

        _loopHelper.Received(1)
            ?.AppendWorldToCurrentPlayerLoop(
            Arg.Any<InitializationSystemGroup>(), Arg.Any<SimulationSystemGroup>(), Arg.Any<PresentationSystemGroup>(),
            Arg.Any<PostRenderingSystemGroup>(), Arg.Any<PhysicsSystemGroup>(), Arg.Any<PostPhysicsSystemGroup>());
    }

    [Test]
    public void ThrowsExceptionIfRepetitiveSystemAdded()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            CustomSystemWithParameters2.InjectToWorld(ref _worldBuilder, new CustomClass1 { Value = 400 });
        });
    }

    [Test]
    public void Disposes()
    {
        // can't test as Pools are not mockable
    }
}