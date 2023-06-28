using Arch.SystemGroups.DefaultSystemGroups;
using Arch.SystemGroups.Metadata;
using Arch.SystemGroups.Tests.MetadataTests.Groups;
using Arch.SystemGroups.Tests.MetadataTests.Systems;

namespace Arch.SystemGroups.Tests.MetadataTests;

public class MetadataTests
{
    [Test]
    public void System1ProvidesAttributes()
    {
        var metadata = MetadataSystem1.Metadata;

        Assert.That(metadata, Is.Not.Null);

        var custom1 = metadata.GetAttributes<Custom1Attribute>();
        Assert.That(custom1, Is.Not.Null);
        Assert.That(custom1.Count, Is.EqualTo(1));
        Assert.That(custom1[0].StrValue, Is.EqualTo("TestValue1"));

        var custom2 = metadata.GetAttributes<Custom2Attribute>();

        CollectionAssert.AreEquivalent(new Custom2Attribute[]
        {
            new(100, 200.5f),
            new(1, 2f)
        }, custom2);

        Assert.That(metadata.UpdateInGroup, Is.EqualTo(typeof(SimulationSystemGroup)));
        Assert.That(metadata.GroupMetadata, Is.EqualTo(SystemGroupAttributesInfo.Instance));
    }

    [Test]
    public void System2ProvidesAttributes()
    {
        var metadata = MetadataSystem2.Metadata;

        Assert.That(metadata, Is.Not.Null);

        var custom1 = metadata.GetAttributes<Custom1Attribute>();
        Assert.That(custom1, Is.Not.Null);
        Assert.That(custom1.Count, Is.EqualTo(1));
        Assert.That(custom1[0].StrValue, Is.EqualTo("TestValue2"));

        var custom2 = metadata.GetAttributes<Custom2Attribute>();

        CollectionAssert.AreEquivalent(new Custom2Attribute[]
        {
            new(0, 0f),
            new(1, 1f),
            new(2, 2f),
            new(3, 3f)
        }, custom2);

        Assert.That(metadata.UpdateInGroup, Is.EqualTo(typeof(MetadataGroup1)));
        Assert.That(metadata.GroupMetadata, Is.EqualTo(MetadataGroup1.Metadata));
    }

    [Test]
    public void Group1ProvidesAttributes()
    {
        var metadata = MetadataGroup1.Metadata;

        Assert.That(metadata, Is.Not.Null);

        var custom3 = metadata.GetAttributes<Custom3Attribute>();
        Assert.That(custom3, Is.Not.Null);
        Assert.That(custom3.Count, Is.EqualTo(1));
        Assert.That(custom3[0].Type, Is.EqualTo(typeof(string)));

        var custom4 = metadata.GetAttributes<Custom4Attribute>();

        CollectionAssert.AreEquivalent(new Custom4Attribute[]
        {
            new("TestValue1", "TestValue2")
        }, custom4);

        Assert.That(metadata.UpdateInGroup, Is.EqualTo(typeof(SimulationSystemGroup)));
        Assert.That(metadata.GroupMetadata, Is.EqualTo(SystemGroupAttributesInfo.Instance));
    }

    [Test]
    public void NestedGroup1ProvidesAttributes()
    {
        CollectionAssert.AreEquivalent(new[] { new Custom1Attribute("MetadataNestedGroup1") },
            MetadataNestedGroup1.AttributesInfo.Custom1Attributes);
        
        Assert.That(MetadataNestedGroup1.Metadata.GroupMetadata, Is.EqualTo(MetadataGroup1.Metadata));
    }
}