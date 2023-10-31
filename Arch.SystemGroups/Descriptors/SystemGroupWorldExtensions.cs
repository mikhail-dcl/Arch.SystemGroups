using System.Collections.Generic;
using JetBrains.Annotations;

namespace Arch.SystemGroups.Descriptors;

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
    public static IReadOnlyList<SystemGroupDescriptor> GenerateDescriptors(this SystemGroupWorld world)
    {
        var descriptors = new List<SystemGroupDescriptor>();
        // Foreach system group such as initialization, simulation, presentation
        foreach (var worldSystemGroup in world.SystemGroups)
        {
            descriptors.Add(GenerateGroupDescriptor(worldSystemGroup.GetType().Name, worldSystemGroup.Nodes));
        }

        return descriptors;
    }

    /// <summary>
    /// Generate the descriptor for the SystemGroup
    /// </summary>
    /// <param name="name">Name of the system group</param>
    /// <param name="nodes">List of nodes within the system group</param>
    private static SystemGroupDescriptor GenerateGroupDescriptor(string name, IReadOnlyList<ExecutionNode<float>> nodes)
    {
        var groupSystems = new List<SystemDescriptor>();
        var groupNestedGroups = new List<SystemGroupDescriptor>();
        GenerateNestedGroupDescriptors(nodes, ref groupSystems, ref groupNestedGroups);
        return new SystemGroupDescriptor(name, groupSystems, groupNestedGroups
        );
    }
    
    /// <summary>
    /// Generate the descriptor for the SystemGroupWorld
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="groupSystems"></param>
    /// <param name="groupNestedGroups"></param>
    /// <returns></returns>
    private static void GenerateNestedGroupDescriptors(IReadOnlyList<ExecutionNode<float>> nodes,
        ref List<SystemDescriptor> groupSystems, ref List<SystemGroupDescriptor> groupNestedGroups)
    {
        if(nodes is null) return;
        foreach (var node in nodes)
        {
            if (node.IsGroup)
            {
                groupNestedGroups.Add(GenerateGroupDescriptor(node.CustomGroup.GetType().Name, node.CustomGroup.Nodes));
            }
            else
            {
                groupSystems.Add(new SystemDescriptor(node.System.GetType().Name, node.ThrottlingEnabled));
            }
        }
    }
}