﻿using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Tests.TestSetup1;
using NSubstitute;

namespace Arch.SystemGroups.Tests.ThrottleGroup;

public class ThrottleGroupTests
{
    private IPlayerLoop _playerLoop;
    private ThrottlePostRenderingSystem _postRenderingSystem;
    private ThrottleSimulationSystem _simulationSystem;

    private ArchSystemsWorldBuilder<TestWorld> _worldBuilder;

    [SetUp]
    public void SetUp()
    {
        _worldBuilder =
            new ArchSystemsWorldBuilder<TestWorld>(new TestWorld(),
                _playerLoop = Substitute.For<IPlayerLoop>());

        _simulationSystem = ThrottleSimulationSystem.InjectToWorld(ref _worldBuilder);
        _postRenderingSystem = ThrottlePostRenderingSystem.InjectToWorld(ref _worldBuilder);
    }

    [TearDown]
    public void TearDown()
    {
        PlayerLoopHelper.AggregatesCache.Clear();
    }

    [Test]
    public void BuildsHierarchy()
    {
        var world = _worldBuilder.Finish();
        
        AssertHelpers.AssertNodesEquivalency(world.SystemGroups.OfType<SimulationSystemGroup>().First().Nodes, typeof(ThrottleSimulationGroup));
        AssertHelpers.AssertNodesEquivalency(world.SystemGroups.OfType<PostRenderingSystemGroup>().First().Nodes, typeof(ThrottlePostRenderingGroup));
    }

    [Test]
    public void Throttles()
    {
        // Can't test it because Time.deltaTime can't be used outside of Unity: System.Security.SecurityException : ECall methods must be packaged into a system module.
        var systemGroups = new List<ISystemGroupAggregate>();
        
        _playerLoop.When(p => p.AddAggregate(typeof(SimulationSystemGroup), Arg.Any<ISystemGroupAggregate<SystemGroupAggregate.None>>()))
            .Do(c => systemGroups.Add(c.Arg<ISystemGroupAggregate<SystemGroupAggregate.None>>()));
        
        _playerLoop.When(p => p.AddAggregate(typeof(PostRenderingSystemGroup), Arg.Any<ISystemGroupAggregate<SystemGroupAggregate.None>>()))
            .Do(c => systemGroups.Add(c.Arg<ISystemGroupAggregate<SystemGroupAggregate.None>>()));
        
        var world = _worldBuilder.Finish();
        world.Initialize();

        for (var i = 0; i < 10; i++)
        {
            foreach (var systemGroup in systemGroups)
            {
                systemGroup.TriggerUpdate();
            }
        }
        
        Assert.That(_simulationSystem.UpdateCount, Is.EqualTo(5));
        Assert.That(_postRenderingSystem.UpdateCount, Is.EqualTo(5));
        Assert.That(_simulationSystem.BeforeUpdateCount, Is.EqualTo(5));
        Assert.That(_postRenderingSystem.BeforeUpdateCount, Is.EqualTo(5));
        Assert.That(_simulationSystem.AfterUpdateCount, Is.EqualTo(5));
        Assert.That(_postRenderingSystem.AfterUpdateCount, Is.EqualTo(5));
    }

    [Test]
    public void ThrowsIfNotInjectedManually()
    {
        Assert.Throws<GroupNotFoundException>(() => ParametrisedThrottleSystem.InjectToWorld(ref _worldBuilder));
        _worldBuilder.Finish();
    }

    [Test]
    public void ThrottlesParametrizedSystem([Values(1, 2, 5, 10)] int framesToSkip)
    {
        const int framesToRun = 100;

        _worldBuilder.InjectCustomGroup(new ParametrisedThrottleGroup(framesToSkip));

        var system = ParametrisedThrottleSystem.InjectToWorld(ref _worldBuilder);
        
        var systemGroups = new List<ISystemGroupAggregate>();

        _playerLoop.When(p => p.AddAggregate(typeof(SimulationSystemGroup), Arg.Any<ISystemGroupAggregate<SystemGroupAggregate.None>>()))
            .Do(c => systemGroups.Add(c.Arg<ISystemGroupAggregate<SystemGroupAggregate.None>>()));
        
        var world = _worldBuilder.Finish();
        world.Initialize();
        
        for (var i = 0; i < framesToRun; i++)
        {
            foreach (var systemGroup in systemGroups)
            {
                systemGroup.TriggerUpdate();
            }
        }
        
        var expectedUpdateCount = 1 + (framesToRun - 1) / (framesToSkip + 1);
        
        Assert.That(system.UpdateCount, Is.EqualTo(expectedUpdateCount));
        Assert.That(system.BeforeUpdateCount, Is.EqualTo(expectedUpdateCount));
        Assert.That(system.AfterUpdateCount, Is.EqualTo(expectedUpdateCount));
    }
}