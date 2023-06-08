using System;
using System.Collections.Generic;
using Arch.SystemGroups.DefaultSystemGroups;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Arch.SystemGroups;

/// <summary>
/// Provides utilities to inject systems into the Unity Player Loop
/// </summary>
public static class UnityPlayerLoopHelper
{
    /// <summary>
    /// Determines whether the system should be added to the beginning or the end of the step of the player loop
    /// </summary>
    public enum AddMode : byte
    {
        /// <summary>
        /// Add the system to the beginning of the step
        /// </summary>
        Prepend,
        
        /// <summary>
        /// Add the system to the end of the step
        /// </summary>
        Append
    }

    /// <summary>
    /// Contains the list of system groups of the same type
    /// </summary>
    internal class SystemGroupAggregate
    {
        private readonly List<SystemGroup> _systemGroups = new (16);

        /// <summary>
        /// For debugging purpose only
        /// </summary>
        internal readonly Type GroupType;

        internal int Count => _systemGroups.Count;

        public SystemGroupAggregate(Type groupType)
        {
            GroupType = groupType;
        }
        
        public void TriggerUpdate()
        {
            for (var i = 0; i < _systemGroups.Count; i++)
            {
                _systemGroups[i].Update();
            }
        }

        public void Add(SystemGroup systemGroup)
        {
            _systemGroups.Add(systemGroup);
        }

        public void Remove(SystemGroup systemGroup)
        {
            _systemGroups.Remove(systemGroup);
        }
    }

    internal class Wrapper : IUnityPlayerLoopHelper
    {
        internal static readonly Wrapper Instance = new ();
        
        /// <summary>
        /// <inheritdoc cref="IUnityPlayerLoopHelper.AppendWorldToCurrentPlayerLoop"/>
        /// </summary>
        public void AppendWorldToCurrentPlayerLoop(InitializationSystemGroup initializationSystemGroup,
            SimulationSystemGroup simulationSystemGroup, PresentationSystemGroup presentationSystemGroup,
            PostRenderingSystemGroup postRenderingSystemGroup, PhysicsSystemGroup physicsSystemGroup,
            PostPhysicsSystemGroup postPhysicsSystemGroup)
        {
            UnityPlayerLoopHelper.AppendWorldToCurrentPlayerLoop(initializationSystemGroup, simulationSystemGroup,
                presentationSystemGroup, postRenderingSystemGroup, physicsSystemGroup, postPhysicsSystemGroup);
        }

        /// <summary>
        /// <inheritdoc cref="IUnityPlayerLoopHelper.RemoveFromCurrentPlayerLoop"/>
        /// </summary>
        public void RemoveFromCurrentPlayerLoop(SystemGroup systemGroup)
        {
            UnityPlayerLoopHelper.RemoveFromCurrentPlayerLoop(systemGroup);
        }
    }

    private static readonly Dictionary<Type, SystemGroupAggregate> Aggregates = new (6);

