using System.Collections.Generic;
using Arch.System;
using UnityEngine.Pool;

namespace Arch.SystemGroups;

/// <summary>
/// The base class that can be used to provide custom behaviour for a group
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class CustomGroupBase<T> : ISystem<T>
{
    private List<ISystem<T>> _systems;
    
    /// <summary>
    /// Creates an empty group
    /// </summary>
    protected CustomGroupBase()
    {
    }
    
    internal List<ISystem<T>> Systems => _systems;

    /// <summary>
    /// Creates a group from the collection from which a pooled instance of the list will be created
    /// </summary>
    /// <param name="systems"></param>
    protected CustomGroupBase(IEnumerable<ISystem<T>> systems)
    {
        _systems = ListPool<ISystem<T>>.Get();
        AddRange(systems);
    }

    /// <summary>
    /// Creates a group from the list that won't be copied
    /// </summary>
    /// <param name="systems"></param>
    protected CustomGroupBase(List<ISystem<T>> systems)
    {
        _systems = systems;
    }
    
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
    protected void InitializeInternal()
    {
        foreach (var system in _systems)
            system.Initialize();
    }

    /// <summary>
    /// Dispose all systems in the group
    /// </summary>
    protected void DisposeInternal()
    {
        foreach (var system in _systems)
            system.Dispose();
        
        ListPool<ISystem<T>>.Release(_systems);
    }

    /// <summary>
    /// Update all systems
    /// </summary>
    /// <param name="t">Delta time</param>
    protected void BeforeUpdateInternal(in T t)
    {
        for (int index = 0; index < _systems.Count; ++index)
            _systems[index].BeforeUpdate(in t);
    }

    /// <summary>
    /// Update all systems
    /// </summary>
    /// <param name="t">Delta time</param>
    protected void UpdateInternal(in T t)
    {
        for (int index = 0; index < _systems.Count; ++index)
            _systems[index].Update(in t);
    }
    
    /// <summary>
    /// Update all systems
    /// </summary>
    /// <param name="t">Delta time</param>
    protected void AfterUpdateInternal(in T t)
    {
        for (int index = 0; index < _systems.Count; ++index)
            _systems[index].AfterUpdate(in t);
    }
    
    /// <summary>
    /// Override to provide Dispose behaviour, you can use <see cref="DisposeInternal"/> as the default implementation
    /// </summary>
    public abstract void Dispose();
    
    /// <summary>
    /// Override to provide initialization behaviour, you can use <see cref="InitializeInternal"/> as the default implementation
    /// </summary>
    public abstract void Initialize();
    
    /// <summary>
    /// Override to provide BeforeUpdate, you can use <see cref="BeforeUpdateInternal"/> as the default implementation
    /// </summary>
    /// <param name="t"></param>
    public abstract void BeforeUpdate(in T t);
    
    /// <summary>
    /// Override to provide Update behaviour, you can use <see cref="UpdateInternal"/> as the default implementation
    /// </summary>
    /// <param name="t"></param>
    public abstract void Update(in T t);
    
    /// <summary>
    /// Override to provide AfterUpdate behaviour, you can use <see cref="AfterUpdateInternal"/> as the default implementation
    /// </summary>
    /// <param name="t"></param>
    public abstract void AfterUpdate(in T t);
}