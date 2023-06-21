using Arch.System;

namespace Arch.SystemGroups.Tests;

public static class AssertHelpers
{
    /// <typeparam name="T">System or Group Type</typeparam>
    internal static int FindIndexOfNode<T>(this List<ExecutionNode<float>> list)
    {
        return list.FindIndex(n => n.IsGroup ? n.CustomGroup.GetType() == typeof(T) : n.System.GetType() == typeof(T));
    }
    
    public static void AssertOrderOfSystemIsLessThenOtherSystem<TGroup, T1, T2>(SystemGroupWorld world)
        where TGroup : SystemGroup
    {
        var group = world.SystemGroups.OfType<TGroup>().First();
        var index1 = group.Nodes.FindIndexOfNode<T1>();
        var index2 = group.Nodes.FindIndexOfNode<T2>();
        
        if (index1 == -1) 
            throw new InvalidOperationException($"System {typeof(T1)} not found in group {typeof(TGroup)}");
        if (index2 == -1)
            throw new InvalidOperationException($"System {typeof(T2)} not found in group {typeof(TGroup)}");
        
        Assert.That(index1, Is.LessThan(index2));
    }
    
    internal static void AssertOrderOfSystemIsLessThenOtherSystem<T1, T2>(List<ExecutionNode<float>> nodes)
    {
        var index1 = nodes.FindIndexOfNode<T1>();
        var index2 = nodes.FindIndexOfNode<T2>();
        
        if (index1 == -1)
            throw new InvalidOperationException($"System {typeof(T1)} not found)");
        
        if (index2 == -1)
            throw new InvalidOperationException($"System {typeof(T2)} not found)");
        
        Assert.That(index1, Is.LessThan(index2));
    }
    
    internal static void AssertNodesEquivalency(List<ExecutionNode<float>> nodes, params Type[] expectedTypes)
    {
        CollectionAssert.AreEquivalent(expectedTypes, nodes.Select(n => n.IsGroup ? n.CustomGroup.GetType() : n.System.GetType()));
    }
    
    internal static T Find<T>(this List<ExecutionNode<float>> nodes)
    {
        foreach (var executionNode in nodes)
        {
            if (executionNode is { IsGroup: true, CustomGroup: T group })
                return group;
            
            if (executionNode is { IsGroup: false, System: T system })
                return system;
        }
        
        throw new InvalidOperationException($"System or Group {typeof(T)} not found");
    }
}