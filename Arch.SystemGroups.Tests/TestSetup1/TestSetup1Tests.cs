using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1.Groups;
using Arch.SystemGroups.Tests.TestSetup1.Systems;
using NSubstitute;

namespace Arch.SystemGroups.Tests.TestSetup1;

public class TestSetup1Tests
{
    private CustomSystem1 _customSystem1;
    private CustomSystem1InCustomGroup1 _customSystem1InCustomGroup1;
    private CustomSystemWithParameters1 _customSystemWithParameters1;
    private CustomSystemWithParameters2 _customSystemWithParameters2;
    private IUnityPlayerLoopHelper? _loopHelper;
    private ArchSystemsWorldBuilder<TestWorld> _worldBuilder;

    [SetUp]
    public void SetUp()
    {
        _worldBuilder =
            new ArchSystemsWorldBuilder<TestWorld>(new TestWorld(),
                _loopHelper = Substitute.For<IUnityPlayerLoopHelper>());

        _customSystem1 = CustomSystem1.InjectToWorld(ref _worldBuilder);
        _customSystemWithParameters1 = CustomSystemWithParameters1.InjectToWorld(ref _worldBuilder, "test", 1);
        _customSystemWithParameters2 =
            CustomSystemWithParameters2.InjectToWorld(ref _worldBuilder, new CustomClass1 { Value = 200 });
        _customSystem1InCustomGroup1 = CustomSystem1InCustomGroup1.InjectToWorld(ref _worldBuilder, 1.0, (f, i) => { });
    }

    [Test]
    public void Sorts()
    {
        var world = _worldBuilder.Finish();

        AssertHelpers
            .AssertOrderOfSystemIsLessThenOtherSystem<InitializationSystemGroup, CustomSystem1, CustomGroup1>(world);
        AssertHelpers
            .AssertOrderOfSystemIsLessThenOtherSystem<InitializationSystemGroup, CustomSystemWithParameters2,
                CustomSystem1>(world);
        AssertHelpers
            .AssertOrderOfSystemIsLessThenOtherSystem<InitializationSystemGroup, CustomSystemWithParameters1,
                CustomSystemWithParameters2>(world);
    }

    [Test]
    public void InitializesSystems()
    {
        var world = _worldBuilder.Finish();
        world.Initialize();

        Assert.That(_customSystem1.IsInitialized, Is.True);
        Assert.That(_customSystemWithParameters1.IsInitialized, Is.True);
        Assert.That(_customSystemWithParameters2.IsInitialized, Is.True);
        Assert.That(_customSystem1InCustomGroup1.IsInitialized, Is.True);
    }

    [Test]
    public void DisposesSystems()
    {
        var world = _worldBuilder.Finish();
        world.Dispose();

        Assert.That(_customSystem1.IsDisposed, Is.True);
        Assert.That(_customSystemWithParameters1.IsDisposed, Is.True);
        Assert.That(_customSystemWithParameters2.IsDisposed, Is.True);
        Assert.That(_customSystem1InCustomGroup1.IsDisposed, Is.True);
    }

    [Test]
    public void BuildsHierarchy()
    {
        var world = _worldBuilder.Finish();

        // In the initialization group there should be 3 systems and 1 group
        var initGroup = world.SystemGroups.OfType<InitializationSystemGroup>().First();
        var firstLevelSystems = initGroup.Nodes;

        AssertHelpers.AssertNodesEquivalency(firstLevelSystems, 
            typeof(CustomSystemWithParameters1),
            typeof(CustomSystemWithParameters2),
            typeof(CustomSystem1),
            typeof(CustomGroup1));

        var customGroup1 = firstLevelSystems.Find<CustomGroup1>();
        AssertHelpers.AssertNodesEquivalency(customGroup1.Nodes, typeof(CustomSystem1InCustomGroup1));
    }

    [Test]
    public void UnusedSystemGroupsAreEmpty()
    {
        var world = _worldBuilder.Finish();

        void AssertIsEmpty<T>() where T : SystemGroup
        {
            var group = world.SystemGroups.OfType<T>().First();
            Assert.That(group.Nodes, Is.Null);
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
                Arg.Any<InitializationSystemGroup>(), Arg.Any<SimulationSystemGroup>(),
                Arg.Any<PresentationSystemGroup>(),
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