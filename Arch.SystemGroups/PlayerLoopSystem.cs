using Arch.System;
using Arch.SystemGroups.Metadata;

namespace Arch.SystemGroups;

/// <summary>
///     The base system for all systems that are executed in the player loop
/// </summary>
/// <typeparam name="TWorld"></typeparam>
public abstract class PlayerLoopSystem<TWorld> : BaseSystem<TWorld, float>
{
    /// <summary>
    ///     Default constructor for a player loop system
    /// </summary>
    /// <param name="world"></param>
    protected PlayerLoopSystem(TWorld world) : base(world)
    {
    }

    /// <summary>
    ///     The metadata of the system in an abstract form
    /// </summary>
    /// <returns></returns>
    protected abstract AttributesInfoBase GetMetadataInternal();
}