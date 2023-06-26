using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.CentralizedThrottling.Systems;
using Arch.SystemGroups.Tests.TestSetup1;
using Arch.SystemGroups.Throttling;
using Arch.SystemGroups.UnityBridge;
using NSubstitute;

namespace Arch.SystemGroups.Tests.CentralizedThrottling;

public class CentralizedThrottlingTests
{
    private NoThrottlingRootSystem _noThrottlingRootSystem;
    private ThrottlingRootSystem _throttlingRootSystem;
    private ImplicitThrottlingExplicitNestedSystem _implicitThrottlingExplicitNestedSystem;
    private ImplicitThrottlingImplicitNestedSystem _implicitThrottlingImplicitNestedSystem;
    private NoThrottlingNestedSystem _noThrottlingNestedSystem;

    private IUpdateBasedSystemGroupThrottler _throttler;

    private ArchSystemsWorldBuilder<TestWorld> _worldBuilder;
    
    [SetUp]
    public void SetUp()
    {
        _throttler = Substitute.For<IUpdateBasedSystemGroupThrottler>();

        _worldBuilder = new ArchSystemsWorldBuilder<TestWorld>(new TestWorld(), Substitute.For<IUnityPlayerLoopHelper>(), updateBasedSystemGroupThrottler: _throttler);
        
        _noThrottlingRootSystem = NoThrottlingRootSystem.InjectToWorld(ref _worldBuilder);
        _throttlingRootSystem = ThrottlingRootSystem.InjectToWorld(ref _worldBuilder);
        _implicitThrottlingExplicitNestedSystem = ImplicitThrottlingExplicitNestedSystem.InjectToWorld(ref _worldBuilder);
        _implicitThrottlingImplicitNestedSystem = ImplicitThrottlingImplicitNestedSystem.InjectToWorld(ref _worldBuilder);
        _noThrottlingNestedSystem = NoThrottlingNestedSystem.InjectToWorld(ref _worldBuilder);
    }

    [Test]
    public void UpdatesAllSystemIfNotThrottled()
    {
        _throttler.ShouldThrottle(Arg.Any<Type>(), Arg.Any<TimeProvider.Info>()).Returns(false);

        var world = _worldBuilder.Finish();
        world.Initialize();

        world.SystemGroups.OfType<SimulationSystemGroup>().First().Update();
        
        AssertSystemUpdatesInvoked(_noThrottlingNestedSystem);
        AssertSystemUpdatesInvoked(_implicitThrottlingExplicitNestedSystem);
        AssertSystemUpdatesInvoked(_implicitThrottlingImplicitNestedSystem);
        AssertSystemUpdatesInvoked(_noThrottlingRootSystem);
        AssertSystemUpdatesInvoked(_throttlingRootSystem);
    }

    [Test]
    public void UpdatesNoThrottlingSystemsIfThrottled()
    {
        _throttler.ShouldThrottle(Arg.Any<Type>(), Arg.Any<TimeProvider.Info>()).Returns(true);

        var world = _worldBuilder.Finish();
        world.Initialize();

        world.SystemGroups.OfType<SimulationSystemGroup>().First().Update();
        
        AssertSystemUpdatesInvoked(_noThrottlingNestedSystem);
        AssertSystemUpdatesInvoked(_noThrottlingRootSystem);
    }

    [Test]
    public void DoesNotUpdateThrottlingSystemsIfThrottled()
    {
        _throttler.ShouldThrottle(Arg.Any<Type>(), Arg.Any<TimeProvider.Info>()).Returns(true);

        var world = _worldBuilder.Finish();

        world.SystemGroups.OfType<SimulationSystemGroup>().First().Update();
        
        AssertSystemUpdatesNotInvoked(_implicitThrottlingExplicitNestedSystem);
        AssertSystemUpdatesNotInvoked(_implicitThrottlingImplicitNestedSystem);
        AssertSystemUpdatesNotInvoked(_throttlingRootSystem);
    }

    private void AssertSystemUpdatesNotInvoked(CentralizedThrottlingTestSystemBase system)
    {
        Assert.Multiple(() =>
        {
            Assert.That(system.UpdateInvoked, Is.False);
            Assert.That(system.AfterUpdateInvoked, Is.False);
            Assert.That(system.BeforeUpdateInvoked, Is.False);
        });
    }

    private void AssertSystemUpdatesInvoked(CentralizedThrottlingTestSystemBase system)
    {
        Assert.Multiple(() =>
        {
            Assert.That(system.UpdateInvoked, Is.True);
            Assert.That(system.AfterUpdateInvoked, Is.True);
            Assert.That(system.BeforeUpdateInvoked, Is.True);
        });
    }
}