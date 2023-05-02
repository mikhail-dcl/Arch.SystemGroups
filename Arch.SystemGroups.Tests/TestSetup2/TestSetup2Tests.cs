using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup2.Groups;
using Arch.SystemGroups.Tests.TestSetup2.Systems;
using NSubstitute;

namespace Arch.SystemGroups.Tests.TestSetup2;

public class TestSetup2Tests
{
    private ArchSystemsWorldBuilder<TestWorld2> _worldBuilder;
    private IUnityPlayerLoopHelper? _loopHelper;
    
    [SetUp]
    public void Setup()
    {
        _worldBuilder = new ArchSystemsWorldBuilder<TestWorld2>(new TestWorld2(), _loopHelper = Substitute.For<IUnityPlayerLoopHelper>());
        
        /*_worldBuilder
            .AddSystemCGroupAA()
            .AddSystemCGroupAB()
            .AddSystemAGroupAA()
            .AddSystemAGroupAB()
            .AddSystemBGroupAA()
            .AddSystemBGroupAB()

            .AddSystemDGroupBA()
            .AddSystemCGroupBA()
            .AddSystemCGroupBAA()
            .AddSystemBGroupBA()
            .AddSystemBGroupBB()
            .AddSystemBGroupBAA()
            .AddSystemAGroupBA()
            .AddSystemAGroupBB()
            .AddSystemAGroupBAA()
            .AddSystemAGroupBAB();
        is equivalent to AddAllSystems()
            */

        _worldBuilder.AddAllSystems();
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
    public void SortsPhysicsSystemGroup()
    {
        var world = _worldBuilder.Finish();

        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<PhysicsSystemGroup, GroupAA, GroupAB>(world);

        Assert.Throws<InvalidOperationException>(() =>
            AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<PhysicsSystemGroup, GroupAB, GroupAC>(world));

        var physicsSystemGroup = world.SystemGroups.OfType<PhysicsSystemGroup>().First();
        
        // group AA
        var aaSystems = physicsSystemGroup.Systems.OfType<GroupAA>().First().Systems;
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupAA, SystemBGroupAA>(aaSystems);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupAA, SystemCGroupAA>(aaSystems);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemBGroupAA, SystemCGroupAA>(aaSystems);
        
        // group AB
        var abSystems = physicsSystemGroup.Systems.OfType<GroupAB>().First().Systems;
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupAB, SystemBGroupAB>(abSystems);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupAB, SystemCGroupAB>(abSystems);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemBGroupAB, SystemCGroupAB>(abSystems);
        
    }

    [Test]
    public void SortsPostRenderingSystemGroup()
    {
        var world = _worldBuilder.Finish();
        
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<PostRenderingSystemGroup, GroupBA, GroupBB>(world);
        Assert.Throws<InvalidOperationException>(() =>
            AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<PostRenderingSystemGroup, GroupBB, GroupBC>(world));

        var firstLevelGroups = world.SystemGroups.OfType<PostRenderingSystemGroup>().First().Systems;
        
        // group BA
        var ba = firstLevelGroups.OfType<GroupBA>().First().Systems;
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<GroupBAA, GroupBAB>(ba);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupBA, SystemBGroupBA>(ba);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupBA, SystemBGroupBA>(ba);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupBA, SystemCGroupBA>(ba);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemBGroupBA, SystemCGroupBA>(ba);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupBA, SystemDGroupBA>(ba);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemBGroupBA, SystemDGroupBA>(ba);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemCGroupBA, SystemDGroupBA>(ba);
        
        // group BB
        var bb = firstLevelGroups.OfType<GroupBB>().First().Systems;
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupBB, SystemBGroupBB>(bb);
        
        // group BAA
        var baa = ba.OfType<GroupBAA>().First().Systems;
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupBAA, SystemBGroupBAA>(baa);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupBAA, SystemCGroupBAA>(baa);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemBGroupBAA, SystemCGroupBAA>(baa);
    }
}