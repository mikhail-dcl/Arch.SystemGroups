using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup2.Groups;
using Arch.SystemGroups.Tests.TestSetup2.Systems;
using NSubstitute;

namespace Arch.SystemGroups.Tests.TestSetup2;

public class TestSetup2Tests
{
    private ArchSystemsWorldBuilder<TestWorld2> _worldBuilder;
    private IPlayerLoop? _loopHelper;
    
    [SetUp]
    public void Setup()
    {
        _worldBuilder = new ArchSystemsWorldBuilder<TestWorld2>(new TestWorld2(), _loopHelper = Substitute.For<IPlayerLoop>());
        
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
    public void SortsPhysicsSystemGroup()
    {
        var world = _worldBuilder.Finish();

        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<PhysicsSystemGroup, GroupAA, GroupAB>(world);

        Assert.Throws<InvalidOperationException>(() =>
            AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<PhysicsSystemGroup, GroupAB, GroupAC>(world));

        var physicsSystemGroup = world.SystemGroups.OfType<PhysicsSystemGroup>().First();
        
        // group AA
        var aaSystems = physicsSystemGroup.Nodes.Find<GroupAA>().Nodes;
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupAA, SystemBGroupAA>(aaSystems);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupAA, SystemCGroupAA>(aaSystems);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemBGroupAA, SystemCGroupAA>(aaSystems);
        
        // group AB
        var abSystems = physicsSystemGroup.Nodes.Find<GroupAB>().Nodes;
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

        var firstLevelGroups = world.SystemGroups.OfType<PostRenderingSystemGroup>().First().Nodes;
        
        // group BA
        var ba = firstLevelGroups.Find<GroupBA>().Nodes;
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<GroupBAA, GroupBAB>(ba);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupBA, SystemBGroupBA>(ba);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupBA, SystemBGroupBA>(ba);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupBA, SystemCGroupBA>(ba);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemBGroupBA, SystemCGroupBA>(ba);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupBA, SystemDGroupBA>(ba);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemBGroupBA, SystemDGroupBA>(ba);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemCGroupBA, SystemDGroupBA>(ba);
        
        // group BB
        var bb = firstLevelGroups.Find<GroupBB>().Nodes;
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupBB, SystemBGroupBB>(bb);
        
        // group BAA
        var baa = ba.Find<GroupBAA>().Nodes;
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupBAA, SystemBGroupBAA>(baa);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemAGroupBAA, SystemCGroupBAA>(baa);
        AssertHelpers.AssertOrderOfSystemIsLessThenOtherSystem<SystemBGroupBAA, SystemCGroupBAA>(baa);
    }
}