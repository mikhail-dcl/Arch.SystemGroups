using System;
using System.Collections.Generic;

namespace Arch.SystemGroups.Metadata
{
    /// <summary>
    ///     Generated attributes info, allows to avoid reflection if such access is needed
    /// </summary>
    public abstract class AttributesInfoBase
    {
        /// <summary>
        ///     <see cref="UpdateInGroupAttribute" /> reflection, will be null for system groups
        /// </summary>
        public abstract Type UpdateInGroup { get; }

        /// <summary>
        ///     Metadata of the group this system belongs to, will be null for system groups
        /// </summary>
        public abstract AttributesInfoBase GroupMetadata { get; }

        /// <summary>
        ///     Get first attribute of type <typeparamref name="T" />
        /// </summary>
        /// <typeparam name="T">Type of attribute</typeparam>
        /// <returns>Null if such attribute is not defined for the class</returns>
        public abstract T GetAttribute<T>() where T : Attribute;

        /// <summary>
        ///     Get all attributes of type <typeparamref name="T" />
        /// </summary>
        /// <typeparam name="T">Type of attribute</typeparam>
        /// <returns>An empty list if no attributes are found</returns>
        public abstract IReadOnlyList<T> GetAttributes<T>() where T : Attribute;
    }
}