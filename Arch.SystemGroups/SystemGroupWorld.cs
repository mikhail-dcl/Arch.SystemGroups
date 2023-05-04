using System;
using System.Collections.Generic;

namespace Arch.SystemGroups;

/// <summary>
/// An entry point to the systems connected to the Unity Player Loop.
/// </summary>
public class SystemGroupWorld : IDisposable
{
    private readonly IUnityPlayerLoopHelper _unityPlayerLoopHelper;
    
    internal IReadOnlyList<SystemGroup> SystemGroups { get; }
    
    internal SystemGroupWorld(IReadOnlyList<SystemGroup> systemGroups, IUnityPlayerLoopHelper unityPlayerLoopHelper)
    {
        _unityPlayerLoopHelper = unityPlayerLoopHelper;
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
            _unityPlayerLoopHelper.RemoveFromCurrentPlayerLoop(SystemGroups[i]);
        }
    }
}