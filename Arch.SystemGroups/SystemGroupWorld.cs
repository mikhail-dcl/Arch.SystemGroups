using System;
using System.Collections.Generic;

namespace Arch.SystemGroups
{
    /// <summary>
    /// An entry point to the systems connected to the Unity Player Loop.
    /// </summary>
    public class SystemGroupWorld : IDisposable
    {
        private readonly IPlayerLoop _playerLoop;
    
        // Aggregate factory is used to create the aggregate for each system group type,
        // acts as a key in the SystemGroupAggregateCache
        private readonly ISystemGroupAggregateFactory _aggregateFactory;

        internal IReadOnlyList<SystemGroup> SystemGroups { get; }
    
        internal SystemGroupWorld(IReadOnlyList<SystemGroup> systemGroups, IPlayerLoop playerLoop, ISystemGroupAggregateFactory aggregateFactory)
        {
            _playerLoop = playerLoop;
            _aggregateFactory = aggregateFactory;
            SystemGroups = systemGroups;
        }

        /// <summary>
        /// Recursively Initialize all systems in the world according to their execution order
        /// </summary>
        public void Initialize()
        {
            for (var i = 0; i < SystemGroups.Count; i++)
            {
                SystemGroups[i].Initialize();
            }
        }

        /// <summary>
        /// Recursively Dispose all systems in the world according to their execution order.
        /// Remove all systems from the player loop
        /// </summary>
        public void Dispose()
        {
            for (var i = 0; i < SystemGroups.Count; i++)
            {
                SystemGroups[i].Dispose();
                _playerLoop.RemoveFromPlayerLoop(_aggregateFactory, SystemGroups[i]);
            }
        }
    }
}