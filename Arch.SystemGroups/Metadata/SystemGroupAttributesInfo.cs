using System;
using System.Collections.Generic;

namespace Arch.SystemGroups.Metadata
{
    /// <summary>
    /// Dummy attributes info for system groups
    /// </summary>
    public class SystemGroupAttributesInfo : AttributesInfoBase
    {
        /// <summary>
        /// Instance shared between all System Groups as they provide no attributes data
        /// </summary>
        public static readonly SystemGroupAttributesInfo Instance = new();
    
        /// <summary>
        /// Returns null
        /// </summary>
        public override Type UpdateInGroup => null;

        /// <summary>
        /// Returns null
        /// </summary>
        public override AttributesInfoBase GroupMetadata => null;

        /// <summary>
        /// Returns null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override T GetAttribute<T>() => null;

        /// <summary>
        /// Returns an empty array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override IReadOnlyList<T> GetAttributes<T>() => Array.Empty<T>();
    }
}