using Arch.SystemGroups.DefaultSystemGroups;
using NSubstitute;

namespace Arch.SystemGroups.Tests.PlayerLoopTests;

public class PlayerLoopHelperTests
{
    private readonly Dictionary<Type, ISystemGroupAggregate<int>> _aggregates = new();
    private ISystemGroupAggregate<int>.IFactory _factory;
    private IPlayerLoop _playerLoop;

    [SetUp]
    public void SetUp()
    {
        _playerLoop = Substitute.For<IPlayerLoop>();
        _factory = Substitute.For<ISystemGroupAggregate<int>.IFactory>();
        _factory.Create(Arg.Any<Type>())
            .Returns(c => _aggregates[c.Arg<Type>()] = Substitute.For<ISystemGroupAggregate<int>>());
    }

    [TearDown]
    public void TearDown()
    {
        _aggregates.Clear();
        PlayerLoopHelper.AggregatesCache.Clear();
    }

    [Test]
    public void CachesAggregates()
    {
        const int iterations = 5;

        for (var i = 0; i < iterations; i++)
        {
            _playerLoop.AppendWorldToCurrentPlayerLoop(_factory, i,
                new InitializationSystemGroup(new List<ExecutionNode<float>>(), null, null),
                new SimulationSystemGroup(new List<ExecutionNode<float>>(), null, null),
                new PresentationSystemGroup(new List<ExecutionNode<float>>(), null, null),
                new PostRenderingSystemGroup(new List<ExecutionNode<float>>(), null, null),
                new PhysicsSystemGroup(new List<ExecutionNode<float>>(), null, null),
                new PostPhysicsSystemGroup(new List<ExecutionNode<float>>(), null, null));
        }

        // only one aggregate must be created

        _factory.Received(1).Create(typeof(InitializationSystemGroup));
        _factory.Received(1).Create(typeof(SimulationSystemGroup));
        _factory.Received(1).Create(typeof(PresentationSystemGroup));
        _factory.Received(1).Create(typeof(PostRenderingSystemGroup));
        _factory.Received(1).Create(typeof(PhysicsSystemGroup));
        _factory.Received(1).Create(typeof(PostPhysicsSystemGroup));
    }

    [Test]
    public void AddsSystemGroupWithDataToAggregate()
    {
        const int iterations = 5;

        for (var i = 0; i < iterations; i++)
        {
            _playerLoop.AppendWorldToCurrentPlayerLoop(_factory, i,
                new InitializationSystemGroup(new List<ExecutionNode<float>>(), null, null),
                new SimulationSystemGroup(new List<ExecutionNode<float>>(), null, null),
                new PresentationSystemGroup(new List<ExecutionNode<float>>(), null, null),
                new PostRenderingSystemGroup(new List<ExecutionNode<float>>(), null, null),
                new PhysicsSystemGroup(new List<ExecutionNode<float>>(), null, null),
                new PostPhysicsSystemGroup(new List<ExecutionNode<float>>(), null, null));
        }

        for (var i = 0; i < iterations; i++)
        {
            _aggregates[typeof(InitializationSystemGroup)].Received(1).Add(i, Arg.Any<InitializationSystemGroup>());
            _aggregates[typeof(SimulationSystemGroup)].Received(1).Add(i, Arg.Any<SimulationSystemGroup>());
            _aggregates[typeof(PresentationSystemGroup)].Received(1).Add(i, Arg.Any<PresentationSystemGroup>());
            _aggregates[typeof(PostRenderingSystemGroup)].Received(1).Add(i, Arg.Any<PostRenderingSystemGroup>());
            _aggregates[typeof(PhysicsSystemGroup)].Received(1).Add(i, Arg.Any<PhysicsSystemGroup>());
            _aggregates[typeof(PostPhysicsSystemGroup)].Received(1).Add(i, Arg.Any<PostPhysicsSystemGroup>());
        }
    }

