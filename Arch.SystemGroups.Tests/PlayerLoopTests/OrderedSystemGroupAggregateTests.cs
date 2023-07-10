using Arch.SystemGroups.DefaultSystemGroups;

namespace Arch.SystemGroups.Tests.PlayerLoopTests;

public class OrderedSystemGroupAggregateTests
{
    [Test]
    public void DebouncesEqualValues()
    {
        var aggregate = new OrderedSystemGroupAggregate<int>(Comparer<int>.Default, true);
        
        aggregate.Add(100, new PresentationSystemGroup(new List<ExecutionNode<float>>(), null, null));
        Assert.DoesNotThrow(() => aggregate.Add(100, new PresentationSystemGroup(new List<ExecutionNode<float>>(), null, null)));
    }

    [Test]
    public void ThrowsIfNotDebounced()
    {
        var aggregate = new OrderedSystemGroupAggregate<int>(Comparer<int>.Default, false);
        
        aggregate.Add(100, new PresentationSystemGroup(new List<ExecutionNode<float>>(), null, null));
        Assert.Throws<ArgumentException>(() => aggregate.Add(100, new PresentationSystemGroup(new List<ExecutionNode<float>>(), null, null)));
    }

    [Test]
    public void SortsSystemGroups()
    {
        var aggregate = new OrderedSystemGroupAggregate<int>(Comparer<int>.Default);
        
        var group1 = new PresentationSystemGroup(new List<ExecutionNode<float>>(), null, null);
        var group2 = new PresentationSystemGroup(new List<ExecutionNode<float>>(), null, null);
        var group3 = new PresentationSystemGroup(new List<ExecutionNode<float>>(), null, null);
        
        aggregate.Add(100, group1);
        aggregate.Add(50, group2);
        aggregate.Add(200, group3);
        
        Assert.That(aggregate.Count, Is.EqualTo(3));
        CollectionAssert.AreEqual(new [] {group2, group1, group3}, aggregate.Values);
    }
}