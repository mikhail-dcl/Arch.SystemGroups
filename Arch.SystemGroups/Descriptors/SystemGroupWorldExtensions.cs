using System.Collections.Generic;
using JetBrains.Annotations;

namespace Arch.SystemGroups.Descriptors
{
    /// <summary>
    /// Extensions for SystemGroupWorld
    /// </summary>
    public static class SystemGroupWorldExtensions
    {
        /// <summary>
        /// Generate the descriptor for the SystemGroupWorld
        /// </summary>
        /// <param name="world"></param>
        [UsedImplicitly]
        public static IReadOnlyList<Descriptor> GenerateDescriptors(this SystemGroupWorld world)
        {
            var descriptors = new List<Descriptor>();
            // Foreach system group such as initialization, simulation, presentation
            foreach (var worldSystemGroup in world.SystemGroups)
            {
                // Throttling is not possible on a root level
                descriptors.Add(GenerateGroupDescriptor(worldSystemGroup.GetType().Name, false, worldSystemGroup.Nodes));
            }

            return descriptors;
        }

        /// <summary>
        /// Generate the descriptor for the SystemGroup
        /// </summary>
        /// <param name="name">Name of the system group</param>
        /// <param name="throttlingEnabled">Determines if this group has throttling enabled</param>
        /// <param name="nodes">List of nodes within the system group</param>
        private static Descriptor GenerateGroupDescriptor(string name, bool throttlingEnabled, IReadOnlyList<ExecutionNode<float>> nodes)
        {
            var descriptors = new List<Descriptor>();
            GenerateNestedGroupDescriptors(nodes, descriptors);
            return new Descriptor(name,throttlingEnabled,descriptors);
        }

        /// <summary>
        /// Generate the descriptor for the SystemGroupWorld
        /// </summary>
        /// <param name="nodes">A list of execution nodes</param>
        /// <param name="descriptors">A list of descriptors</param>
        /// <returns></returns>
        private static void GenerateNestedGroupDescriptors(IReadOnlyList<ExecutionNode<float>> nodes,
            List<Descriptor> descriptors)
        {
            if(nodes is null) return;
            foreach (var node in nodes)
            {
                if (node.IsGroup)
                {
                    descriptors.Add(GenerateGroupDescriptor(node.CustomGroup.GetType().Name, node.ThrottlingEnabled, node.CustomGroup.Nodes));
                }
                else
                {
                    descriptors.Add(new Descriptor(node.System.GetType().Name, node.ThrottlingEnabled));
                }
            }
        }
    }
}