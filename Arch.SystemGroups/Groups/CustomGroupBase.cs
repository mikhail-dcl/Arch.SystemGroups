using System.Collections.Generic;
using System.Linq;
using Arch.System;
using Arch.SystemGroups.Metadata;
using UnityEngine.Pool;

namespace Arch.SystemGroups;

/// <summary>
///     The base class that can be used to provide custom behaviour for a group
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class CustomGroupBase<T>
{
    /// <summary>
    ///     Creates an empty group
    /// </summary>
    protected CustomGroupBase()
    {
    }

    /// <summary>
    ///     Creates a group from the collection from which a pooled instance of the list will be created
    /// </summary>
    protected CustomGroupBase(IEnumerable<ISystem<T>> systems, bool throttlingEnabled)
    {
        Nodes = ListPool<ExecutionNode<T>>.Get();
        AddRange(systems.Select(s => new ExecutionNode<T>(s, throttlingEnabled)));
    }

    internal List<ExecutionNode<T>> Nodes { get; private set; }

    /// <summary>
    ///     Override to provide Dispose behaviour, you can use <see cref="DisposeInternal" /> as the default implementation
    /// </summary>
    public abstract void Dispose();

    /// <summary>
    ///     Override to provide initialization behaviour, you can use <see cref="InitializeInternal" /> as the default
    ///     implementation
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    ///     Override to provide BeforeUpdate, you can use <see cref="BeforeUpdateInternal" /> as the default implementation
    /// </summary>
    /// <param name="t"></param>
    /// <param name="throttle">Indicates that the current invocation is throttled</param>
    public abstract void BeforeUpdate(in T t, bool throttle);

    /// <summary>
    ///     Override to provide Update behaviour, you can use <see cref="UpdateInternal" /> as the default implementation
    /// </summary>
    /// <param name="t"></param>
    /// <param name="throttle">Indicates that the current invocation is throttled</param>
    public abstract void Update(in T t, bool throttle);

    /// <summary>
    ///     Override to provide AfterUpdate behaviour, you can use <see cref="AfterUpdateInternal" /> as the default
    ///     implementation
    /// </summary>
    /// <param name="throttle">Indicates that the current invocation is throttled</param>
    /// <param name="t"></param>
    public abstract void AfterUpdate(in T t, bool throttle);

    /// <summary>
    ///     Adds systems to the group
    /// </summary>
    /// <param name="systems"></param>
    internal void AddRange(IEnumerable<ExecutionNode<T>> systems)
    {
        Nodes.AddRange(systems);
    }

    internal void SetSystems(List<ExecutionNode<T>> systems)
    {
        Nodes = systems;
    }

    /// <summary>
    ///     Initialize all systems in the group
    /// </summary>
    protected void InitializeInternal()
    {
        foreach (var node in Nodes)
            node.Initialize();
    }

    /// <summary>
    ///     Dispose all systems in the group
    /// </summary>
    protected void DisposeInternal()
    {
        foreach (var node in Nodes)
            node.Dispose();

        ListPool<ExecutionNode<T>>.Release(Nodes);
    }

    /// <summary>
    ///     Update all systems
    /// </summary>
    /// <param name="t">Delta time</param>
    /// <param name="throttle">Current update is throttled</param>
    protected void BeforeUpdateInternal(in T t, bool throttle)
    {
        for (var index = 0; index < Nodes.Count; ++index)
            Nodes[index].BeforeUpdate(in t, throttle);
    }

    /// <summary>
    ///     Update all systems
    /// </summary>
    /// <param name="t">Delta time</param>
    /// <param name="throttle">Current update is throttled</param>
    protected void UpdateInternal(in T t, bool throttle)
    {
        for (var index = 0; index < Nodes.Count; ++index)
            Nodes[index].Update(in t, throttle);
    }

    /// <summary>
    ///     Update all systems
    /// </summary>
    /// <param name="t">Delta time</param>
    /// <param name="throttle">Current update is throttled</param>
    protected void AfterUpdateInternal(in T t, bool throttle)
    {
        for (var index = 0; index < Nodes.Count; ++index)
            Nodes[index].AfterUpdate(in t, throttle);
    }
    
    /// <summary>
    ///    The metadata of the group in an abstract form
    /// </summary>
    /// <returns></returns>
    protected abstract AttributesInfoBase GetMetadataInternal();
}