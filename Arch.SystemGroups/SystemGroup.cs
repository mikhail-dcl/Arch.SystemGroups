using System;
using System.Buffers;
using System.Collections.Generic;
using Arch.System;
using UnityEngine.Pool;

namespace Arch.SystemGroups;

/// <summary>
/// Denotes a root group connected to a specific phase of the player loop.
/// By default updated by the scaled deltaTime.
/// If Unscaled delta time is needed consider using Time.unscaledXXX manually.
/// </summary>
public abstract class SystemGroup : IDisposable
{
    private readonly List<ISystem<float>> _systems;
    internal SystemGroup(List<ISystem<float>> systems)
    {
        _systems = systems;
    }
    
    internal List<ISystem<float>> Systems => _systems;

    internal abstract void Update();

    protected void Update(float deltaTime)
    {
        if (_systems.Count == 0) return;
        
        foreach (var system in _systems)
        {
            system.BeforeUpdate(deltaTime);
        }
        
        foreach (var system in _systems)
        {
            system.Update(deltaTime);
        }
        
        foreach (var system in _systems)
        {
            system.AfterUpdate(deltaTime);
        }
    }

    public void Initialize()
    {
        foreach (var system in _systems)
            system.Initialize();
    }

    public void Dispose()
    {
        ListPool<ISystem<float>>.Release(_systems);
    }
}