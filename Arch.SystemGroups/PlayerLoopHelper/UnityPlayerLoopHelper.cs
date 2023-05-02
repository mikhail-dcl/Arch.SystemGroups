using System;
using System.Collections.Generic;
using Arch.SystemGroups.DefaultSystemGroups;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Arch.SystemGroups;

public static class UnityPlayerLoopHelper
{
    public enum AddMode : byte
    {
        Prepend,
        Append
    }

    internal class DelegateWrapper
    {
        public DelegateWrapper(SystemGroup systemGroup)
        {
            SystemGroup = systemGroup;
        }

        internal SystemGroup SystemGroup { get; }
        
        public void TriggerUpdate()
        {
            SystemGroup.Update();
        }
    }

    internal class Wrapper : IUnityPlayerLoopHelper
    {
        internal static readonly Wrapper Instance = new ();
        
        public void AppendWorldToCurrentPlayerLoop(InitializationSystemGroup initializationSystemGroup,
            SimulationSystemGroup simulationSystemGroup, PresentationSystemGroup presentationSystemGroup,
            PostRenderingSystemGroup postRenderingSystemGroup, PhysicsSystemGroup physicsSystemGroup,
            PostPhysicsSystemGroup postPhysicsSystemGroup)
        {
            UnityPlayerLoopHelper.AppendWorldToCurrentPlayerLoop(initializationSystemGroup, simulationSystemGroup,
                presentationSystemGroup, postRenderingSystemGroup, physicsSystemGroup, postPhysicsSystemGroup);
        }

        public void RemoveFromCurrentPlayerLoop(SystemGroup systemGroup)
        {
            UnityPlayerLoopHelper.RemoveFromCurrentPlayerLoop(systemGroup);
        }
    }

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
        
        var wrapper = new DelegateWrapper(systemGroup);
        if (!AppendToPlayerLoopList(systemGroup.GetType(), wrapper.TriggerUpdate, ref playerLoop, playerLoopSystemType, addMode))
            throw new ArgumentException($"Could not find PlayerLoopSystem with type={playerLoopSystemType}");
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
    
    public static void RemoveFromCurrentPlayerLoop(SystemGroup systemGroup)
    {
        var playerLoop = PlayerLoop.GetCurrentPlayerLoop();

        if (RemoveFromPlayerLoopList(systemGroup, ref playerLoop))
            PlayerLoop.SetPlayerLoop(playerLoop);
    }
    
    private static bool RemoveFromPlayerLoopList(SystemGroup systemGroup, ref PlayerLoopSystem playerLoop)
    {
        static bool IsSystemGroup(SystemGroup systemGroup, ref PlayerLoopSystem playerLoopSystem)
        {
            return playerLoopSystem.updateDelegate?.Target is DelegateWrapper wrapper && wrapper.SystemGroup == systemGroup;
        }
        
        if (playerLoop.subSystemList == null || playerLoop.subSystemList.Length == 0)
            return false;

        var result = false;
        var newSubSystemList = new List<PlayerLoopSystem>(playerLoop.subSystemList.Length);
        for (var i = 0; i < playerLoop.subSystemList.Length; ++i)
        {
            ref var playerLoopSubSystem = ref playerLoop.subSystemList[i];
            result |= RemoveFromPlayerLoopList(systemGroup, ref playerLoopSubSystem);
            if (!IsSystemGroup(systemGroup, ref playerLoopSubSystem))
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