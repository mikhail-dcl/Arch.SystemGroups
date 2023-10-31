/// <summary>
/// 
/// </summary>
public struct SystemDescriptor
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Name of the system</param>
    /// <param name="throttlingEnabled">Is throttling enabled for this system</param>
    public SystemDescriptor(string name, bool throttlingEnabled)
    {
        Name = name;
        ThrottlingEnabled = throttlingEnabled;
    }

    /// <summary>
    /// Name of the system
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Is throttling enabled for this system
    /// </summary>
    public bool ThrottlingEnabled { get; }
}
