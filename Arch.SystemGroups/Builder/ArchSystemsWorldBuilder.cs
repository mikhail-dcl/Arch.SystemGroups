using System;
using System.Collections.Generic;
using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Throttling;
using UnityEngine.Pool;

namespace Arch.SystemGroups;

/// <summary>
///     The builder of systems attached to the Unity Player Loop
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct ArchSystemsWorldBuilder<T>
{
    private struct GroupInfo
    {
        public Dictionary<Type, ExecutionNode<float>> Systems;
        public Dictionary<Type, List<Type>> Edges;
    }

    private readonly Dictionary<Type, GroupInfo> _groupsInfo;
    private readonly Dictionary<Type, CustomGroupBase<float>> _customGroups;

    private readonly IPlayerLoop _playerLoop;

    private readonly IFixedUpdateBasedSystemGroupThrottler _fixedUpdateBasedSystemGroupThrottler;
    private readonly IUpdateBasedSystemGroupThrottler _updateBasedSystemGroupThrottler;
    private readonly ISystemGroupExceptionHandler _exceptionHandler;

    /// <summary>
    ///     Create a systems builder for the given world
    /// </summary>
    /// <param name="world">ECS World (Normally "Arch.Core.World")</param>
    /// <param name="fixedUpdateBasedSystemGroupThrottler">Throttler for all Fixed Update based Systems</param>
    /// <param name="updateBasedSystemGroupThrottler">Throttler for all Update based Systems</param>
    /// <param name="exceptionHandler">Exception handler</param>
    public ArchSystemsWorldBuilder(T world, IFixedUpdateBasedSystemGroupThrottler fixedUpdateBasedSystemGroupThrottler = null,
        IUpdateBasedSystemGroupThrottler updateBasedSystemGroupThrottler = null,
        ISystemGroupExceptionHandler exceptionHandler = null) : this(world,
        UnityPlayerLoop.Instance, fixedUpdateBasedSystemGroupThrottler, updateBasedSystemGroupThrottler, exceptionHandler)
    {
    }

    internal ArchSystemsWorldBuilder(T world, IPlayerLoop playerLoop,
        IFixedUpdateBasedSystemGroupThrottler fixedUpdateBasedSystemGroupThrottler = null,
        IUpdateBasedSystemGroupThrottler updateBasedSystemGroupThrottler = null,
        ISystemGroupExceptionHandler exceptionHandler = null)
    {
        World = world;
        _playerLoop = playerLoop;
        _fixedUpdateBasedSystemGroupThrottler = fixedUpdateBasedSystemGroupThrottler;
        _updateBasedSystemGroupThrottler = updateBasedSystemGroupThrottler;
        _exceptionHandler = exceptionHandler;

        _groupsInfo = DictionaryPool<Type, GroupInfo>.Get();
        _customGroups = DictionaryPool<Type, CustomGroupBase<float>>.Get();
    }

    /// <summary>
    ///     Current World
    /// </summary>
    public T World { get; }

    /// <summary>
    ///     Creates Groups Automatically
    /// </summary>
    public ArchSystemsWorldBuilder<T> TryCreateGroup<TGroup>(Type updateInGroupType,
        Action<Dictionary<Type, List<Type>>> addToEdges, bool throttlingEnabled) where TGroup : CustomGroupBase<float>, new()
    {
        if (_customGroups.ContainsKey(typeof(TGroup)))
            return this;

        var newGroup = new TGroup();

        _customGroups[typeof(TGroup)] = newGroup;

        return AddToGroup(new ExecutionNode<float>(newGroup, throttlingEnabled), updateInGroupType, typeof(TGroup), addToEdges);
    }

    /// <summary>
    ///     Registers a group that is not created automatically
    /// </summary>
    public void TryRegisterGroup<TGroup>(Type updateInGroupType, Action<Dictionary<Type, List<Type>>> addToEdges, bool throttlingEnabled)
        where TGroup : CustomGroupBase<float>
    {
        // Thr group should be injected in advance
        if (!_customGroups.TryGetValue(typeof(TGroup), out var customGroup))
            throw new GroupNotFoundException(typeof(TGroup));

        AddToGroup(new ExecutionNode<float>(customGroup, throttlingEnabled), updateInGroupType, typeof(TGroup), addToEdges, false);
    }

    /// <summary>
    ///     Used by auto-generated code
    /// </summary>
    public ArchSystemsWorldBuilder<T> AddToGroup(ISystem<float> system, Type updateInGroupType, Type systemType,
        Action<Dictionary<Type, List<Type>>> addToEdges, bool throttlingEnabled, bool assertGroupExists = true)
    {
        return AddToGroup(new ExecutionNode<float>(system, throttlingEnabled), updateInGroupType, systemType, addToEdges,
            assertGroupExists);
    }

    private ArchSystemsWorldBuilder<T> AddToGroup(ExecutionNode<float> node, Type updateInGroupType, Type systemType,
        Action<Dictionary<Type, List<Type>>> addToEdges, bool assertGroupExists = true)
    {
        if (!_groupsInfo.TryGetValue(updateInGroupType, out var group))
            _groupsInfo[updateInGroupType] = group = new GroupInfo
            {
                Systems = DictionaryPool<Type, ExecutionNode<float>>.Get(),
                Edges = DictionaryPool<Type, List<Type>>.Get()
            };

        if (group.Systems.ContainsKey(systemType))
        {
            if (assertGroupExists)
                throw new InvalidOperationException(
                    $"System {systemType} is already added to the group {updateInGroupType}.");
            return this;
        }

        group.Systems[systemType] = node;
        addToEdges(group.Edges);

        return this;
    }

    internal void AddCustomGroup<TGroup>(TGroup customGroup) where TGroup : CustomGroupBase<float>
    {
        _customGroups.Add(typeof(TGroup), customGroup);
    }

    
    /// <summary>
    /// Finalize the builder and create a systems world
    /// </summary>
    public SystemGroupWorld Finish()
    {
        return Finish(SystemGroupAggregate.Factory.Instance, default);
    }

    /// <summary>
    ///     Finalize the builder and create a systems world according to the custom aggregation mechanism
    /// </summary>
    /// <param name="aggregateFactory">factory for custom aggregation</param>
    /// <param name="aggregationData">data for custom aggregation</param>
    /// <typeparam name="TAggregationData">Type of aggregation data</typeparam>
    /// <exception cref="GroupNotFoundException"></exception>
    public SystemGroupWorld Finish<TAggregationData>(ISystemGroupAggregate<TAggregationData>.IFactory aggregateFactory, TAggregationData aggregationData)
    {
        var initializationSystemGroup = InitializationSystemGroup.Empty;
        var simulationSystemGroup = SimulationSystemGroup.Empty;
        var presentationSystemGroup = PresentationSystemGroup.Empty;
        var postRenderingSystemGroup = PostRenderingSystemGroup.Empty;
        var physicsSystemGroup = PhysicsSystemGroup.Empty;
        var postPhysicsSystemGroup = PostPhysicsSystemGroup.Empty;

        // how to detect detached systems?

        CreateSystemGroup(ref initializationSystemGroup, CreateInitializationSystemGroup);
        CreateSystemGroup(ref simulationSystemGroup, CreateSimulationSystemGroup);
        CreateSystemGroup(ref presentationSystemGroup, CreatePresentationSystemGroup);
        CreateSystemGroup(ref postRenderingSystemGroup, CreatePostRenderingSystemGroup);
        CreateSystemGroup(ref physicsSystemGroup, CreatePhysicsSystemGroup);
        CreateSystemGroup(ref postPhysicsSystemGroup, CreatePostPhysicsSystemGroup);

        // All remaining groups are empty at the moment
        // Fill them with systems

        foreach (var (systemType, groupInfo) in _groupsInfo)
        {
            if (!_customGroups.TryGetValue(systemType, out var customGroup))
                throw new GroupNotFoundException(systemType);

            customGroup.SetSystems(ArchSystemsSorter.SortSystems(groupInfo.Systems, groupInfo.Edges));
            CleanUpGroupInfo(in groupInfo);
        }

        DictionaryPool<Type, GroupInfo>.Release(_groupsInfo);
        DictionaryPool<Type, CustomGroupBase<float>>.Release(_customGroups);

        _playerLoop.AppendWorldToCurrentPlayerLoop(
            aggregateFactory,
            aggregationData,
            initializationSystemGroup,
            simulationSystemGroup,
            presentationSystemGroup,
            postRenderingSystemGroup,
            physicsSystemGroup,
            postPhysicsSystemGroup
        );

        return new SystemGroupWorld(new SystemGroup[]
        {
            initializationSystemGroup,
            simulationSystemGroup,
            presentationSystemGroup,
            postRenderingSystemGroup,
            physicsSystemGroup,
            postPhysicsSystemGroup
        }, _playerLoop, aggregateFactory);
    }

    private PostPhysicsSystemGroup CreatePostPhysicsSystemGroup(List<ExecutionNode<float>> list)
    {
        return new PostPhysicsSystemGroup(list, _fixedUpdateBasedSystemGroupThrottler, _exceptionHandler);
    }

    private PhysicsSystemGroup CreatePhysicsSystemGroup(List<ExecutionNode<float>> list)
    {
        return new PhysicsSystemGroup(list, _fixedUpdateBasedSystemGroupThrottler, _exceptionHandler);
    }

    private PostRenderingSystemGroup CreatePostRenderingSystemGroup(List<ExecutionNode<float>> list)
    {
        return new PostRenderingSystemGroup(list, _updateBasedSystemGroupThrottler, _exceptionHandler);
    }

    private PresentationSystemGroup CreatePresentationSystemGroup(List<ExecutionNode<float>> list)
    {
        return new PresentationSystemGroup(list, _updateBasedSystemGroupThrottler, _exceptionHandler);
    }

    private SimulationSystemGroup CreateSimulationSystemGroup(List<ExecutionNode<float>> list)
    {
        return new SimulationSystemGroup(list, _updateBasedSystemGroupThrottler, _exceptionHandler);
    }

    private InitializationSystemGroup CreateInitializationSystemGroup(List<ExecutionNode<float>> list)
    {
        return new InitializationSystemGroup(list, _updateBasedSystemGroupThrottler, _exceptionHandler);
    }

    private void CreateSystemGroup<TGroup>(ref TGroup group, Func<List<ExecutionNode<float>>, TGroup> constructor)
        where TGroup : SystemGroup
    {
        if (_groupsInfo.TryGetValue(typeof(TGroup), out var groupsInfo))
        {
            group = constructor(ArchSystemsSorter.SortSystems(groupsInfo.Systems, groupsInfo.Edges));

            CleanUpGroupInfo(in groupsInfo);
            _groupsInfo.Remove(typeof(TGroup));
        }
    }

    private void CleanUpGroupInfo(in GroupInfo groupInfo)
    {
        DictionaryPool<Type, ExecutionNode<float>>.Release(groupInfo.Systems);

        foreach (var (_, edges) in groupInfo.Edges)
            ListPool<Type>.Release(edges);

        DictionaryPool<Type, List<Type>>.Release(groupInfo.Edges);
    }
}