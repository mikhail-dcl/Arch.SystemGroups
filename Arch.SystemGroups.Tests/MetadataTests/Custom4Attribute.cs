namespace Arch.SystemGroups.Tests.MetadataTests;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class Custom4Attribute : Attribute, IEquatable<Custom4Attribute>
{
    public readonly string[] Array;

    public Custom4Attribute(params string[] array)
    {
        Array = array;
    }

    public bool Equals(Custom4Attribute? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && Array.SequenceEqual(other.Array);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Custom4Attribute)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Array);
    }
}