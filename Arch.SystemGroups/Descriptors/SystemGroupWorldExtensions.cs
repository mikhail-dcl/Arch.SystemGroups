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
            var groupSystems = new List<SystemDescriptor>();
            IReadOnlyList<SystemGroupDescriptor> subGroups = null;

            if (worldSystemGroup.Nodes is not null)
            {
                foreach (var node in worldSystemGroup.Nodes)
                {
                    if (node.IsGroup)
                    {
                        subGroups = GenerateGroupDescriptor(node.CustomGroup);
                    }
                    else
                    {
                        groupSystems.Add(new SystemDescriptor(node.System.GetType().Name, node.ThrottlingEnabled));
                    }
                }   
            }
            
            descriptors.Add(new SystemGroupDescriptor(worldSystemGroup.GetType().Name, groupSystems, subGroups));
        }

        return descriptors;
    }
    
    /// <summary>
    /// Generate the descriptor for the SystemGroupWorld
    /// </summary>
    /// <param name="world"></param>
    private static IReadOnlyList<SystemGroupDescriptor> GenerateGroupDescriptor(CustomGroupBase<float> customGroup)
    {
        var descriptors = new List<SystemGroupDescriptor>();
        var groupSystems = new List<SystemDescriptor>();
        IReadOnlyList<SystemGroupDescriptor> subGroups = null;

        if (customGroup.Nodes is not null)
        {
            foreach (var node in customGroup.Nodes)
            {
                if (node.IsGroup)
                {
                    subGroups = GenerateGroupDescriptor(node.CustomGroup);
                }
                else
                {
                    groupSystems.Add(new SystemDescriptor(node.System.GetType().Name, node.ThrottlingEnabled));
                }
            }   
        }
            
        descriptors.Add(new SystemGroupDescriptor(customGroup.GetType().Name, groupSystems, subGroups));
        return descriptors;
    }
}