using System.Collections.Generic;

namespace Arch.SystemGroups.Descriptors;

    /// <summary>
    /// Descriptor for a SystemGroup
    /// </summary>
    public struct SystemGroupDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">name of the system group</param>
        /// <param name="systems">a list of systems within this group</param>
        /// <param name="groups">A list of subGroups within this group</param>
        public SystemGroupDescriptor(string name, IReadOnlyList<SystemDescriptor> systems, IReadOnlyList<SystemGroupDescriptor> groups)
        {
            Name = name;
            Systems = systems;
            Groups = groups;
        }

        /// <summary>
        /// Name of this SystemGroup
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// Systems within this group 
        /// </summary>
        public IReadOnlyList<SystemDescriptor> Systems { get; }
        
        /// <summary>
        /// Sub Groups within this group
        /// </summary>
        public IReadOnlyList<SystemGroupDescriptor> Groups { get; }
    }