    [Test]
    public void RemovesSystemGroupFromAggregate()
    {
        InitializationSystemGroup initializationSystemGroup;
        SimulationSystemGroup simulationSystemGroup;
        PresentationSystemGroup presentationSystemGroup;
        PostRenderingSystemGroup postRenderingSystemGroup;
        PhysicsSystemGroup physicsSystemGroup;
        PostPhysicsSystemGroup postPhysicsSystemGroup;

        _playerLoop.AppendWorldToCurrentPlayerLoop(_factory, default,
            initializationSystemGroup = new InitializationSystemGroup(new List<ExecutionNode<float>>(), null, null),
            simulationSystemGroup = new SimulationSystemGroup(new List<ExecutionNode<float>>(), null, null),
            presentationSystemGroup = new PresentationSystemGroup(new List<ExecutionNode<float>>(), null, null),
            postRenderingSystemGroup = new PostRenderingSystemGroup(new List<ExecutionNode<float>>(), null, null),
            physicsSystemGroup = new PhysicsSystemGroup(new List<ExecutionNode<float>>(), null, null),
            postPhysicsSystemGroup = new PostPhysicsSystemGroup(new List<ExecutionNode<float>>(), null, null));
        
        foreach (var systemGroupAggregate in _aggregates.Values)
            systemGroupAggregate.Count.Returns(2);

        _playerLoop.RemoveFromPlayerLoop(_factory, initializationSystemGroup);
        _playerLoop.RemoveFromPlayerLoop(_factory, simulationSystemGroup);
        _playerLoop.RemoveFromPlayerLoop(_factory, presentationSystemGroup);
        _playerLoop.RemoveFromPlayerLoop(_factory, postRenderingSystemGroup);
        _playerLoop.RemoveFromPlayerLoop(_factory, physicsSystemGroup);
        _playerLoop.RemoveFromPlayerLoop(_factory, postPhysicsSystemGroup);

        _aggregates[typeof(InitializationSystemGroup)].Received(1).Remove(initializationSystemGroup);
        _aggregates[typeof(SimulationSystemGroup)].Received(1).Remove(simulationSystemGroup);
        _aggregates[typeof(PresentationSystemGroup)].Received(1).Remove(presentationSystemGroup);
        _aggregates[typeof(PostRenderingSystemGroup)].Received(1).Remove(postRenderingSystemGroup);
        _aggregates[typeof(PhysicsSystemGroup)].Received(1).Remove(physicsSystemGroup);
        _aggregates[typeof(PostPhysicsSystemGroup)].Received(1).Remove(postPhysicsSystemGroup);
    }

    [Test]
    public void RemovesAggregateWhenEmpty()
    {
        var systemGroups = new List<SystemGroup>();

        for (var i = 0; i < 2; i++)
        {
            var initializationSystemGroup = new InitializationSystemGroup(new List<ExecutionNode<float>>(), null, null);
            var simulationSystemGroup = new SimulationSystemGroup(new List<ExecutionNode<float>>(), null, null);
            var presentationSystemGroup = new PresentationSystemGroup(new List<ExecutionNode<float>>(), null, null);
            var postRenderingSystemGroup = new PostRenderingSystemGroup(new List<ExecutionNode<float>>(), null, null);
            var physicsSystemGroup = new PhysicsSystemGroup(new List<ExecutionNode<float>>(), null, null);
            var postPhysicsSystemGroup = new PostPhysicsSystemGroup(new List<ExecutionNode<float>>(), null, null);

            systemGroups.Add(initializationSystemGroup);
            systemGroups.Add(simulationSystemGroup);
            systemGroups.Add(presentationSystemGroup);
            systemGroups.Add(postRenderingSystemGroup);
            systemGroups.Add(physicsSystemGroup);
            systemGroups.Add(postPhysicsSystemGroup);

            _playerLoop.AppendWorldToCurrentPlayerLoop(_factory, default,
                initializationSystemGroup, simulationSystemGroup, presentationSystemGroup,
                postRenderingSystemGroup, physicsSystemGroup, postPhysicsSystemGroup);
        }

        foreach (var systemGroupAggregate in _aggregates.Values)
        {
            systemGroupAggregate.Received(2).Add(Arg.Any<int>(), Arg.Any<SystemGroup>());
            systemGroupAggregate.Count.Returns(2);
        }

        // First iteration
        for (var i = 0; i < systemGroups.Count / 2; i++)
        {
            _playerLoop.RemoveFromPlayerLoop(_factory, systemGroups[i]);
        }

        _playerLoop.DidNotReceive().RemoveAggregate(Arg.Any<ISystemGroupAggregate>());
        
        foreach (var systemGroupAggregate in _aggregates.Values)
        {
            systemGroupAggregate.Count.Returns(0);
        }

        // Second iteration
        for (var i = 0; i < systemGroups.Count; i++)
        {
            _playerLoop.RemoveFromPlayerLoop(_factory, systemGroups[i]);
        }

        _playerLoop.Received(1).RemoveAggregate(_aggregates[typeof(InitializationSystemGroup)]);
        _playerLoop.Received(1).RemoveAggregate(_aggregates[typeof(SimulationSystemGroup)]);
        _playerLoop.Received(1).RemoveAggregate(_aggregates[typeof(PresentationSystemGroup)]);
        _playerLoop.Received(1).RemoveAggregate(_aggregates[typeof(PostRenderingSystemGroup)]);
        _playerLoop.Received(1).RemoveAggregate(_aggregates[typeof(PhysicsSystemGroup)]);
        _playerLoop.Received(1).RemoveAggregate(_aggregates[typeof(PostPhysicsSystemGroup)]);
        
        Assert.That(PlayerLoopHelper.AggregatesCache, Is.Empty);
    }
}