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
    
    /// <summary>
    /// Creates an empty group, for auto-generated code only,
    /// Don't invoke it manually
    /// </summary>
    protected CustomGroup()
    {
    }

    /// <summary>
    /// Creates a group from the collection from which a pooled instance of the list will be created
    /// </summary>
    /// <param name="systems"></param>
    public CustomGroup(IEnumerable<ISystem<T>> systems)
    {
        _systems = ListPool<ISystem<T>>.Get();
        AddRange(systems);
    }

    /// <summary>
    /// Creates a group from the list that won't be copied
    /// </summary>
    /// <param name="systems"></param>
    public CustomGroup(List<ISystem<T>> systems)
    {
        _systems = systems;
    }

    internal List<ISystem<T>> Systems => _systems;

    /// <summary>
    /// Adds systems to the group
    /// </summary>
    /// <param name="systems"></param>
    public void AddRange(IEnumerable<ISystem<T>> systems)
    {
        _systems.AddRange(systems);
    }

    internal void SetSystems(List<ISystem<T>> systems)
    {
        _systems = systems;
    }

    /// <summary>
    /// Initialize all systems in the group
    /// </summary>
    public void Initialize()
    {
        foreach (var system in _systems)
            system.Initialize();
    }

    /// <summary>
    /// Dispose all systems in the group
    /// </summary>
    public void Dispose()
    {
        ListPool<ISystem<T>>.Release(_systems);
    }

    /// <summary>
    /// To comply with Arch.System.ISystem
    /// </summary>
    /// <param name="t">Delta time</param>
    public void BeforeUpdate(in T t)
    {
        for (int index = 0; index < _systems.Count; ++index)
            _systems[index].BeforeUpdate(in t);
    }

    /// <summary>
    /// To comply with Arch.System.ISystem
    /// </summary>
    /// <param name="t">Delta time</param>
    public void Update(in T t)
    {
        for (int index = 0; index < _systems.Count; ++index)
            _systems[index].Update(in t);
    }
    
    /// <summary>
    /// To comply with Arch.System.ISystem
    /// </summary>
    /// <param name="t">Delta time</param>
    public void AfterUpdate(in T t)
    {
        for (int index = 0; index < _systems.Count; ++index)
            _systems[index].AfterUpdate(in t);
    }
}