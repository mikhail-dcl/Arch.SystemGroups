using System;
using System.Collections.Generic;

/// <summary>
/// Describes a group or system
/// </summary>
public struct Descriptor
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Name of the system</param>
    /// <param name="throttlingEnabled">Is throttling enabled for this system</param>
    /// <param name="subDescriptors"></param>
    public Descriptor(string name, bool throttlingEnabled, List<Descriptor> subDescriptors = null)
    {
        Name = name;
        ThrottlingEnabled = throttlingEnabled;
        SubDescriptors = subDescriptors;
    }

    /// <summary>
    /// Name of the system
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Is throttling enabled for this group or system
    /// </summary>
    public bool ThrottlingEnabled { get; }


    /// <summary>
    /// Is this descriptor a group of other systems or groups
    /// </summary>
    public bool IsGroup => SubDescriptors != null;
    
    
    /// <summary>
    /// Is this descriptor a system
    /// </summary>
    public bool IsSystem => SubDescriptors == null;
    
    
    /// <summary>
    /// A list of sub descriptors if this descriptor is a group
    /// </summary>
    public List<Descriptor> SubDescriptors { get; }
}
