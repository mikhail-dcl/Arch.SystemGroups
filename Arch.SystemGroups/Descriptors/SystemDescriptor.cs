/// <summary>
/// 
/// </summary>
public class SystemDescriptor
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Name of the system</param>
    public SystemDescriptor(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Name of the system
    /// </summary>
    public string Name { get; }
}