    /// <summary>
    /// <inheritdoc cref="IUnityPlayerLoopHelper.AppendWorldToCurrentPlayerLoop"/>
    /// </summary>
    public static void AppendWorldToCurrentPlayerLoop(
        InitializationSystemGroup initializationSystemGroup,
        SimulationSystemGroup simulationSystemGroup,
        PresentationSystemGroup presentationSystemGroup,
        PostRenderingSystemGroup postRenderingSystemGroup,
        PhysicsSystemGroup physicsSystemGroup,
        PostPhysicsSystemGroup postPhysicsSystemGroup)
    {
        var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
        
        AddSystemToPlayerLoop(initializationSystemGroup, ref playerLoop, typeof(Initialization), AddMode.Append);
        AddSystemToPlayerLoop(simulationSystemGroup, ref playerLoop, typeof(Update), AddMode.Append);
        AddSystemToPlayerLoop(presentationSystemGroup, ref playerLoop, typeof(PreLateUpdate), AddMode.Append);
        AddSystemToPlayerLoop(postRenderingSystemGroup, ref playerLoop, typeof(PostLateUpdate), AddMode.Append);
        AddSystemToPlayerLoop(physicsSystemGroup, ref playerLoop, typeof(FixedUpdate), AddMode.Prepend);
        AddSystemToPlayerLoop(postPhysicsSystemGroup, ref playerLoop, typeof(FixedUpdate), AddMode.Append);

        PlayerLoop.SetPlayerLoop(playerLoop);
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
    /// <param name="systemGroup">The ECS system to add to the player loop.</param>
    /// <param name="playerLoop">Existing player loop to modify (e.g. PlayerLoop.GetCurrentPlayerLoop())</param>
    /// <param name="playerLoopSystemType">The Type of the PlayerLoopSystem subsystem to which the ECS system should be appended.
    /// See the UnityEngine.PlayerLoop namespace for valid values.</param>
    /// <param name="addMode">Append or Prepend</param>
    public static void AddSystemToPlayerLoop(SystemGroup systemGroup, ref PlayerLoopSystem playerLoop, Type playerLoopSystemType, AddMode addMode)
    {
        if (systemGroup == null) return;

        var systemGroupType = systemGroup.GetType();
        
        // If there is no aggregate yet, add it
        if (!Aggregates.TryGetValue(systemGroupType, out var aggregate))
            Aggregates[systemGroupType] = aggregate = new SystemGroupAggregate(systemGroupType);

        if (!AppendToPlayerLoopList(systemGroup.GetType(), aggregate.TriggerUpdate, ref playerLoop, playerLoopSystemType, addMode))
            throw new ArgumentException($"Could not find PlayerLoopSystem with type={playerLoopSystemType}");
        
        aggregate.Add(systemGroup);
    }
    
    static bool AppendToPlayerLoopList(Type updateType, PlayerLoopSystem.UpdateFunction updateFunction, ref PlayerLoopSystem playerLoop, Type playerLoopSystemType, AddMode addMode)
    {
        if (updateType == null || updateFunction == null || playerLoopSystemType == null)
            return false;

        if (playerLoop.type == playerLoopSystemType)
        {
            var oldListLength = playerLoop.subSystemList?.Length ?? 0;
            var newSubsystemList = new PlayerLoopSystem[oldListLength + 1];
           
            switch (addMode)
            {
                case AddMode.Prepend:
                {
                    newSubsystemList[0] = new PlayerLoopSystem
                    {
                        type = updateType,
                        updateDelegate = updateFunction
                    };
                    for (var i = 0; i < oldListLength; ++i)
                        newSubsystemList[i + 1] = playerLoop.subSystemList[i];
                    break;
                }
                default:
                {
                    for (var i = 0; i < oldListLength; ++i)
                        newSubsystemList[i] = playerLoop.subSystemList[i];
                    newSubsystemList[oldListLength] = new PlayerLoopSystem
                    {
                        type = updateType,
                        updateDelegate = updateFunction
                    };
                    break;
                }
            }

            playerLoop.subSystemList = newSubsystemList;
            
            return true;
        }

        if (playerLoop.subSystemList != null)
        {
            for (var i = 0; i < playerLoop.subSystemList.Length; ++i)
            {
                if (AppendToPlayerLoopList(updateType, updateFunction, ref playerLoop.subSystemList[i], playerLoopSystemType, addMode))
                    return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Remove the system group from the player loop
    /// </summary>
    public static void RemoveFromCurrentPlayerLoop(SystemGroup systemGroup)
    {
        var systemGroupType = systemGroup.GetType();
        
        if (!Aggregates.TryGetValue(systemGroupType, out var aggregate))
            return;
        
        aggregate.Remove(systemGroup);

        if (aggregate.Count > 0)
            return;

        Aggregates.Remove(systemGroupType);
        
        // If there are no more system groups in the aggregate remove the aggregate itself
        var playerLoop = PlayerLoop.GetCurrentPlayerLoop();

        if (RemoveFromPlayerLoopList(aggregate, ref playerLoop))
            PlayerLoop.SetPlayerLoop(playerLoop);
    }
    
    private static bool RemoveFromPlayerLoopList(SystemGroupAggregate aggregate, ref PlayerLoopSystem playerLoop)
    {
        static bool IsSystemGroup(SystemGroupAggregate aggregate, ref PlayerLoopSystem playerLoopSystem)
        {
            return playerLoopSystem.updateDelegate?.Target == aggregate;
        }
        
        if (playerLoop.subSystemList == null || playerLoop.subSystemList.Length == 0)
            return false;

        var result = false;
        var newSubSystemList = new List<PlayerLoopSystem>(playerLoop.subSystemList.Length);
        for (var i = 0; i < playerLoop.subSystemList.Length; ++i)
        {
            ref var playerLoopSubSystem = ref playerLoop.subSystemList[i];
            result |= RemoveFromPlayerLoopList(aggregate, ref playerLoopSubSystem);
            if (!IsSystemGroup(aggregate, ref playerLoopSubSystem))
                newSubSystemList.Add(playerLoopSubSystem);
        }

        if (newSubSystemList.Count != playerLoop.subSystemList.Length)
        {
            playerLoop.subSystemList = newSubSystemList.ToArray();
            result = true;
        }
        return result;
    }
}