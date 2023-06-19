using System;
using System.Collections.Generic;
using Arch.System;
using Arch.SystemGroups.DefaultSystemGroups;
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
        public Dictionary<Type, ISystem<float>> Systems;
        public Dictionary<Type, List<Type>> Edges;
    }

    private readonly Dictionary<Type, GroupInfo> _groupsInfo;
    private readonly Dictionary<Type, CustomGroupBase<float>> _customGroups;

    private readonly IUnityPlayerLoopHelper _unityPlayerLoopHelper;

    /// <summary>
    ///     Create a systems builder for the given world
    /// </summary>
    /// <param name="world">ECS World (Normally "Arch.Core.World")</param>
    public ArchSystemsWorldBuilder(T world) : this(world, UnityPlayerLoopHelper.Wrapper.Instance)
    {
    }

    internal ArchSystemsWorldBuilder(T world, IUnityPlayerLoopHelper unityPlayerLoopHelper)
    {
        World = world;
        _unityPlayerLoopHelper = unityPlayerLoopHelper;

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
        Action<Dictionary<Type, List<Type>>> addToEdges) where TGroup : CustomGroupBase<float>, new()
    {
        if (_customGroups.ContainsKey(typeof(TGroup)))
            return this;

        var newGroup = new TGroup();

        _customGroups[typeof(TGroup)] = newGroup;

        return AddToGroup(newGroup, updateInGroupType, typeof(TGroup), addToEdges);
    }

    /// <summary>
    ///     Registers a group that is not created automatically
    /// </summary>
    public void TryRegisterGroup<TGroup>(Type updateInGroupType, Action<Dictionary<Type, List<Type>>> addToEdges)
        where TGroup : CustomGroupBase<float>
    {
        // Thr group should be injected in advance
        if (!_customGroups.TryGetValue(typeof(TGroup), out var customGroup))
            throw new GroupNotFoundException(typeof(TGroup));

        AddToGroup(customGroup, updateInGroupType, typeof(TGroup), addToEdges, false);
    }

    /// <summary>
    ///     Used by auto-generated code
    /// </summary>
    public ArchSystemsWorldBuilder<T> AddToGroup(ISystem<float> system, Type updateInGroupType, Type systemType,
        Action<Dictionary<Type, List<Type>>> addToEdges, bool assertGroupExists = true)
    {
        if (!_groupsInfo.TryGetValue(updateInGroupType, out var group))
            _groupsInfo[updateInGroupType] = group = new GroupInfo
            {
                Systems = DictionaryPool<Type, ISystem<float>>.Get(),
                Edges = DictionaryPool<Type, List<Type>>.Get()
            };

        if (group.Systems.ContainsKey(systemType))
        {
            if (assertGroupExists)
                throw new InvalidOperationException($"System {systemType} is already added to the group {updateInGroupType}.");
            return this;
        }

        group.Systems[systemType] = system;
        addToEdges(group.Edges);

        return this;
    }

    internal void AddCustomGroup<TGroup>(TGroup customGroup) where TGroup : CustomGroupBase<float>
    {
        _customGroups.Add(typeof(TGroup), customGroup);
    }

    /// <summary>
    ///     Finalize the builder and create a systems world
    /// </summary>
    /// <returns></returns>
    public SystemGroupWorld Finish()
    {
        var initializationSystemGroup = InitializationSystemGroup.Empty;
        var simulationSystemGroup = SimulationSystemGroup.Empty;
        var presentationSystemGroup = PresentationSystemGroup.Empty;
        var postRenderingSystemGroup = PostRenderingSystemGroup.Empty;
        var physicsSystemGroup = PhysicsSystemGroup.Empty;
        var postPhysicsSystemGroup = PostPhysicsSystemGroup.Empty;

        // how to detect detached systems?

        CreateSystemGroup(ref initializationSystemGroup, list => new InitializationSystemGroup(list));
        CreateSystemGroup(ref simulationSystemGroup, list => new SimulationSystemGroup(list));
        CreateSystemGroup(ref presentationSystemGroup, list => new PresentationSystemGroup(list));
        CreateSystemGroup(ref postRenderingSystemGroup, list => new PostRenderingSystemGroup(list));
        CreateSystemGroup(ref physicsSystemGroup, list => new PhysicsSystemGroup(list));
        CreateSystemGroup(ref postPhysicsSystemGroup, list => new PostPhysicsSystemGroup(list));

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

        _unityPlayerLoopHelper.AppendWorldToCurrentPlayerLoop(
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
        }, _unityPlayerLoopHelper);
    }

    private void CreateSystemGroup<TGroup>(ref TGroup group, Func<List<ISystem<float>>, TGroup> constructor)
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
        DictionaryPool<Type, ISystem<float>>.Release(groupInfo.Systems);

        foreach (var (_, edges) in groupInfo.Edges)
            ListPool<Type>.Release(edges);

        DictionaryPool<Type, List<Type>>.Release(groupInfo.Edges);
    }
}