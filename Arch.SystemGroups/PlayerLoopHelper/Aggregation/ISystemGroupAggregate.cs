using System;

namespace Arch.SystemGroups
{
    /// <summary>
    ///     Non-generic interface for <see cref="ISystemGroupAggregate{T}" />
    /// </summary>
    public interface ISystemGroupAggregate
    {
        /// <summary>
        ///     Count of system groups
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     This function is called from Unity Player Loop
        /// </summary>
        void TriggerUpdate();
    
        /// <summary>
        ///     Remove a system group from the aggregate
        /// </summary>
        void Remove(SystemGroup systemGroup);
    }

    /// <summary>
    ///     Defines a way of aggregating system groups of the same type
    ///     <typeparam name="T">Additional Data set per world basis</typeparam>
    /// </summary>
    public interface ISystemGroupAggregate<T> : ISystemGroupAggregate
    {
        /// <summary>
        ///     Add a system group to the aggregate
        /// </summary>
        void Add(in T data, SystemGroup systemGroup);

        /// <summary>
        ///     Factory for SystemGroupAggregate
        /// </summary>
        interface IFactory : ISystemGroupAggregateFactory
        {
            ISystemGroupAggregate ISystemGroupAggregateFactory.Create(Type systemGroupType)
            {
                return Create(systemGroupType);
            }

            /// <summary>
            ///     Creates a new instance of SystemGroupAggregate.
            ///     Called once per type of SystemGroup
            /// </summary>
            /// <returns></returns>
            new ISystemGroupAggregate<T> Create(Type systemGroupType);
        }
    }
}