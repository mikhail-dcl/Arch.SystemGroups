using System;
using System.Buffers;
using System.Collections.Generic;
using Arch.System;
using Arch.SystemGroups.Throttling;
using Arch.SystemGroups.UnityBridge;
using JetBrains.Annotations;
using UnityEngine.Pool;

namespace Arch.SystemGroups;

/// <summary>
/// Denotes a root group connected to a specific phase of the player loop.
/// By default updated by the scaled deltaTime.
/// If Unscaled delta time is needed consider using Time.unscaledXXX manually.
/// </summary>
public abstract class SystemGroup : IDisposable
{
    private readonly List<ExecutionNode<float>> _nodes;
    private readonly ISystemGroupThrottler _throttler;

    private readonly Type _type;

    internal SystemGroup(List<ExecutionNode<float>> nodes, [CanBeNull] ISystemGroupThrottler throttler)
    {
        _nodes = nodes;
        _throttler = throttler;
        _type = GetType();
    }
    
    internal List<ExecutionNode<float>> Nodes => _nodes;

    internal abstract void Update();

    private protected void Update(in TimeProvider.Info timeInfo)
    {
        if (_nodes == null) return;
        
        if (_nodes.Count == 0) return;

        var throttle = _throttler != null && _throttler.ShouldThrottle(_type, in timeInfo);
        
        foreach (var system in _nodes)
        {
            system.BeforeUpdate(in timeInfo.DeltaTime, throttle);
        }
        
        foreach (var system in _nodes)
        {
            system.Update(in timeInfo.DeltaTime, throttle);
        }
        
        foreach (var system in _nodes)
        {
            system.AfterUpdate(in timeInfo.DeltaTime, throttle);
        }
        
        _throttler?.OnSystemGroupUpdateFinished(_type, throttle);
    }

    internal void Initialize()
    {
        if (_nodes == null) return;
        
        foreach (var system in _nodes)
            system.Initialize();
    }

    /// <summary>
    /// Dispose all systems and release the list allocated for them.
    /// After the dispose is called the instance of the group is no longer usable.
    /// </summary>
    public void Dispose()
    {
        if (_nodes == null) return;
        
        foreach (var system in _nodes)
            system.Dispose();
        ListPool<ExecutionNode<float>>.Release(_nodes);
    }
}