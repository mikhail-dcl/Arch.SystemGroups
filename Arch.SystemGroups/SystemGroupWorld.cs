using System;
using System.Collections.Generic;

namespace Arch.SystemGroups;

public class SystemGroupWorld : IDisposable
{
    private readonly IUnityPlayerLoopHelper _unityPlayerLoopHelper;
    
    internal IReadOnlyList<SystemGroup> SystemGroups { get; }
    
    internal SystemGroupWorld(IReadOnlyList<SystemGroup> systemGroups, IUnityPlayerLoopHelper unityPlayerLoopHelper)
    {
        _unityPlayerLoopHelper = unityPlayerLoopHelper;
        SystemGroups = systemGroups;
    }

    public void Initialize()
    {
        for (var i = 0; i < SystemGroups.Count; i++)
        {
            SystemGroups[i].Initialize();
        }
    }

    public void Dispose()
    {
        for (var i = 0; i < SystemGroups.Count; i++)
        {
            SystemGroups[i].Dispose();
            _unityPlayerLoopHelper.RemoveFromCurrentPlayerLoop(SystemGroups[i]);
        }
    }
}