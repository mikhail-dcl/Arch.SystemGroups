namespace Arch.SystemGroups.Tests.MetadataTests;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class Custom2Attribute : Attribute, IEquatable<Custom2Attribute>
{
    public readonly int V1;
    public readonly float V2;
    
    public Custom2Attribute(int v1, float v2)
    {
        V1 = v1;
        V2 = v2;
    }

    public bool Equals(Custom2Attribute? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && V1 == other.V1 && V2.Equals(other.V2);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Custom2Attribute)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), V1, V2);
    }
}