namespace Arch.SystemGroups.Tests.MetadataTests;

[AttributeUsage(AttributeTargets.Class)]
public class Custom1Attribute : Attribute, IEquatable<Custom1Attribute>
{
    public readonly string StrValue;
    
    public Custom1Attribute(string str)
    {
        StrValue = str;
    }

    public bool Equals(Custom1Attribute? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && StrValue == other.StrValue;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Custom1Attribute)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), StrValue);
    }
}