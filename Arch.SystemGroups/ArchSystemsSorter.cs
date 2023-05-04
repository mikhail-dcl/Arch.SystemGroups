using System;
using System.Collections.Generic;
using Arch.System;
using UnityEngine.Pool;

namespace Arch.SystemGroups;

/// <summary>
/// Sorts systems. Used by auto-generated code. Consider ignoring it
/// </summary>
public static class ArchSystemsSorter
{
    /// <summary>
    /// Adds a dependency edge to the graph.
    /// Supports redundancies.
    /// Circular dependencies will be resolved on the final stage of the graph traversal
    /// </summary>
    /// <param name="from">System that should be updated before <paramref name="to"/> </param>
    /// <param name="to">System that should be updated after</param>
    /// <param name="edges">Storage of edges</param>
    public static void AddEdge(Type from, Type to, Dictionary<Type, List<Type>> edges)
    {
        if (!edges.TryGetValue(from, out var list))
            edges[from] = list = ListPool<Type>.Get();
        list.Add(to);
    }
        
    internal static List<ISystem<float>> SortSystems(Dictionary<Type, ISystem<float>> systems, Dictionary<Type, List<Type>> edges)
    {
        var result = ListPool<ISystem<float>>.Get();

        var visited = HashSetPool<Type>.Get();
        
        foreach (var system in systems.Keys)
        {
            // Circular dependencies on the root will be found in the second cycle of the DFS Traversal
            DFSTraversal(systems, system, visited, result, edges);
        }
            
        HashSetPool<Type>.Release(visited);
        
        result.Reverse();

        return result;
    }

    private static Type DFSTraversal(Dictionary<Type, ISystem<float>> systems, Type currentVertex, 
        HashSet<Type> visited, List<ISystem<float>> result, Dictionary<Type, List<Type>> edges)
    {
        if (visited.Add(currentVertex))
        {
            if (edges.TryGetValue(currentVertex, out var currentVertexEdges))
            {
                foreach (var vertex in currentVertexEdges)
                {
                    // the current vertex may not belong to the current hierarchy
                    if (!systems.ContainsKey(vertex))
                        continue;
                    
                    var circularDependency = DFSTraversal(systems, vertex, visited, result, edges);
                    if (circularDependency == currentVertex)
                        throw new InvalidOperationException($"Circular dependency detected on a path: {currentVertex} -> {vertex} -> ...");
                }
            }

            // Instead of adding to the head (that is much more expensive) we add to the tail and reverse the list
            result.Add(systems[currentVertex]);
        }
        
        // return the type that is a potential circular dependency
        return currentVertex;
    }
}