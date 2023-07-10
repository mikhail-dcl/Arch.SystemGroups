using System.Collections.Generic;
using Arch.SystemGroups.DefaultSystemGroups;

namespace Arch.SystemGroups;

/// <summary>
/// Provides utilities to inject systems into the Unity Player Loop
/// </summary>
public static class PlayerLoopHelper
{
    internal static readonly Dictionary<ISystemGroupAggregateFactory, SystemGroupAggregateCache> AggregatesCache = new (10);

    /// <summary>
    /// Append ECS world to the provided player loop, supports custom system group aggregates
    /// </summary>
    public static void AppendWorldToCurrentPlayerLoop<T>(
        this IPlayerLoop playerLoop,
        ISystemGroupAggregate<T>.IFactory systemGroupAggregateFactory,
        T data,
        InitializationSystemGroup initializationSystemGroup,
        SimulationSystemGroup simulationSystemGroup,
        PresentationSystemGroup presentationSystemGroup,
        PostRenderingSystemGroup postRenderingSystemGroup,
        PhysicsSystemGroup physicsSystemGroup,
        PostPhysicsSystemGroup postPhysicsSystemGroup)
    {
        playerLoop.OnWorldStartAppending();

        playerLoop.AddSystemToPlayerLoop(data, initializationSystemGroup, systemGroupAggregateFactory);
        playerLoop.AddSystemToPlayerLoop(data, simulationSystemGroup, systemGroupAggregateFactory);
        playerLoop.AddSystemToPlayerLoop(data, presentationSystemGroup, systemGroupAggregateFactory);
        playerLoop.AddSystemToPlayerLoop(data, postRenderingSystemGroup, systemGroupAggregateFactory);
        playerLoop.AddSystemToPlayerLoop(data, physicsSystemGroup, systemGroupAggregateFactory);
        playerLoop.AddSystemToPlayerLoop(data, postPhysicsSystemGroup, systemGroupAggregateFactory);
        
        playerLoop.OnWorldEndAppending();
    }

    /// <summary>
    /// Add an ECS system to a specific point in the Unity player loop, so that it is updated every frame.
    /// <para>The system groups are being inserted into their corresponding aggregate that is unique for each group type,
    /// the execution order of the system groups inside the aggregate is not guaranteed and must be expected to be used for independent worlds</para>
    /// </summary>
    /// <remarks>
    /// This function does not change the currently active player loop. If this behavior is desired, it's necessary
    /// to call PlayerLoop.SetPlayerLoop(playerLoop) after the systems have been removed.
    /// </remarks>
    /// <param name="data">Additional data per world</param>
    /// <param name="systemGroup">The ECS system to add to the player loop.</param>
    /// <param name="playerLoop">Existing player loop to modify (e.g. PlayerLoop.GetCurrentPlayerLoop())</param>
    /// <param name="systemGroupAggregateFactory">Factory of System Group Aggregates</param>
    private static void AddSystemToPlayerLoop<T>(this IPlayerLoop playerLoop, in T data, SystemGroup systemGroup, 
        ISystemGroupAggregate<T>.IFactory systemGroupAggregateFactory)
    {
        if (systemGroup == null) return;

        var systemGroupType = systemGroup.GetType();

        var aggregateCache = GetOrCreateAggregateCache(systemGroupAggregateFactory);
        
        // If there is no aggregate yet, add it
        if (!aggregateCache.TryGetValue(systemGroupType, out ISystemGroupAggregate<T> aggregate))
        {
            aggregate = aggregateCache.Add(systemGroupType, systemGroupAggregateFactory);
            playerLoop.AddAggregate(systemGroupType, aggregate);
        }
        
        aggregate.Add(data, systemGroup);
    }

    private static SystemGroupAggregateCache GetOrCreateAggregateCache(ISystemGroupAggregateFactory factory)
    {
        if (AggregatesCache.TryGetValue(factory, out var cache)) return cache;
        AggregatesCache[factory] = cache = new SystemGroupAggregateCache();
        return cache;
    }

    /// <summary>
    /// Remove the system group from the player loop
    /// </summary>
    public static void RemoveFromPlayerLoop(this IPlayerLoop playerLoop, ISystemGroupAggregateFactory systemGroupAggregateFactory, SystemGroup systemGroup)
    {
        if (!AggregatesCache.TryGetValue(systemGroupAggregateFactory, out var cache))
            return;
        
        var systemGroupType = systemGroup.GetType();
        cache.Remove(playerLoop, systemGroupType, systemGroup);

        if (cache.Count > 0)
            return;

        // Clean-up if there are no more system groups in the aggregate
        AggregatesCache.Remove(systemGroupAggregateFactory);
    }
}