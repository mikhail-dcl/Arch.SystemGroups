using Arch.System;

namespace Arch.SystemGroups.Tests;

public class AssertHelpers
{
    public static void AssertOrderOfSystemIsLessThenOtherSystem<TGroup, T1, T2>(SystemGroupWorld world)
        where T1 : ISystem<float>
        where T2 : ISystem<float>
        where TGroup : SystemGroup
    {
        var group = world.SystemGroups.OfType<TGroup>().First();
        var index1 = group.Systems.FindIndex(system => system.GetType() == typeof(T1));
        var index2 = group.Systems.FindIndex(system => system.GetType() == typeof(T2));
        
        if (index1 == -1) 
            throw new InvalidOperationException($"System {typeof(T1)} not found in group {typeof(TGroup)}");
        if (index2 == -1)
            throw new InvalidOperationException($"System {typeof(T2)} not found in group {typeof(TGroup)}");
        
        Assert.That(index1, Is.LessThan(index2));
    }
    
    public static void AssertOrderOfSystemIsLessThenOtherSystem<T1, T2>(List<ISystem<float>> systems)
        where T1 : ISystem<float>
        where T2 : ISystem<float>
    {
        var index1 = systems.FindIndex(system => system.GetType() == typeof(T1));
        var index2 = systems.FindIndex(system => system.GetType() == typeof(T2));
        
        if (index1 == -1)
            throw new InvalidOperationException($"System {typeof(T1)} not found)");
        
        if (index2 == -1)
            throw new InvalidOperationException($"System {typeof(T2)} not found)");
        
        Assert.That(index1, Is.LessThan(index2));
    }
}