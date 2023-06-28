using System;
using System.Collections.Generic;
using Arch.SystemGroups.Metadata;
using Arch.SystemGroups.Throttling;
using Arch.SystemGroups.UnityBridge;
using JetBrains.Annotations;
using UnityEngine.Pool;

namespace Arch.SystemGroups;

/// <summary>
///     Denotes a root group connected to a specific phase of the player loop.
///     By default updated by the scaled deltaTime.
///     If Unscaled delta time is needed consider using Time.unscaledXXX manually.
/// </summary>
public abstract class SystemGroup : IDisposable
{
    public enum State : byte
    {
        /// <summary>
        ///     Initialized was not called yet
        /// </summary>
        NotInitialized,

        /// <summary>
        ///     Up and Running
        /// </summary>
        Active,

        /// <summary>
        ///     The execution of update functions is suspended but the group is not disposed
        /// </summary>
        Suspended,

        /// <summary>
        ///     Disposed was executed
        /// </summary>
        Disposed
    }

    private readonly ISystemGroupExceptionHandler _exceptionHandler;

    private readonly ISystemGroupThrottler _throttler;

    private readonly Type _type;

    internal SystemGroup(List<ExecutionNode<float>> nodes, [CanBeNull] ISystemGroupThrottler throttler,
        [CanBeNull] ISystemGroupExceptionHandler exceptionHandler)
    {
        Nodes = nodes;
        _throttler = throttler;
        _exceptionHandler = exceptionHandler;
        _type = GetType();
    }
    
    public static SystemGroupAttributesInfo Metadata = SystemGroupAttributesInfo.Instance;

    internal State CurrentState { get; private set; } = State.NotInitialized;

    internal List<ExecutionNode<float>> Nodes { get; }

    internal abstract void Update();

    private protected void Update(in TimeProvider.Info timeInfo)
    {
        if (Nodes == null) return;

        if (Nodes.Count == 0) return;

        if (CurrentState != State.Active) return;

        var throttle = _throttler != null && _throttler.ShouldThrottle(_type, in timeInfo);

        try
        {
            foreach (var system in Nodes)
            {
                system.BeforeUpdate(in timeInfo.DeltaTime, throttle);
            }

            foreach (var system in Nodes)
            {
                system.Update(in timeInfo.DeltaTime, throttle);
            }

            foreach (var system in Nodes)
            {
                system.AfterUpdate(in timeInfo.DeltaTime, throttle);
            }
        }
        catch (Exception e)
        {
            if (_exceptionHandler == null)
                throw;
            
            switch (_exceptionHandler.Handle(e, _type))
            {
                case ISystemGroupExceptionHandler.Action.Suspend:
                    CurrentState = State.Suspended;
                    break;
                case ISystemGroupExceptionHandler.Action.Dispose:
                    Dispose();
                    break;
            }
        }

        _throttler?.OnSystemGroupUpdateFinished(_type, throttle);
    }

    internal void Initialize()
    {
        if (Nodes == null) return;

        CurrentState = State.Active;

        foreach (var system in Nodes)
            system.Initialize();
    }
    
    /// <summary>
    ///     Dispose all systems and release the list allocated for them.
    ///     After the dispose is called the instance of the group is no longer usable.
    /// </summary>
    public void Dispose()
    {
        if (Nodes == null) return;
        if (CurrentState == State.Disposed) return;

        foreach (var system in Nodes)
            system.Dispose();
        ListPool<ExecutionNode<float>>.Release(Nodes);
        CurrentState = State.Disposed;
    }
}