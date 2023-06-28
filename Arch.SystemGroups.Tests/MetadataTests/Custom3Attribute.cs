namespace Arch.SystemGroups.Tests.MetadataTests;

[AttributeUsage(AttributeTargets.Class)]
public class Custom3Attribute : Attribute
{
    public readonly Type Type;
    
    public Custom3Attribute(Type type)
    {
        Type = type;
    }
}