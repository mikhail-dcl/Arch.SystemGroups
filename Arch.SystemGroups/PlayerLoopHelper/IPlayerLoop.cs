using System;

namespace Arch.SystemGroups
{
    /// <summary>
    /// Abstraction needed for Mocking or providing a custom implementation of injection into the Player Loop
    /// </summary>
    public interface IPlayerLoop
    {
        /// <summary>
        /// Called before all other methods once for each world
        /// </summary>
        void OnWorldStartAppending();

        /// <summary>
        /// Called after all <see cref="AddAggregate{T}"/> and <see cref="RemoveAggregate"/>
        /// </summary>
        void OnWorldEndAppending();
    
        /// <summary>
        /// Adds an aggregate of system groups to the player loop. It is called only once upon the first mentioning of <paramref name="systemGroupType"/>.
        /// </summary>
        void AddAggregate<T>(Type systemGroupType, ISystemGroupAggregate<T> aggregate);

        /// <summary>
        /// Removes the given system group from the Unity Player Loop.
        /// </summary>
        /// <param name="aggregate"></param>
        void RemoveAggregate(ISystemGroupAggregate aggregate);
    }
}
