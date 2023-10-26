using System.Collections.Generic;

namespace Arch.SystemGroups.Descriptors;

    /// <summary>
    /// Descriptor for a SystemGroup
    /// </summary>
    public class SystemGroupDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">name of the system group</param>
        /// <param name="systems">a list of systems within this group</param>
        public SystemGroupDescriptor(string name, IReadOnlyList<SystemDescriptor> systems)
        {
            Name = name;
            Systems = systems;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<SystemDescriptor> Systems { get; }
    }
