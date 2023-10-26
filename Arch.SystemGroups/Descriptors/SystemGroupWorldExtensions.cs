using System.Collections.Generic;

namespace Arch.SystemGroups.Descriptors;

/// <summary>
/// Extensions for SystemGroupWorld
/// </summary>
public static partial class SystemGroupWorldExtensions
{
    /// <summary>
    /// Generate the descriptor for the SystemGroupWorld
    /// </summary>
    /// <param name="world"></param>
    public static IReadOnlyList<SystemGroupDescriptor> GenerateDescriptor(this SystemGroupWorld world)
    {
        var descriptors = new List<SystemGroupDescriptor>();
        foreach (var worldSystemGroup in world.SystemGroups)
        {
            var systems = new List<SystemDescriptor>();
            if (worldSystemGroup.Nodes != null)
            {
                foreach (var node in worldSystemGroup.Nodes)
                {
                    systems.Add(new SystemDescriptor(node.System.GetType().Name));
                }   
            }
            
            descriptors.Add(new SystemGroupDescriptor(worldSystemGroup.GetType().Name, systems));
        }

        return descriptors;
    }
}