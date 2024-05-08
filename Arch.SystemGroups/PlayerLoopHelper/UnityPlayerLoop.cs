using System;
using System.Collections.Generic;
using Arch.SystemGroups.DefaultSystemGroups;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Arch.SystemGroups
{
    /// <summary>
    ///     Single-threaded wrapper over Unity's PlayerLoop
    /// </summary>
    public class UnityPlayerLoop : IPlayerLoop
    {
        /// <summary>
        ///     Singleton instance
        /// </summary>
        public static readonly UnityPlayerLoop Instance = new();

        private PlayerLoopSystem _playerLoop;

        /// <summary>
        ///     <inheritdoc cref="IPlayerLoop.OnWorldStartAppending" />
        /// </summary>
        public void OnWorldStartAppending()
        {
            _playerLoop = PlayerLoop.GetCurrentPlayerLoop();
        }

        /// <summary>
        ///     <inheritdoc cref="IPlayerLoop.OnWorldEndAppending" />
        /// </summary>
        public void OnWorldEndAppending()
        {
            PlayerLoop.SetPlayerLoop(_playerLoop);
        }

        /// <summary>
        ///     <inheritdoc cref="IPlayerLoop.AddAggregate{T}" />
        /// </summary>
        public void AddAggregate<T>(Type systemGroupType, ISystemGroupAggregate<T> aggregate)
        {
            var (playerLoopSystemType, addMode) = GetPlayerLoopSystemType(systemGroupType);

            if (!AppendToPlayerLoopList(systemGroupType, aggregate.TriggerUpdate, ref _playerLoop, playerLoopSystemType,
                    addMode))
                throw new ArgumentException($"Could not find PlayerLoopSystem with type={playerLoopSystemType}");
        }

        /// <summary>
        ///     <inheritdoc cref="IPlayerLoop.RemoveAggregate" />
        /// </summary>
        /// <param name="aggregate"></param>
        public void RemoveAggregate(ISystemGroupAggregate aggregate)
        {
            // If there are no more system groups in the aggregate remove the aggregate itself
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();

            if (RemoveFromPlayerLoopList(aggregate, ref playerLoop))
                PlayerLoop.SetPlayerLoop(playerLoop);
        }

        private static (Type, PlayerLoopAddMode) GetPlayerLoopSystemType(Type systemGroupType)
        {
            if (systemGroupType == typeof(InitializationSystemGroup))
                return (typeof(Initialization), PlayerLoopAddMode.Append);

            if (systemGroupType == typeof(SimulationSystemGroup))
                return (typeof(Update), PlayerLoopAddMode.Append);

            if (systemGroupType == typeof(PresentationSystemGroup))
                return (typeof(PreLateUpdate), PlayerLoopAddMode.Append);
            
            if (systemGroupType == typeof(PreRenderingSystemGroup))
                return (typeof(PostLateUpdate), PlayerLoopAddMode.Prepend);

            if (systemGroupType == typeof(PostRenderingSystemGroup))
                return (typeof(PostLateUpdate), PlayerLoopAddMode.Append);

            if (systemGroupType == typeof(PhysicsSystemGroup))
                return (typeof(FixedUpdate), PlayerLoopAddMode.Prepend);

            if (systemGroupType == typeof(PostPhysicsSystemGroup))
                return (typeof(FixedUpdate), PlayerLoopAddMode.Append);

            throw new ArgumentException($"Could not find PlayerLoopSystem for system group {systemGroupType}");
        }

        private static bool AppendToPlayerLoopList(Type updateType, PlayerLoopSystem.UpdateFunction updateFunction,
            ref PlayerLoopSystem playerLoop, Type playerLoopSystemType, PlayerLoopAddMode addMode)
        {
            if (updateType == null || updateFunction == null || playerLoopSystemType == null)
                return false;

            if (playerLoop.type == playerLoopSystemType)
            {
                var oldListLength = playerLoop.subSystemList?.Length ?? 0;
                var newSubsystemList = new PlayerLoopSystem[oldListLength + 1];

                switch (addMode)
                {
                    case PlayerLoopAddMode.Prepend:
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
                    if (AppendToPlayerLoopList(updateType, updateFunction, ref playerLoop.subSystemList[i],
                            playerLoopSystemType, addMode))
                        return true;
                }
            }

            return false;
        }

        private static bool RemoveFromPlayerLoopList(ISystemGroupAggregate aggregate, ref PlayerLoopSystem playerLoop)
        {
            static bool IsSystemGroup(ISystemGroupAggregate aggregate, ref PlayerLoopSystem playerLoopSystem)
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
}