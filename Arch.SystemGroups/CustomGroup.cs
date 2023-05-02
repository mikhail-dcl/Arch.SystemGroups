using System.Collections.Generic;
using Arch.System;
using UnityEngine.Pool;

namespace Arch.SystemGroups;

/// <summary>
/// Similar to `Arch.System.Group` but with better API that allows pooling
/// </summary>
/// <typeparam name="T"></typeparam>
public class CustomGroup<T> : ISystem<T>
{
    private List<ISystem<T>> _systems;

    protected CustomGroup()
    {
        
    }

    public CustomGroup(IEnumerable<ISystem<T>> systems)
    {
        _systems = ListPool<ISystem<T>>.Get();
        AddRange(systems);
    }

    public CustomGroup(List<ISystem<T>> systems)
    {
        _systems = systems;
    }

    internal List<ISystem<T>> Systems => _systems;

    public void AddRange(IEnumerable<ISystem<T>> systems)
    {
        _systems.AddRange(systems);
    }

    internal void SetSystems(List<ISystem<T>> systems)
    {
        _systems = systems;
    }

    public void Initialize()
    {
        foreach (var system in _systems)
            system.Initialize();
    }

    public void Dispose()
    {
        ListPool<ISystem<T>>.Release(_systems);
    }

    public void BeforeUpdate(in T t)
    {
        for (int index = 0; index < _systems.Count; ++index)
            _systems[index].BeforeUpdate(in t);
    }

    public void Update(in T t)
    {
        for (int index = 0; index < _systems.Count; ++index)
            _systems[index].Update(in t);
    }

    public void AfterUpdate(in T t)
    {
        for (int index = 0; index < _systems.Count; ++index)
            _systems[index].AfterUpdate(in t);
    }
}