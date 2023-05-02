using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Arch.SystemGroups.SourceGenerator;

/// <summary>
/// Groups do not implement <see cref="ISystem'1"/> interface
/// and do not contain constructor
/// </summary>
public struct GroupInfo
{
    /// <summary>
    /// This type of the class containing the Update Attributes.
    /// </summary>
    public ITypeSymbol This { get; set; }
    
    /// <summary>
    /// If the class containing Update Attributes method is within the global namespace.
    /// </summary>
    public bool IsGlobalNamespace { get; set; }
    
    /// <summary>
    /// Public, internal or private
    /// </summary>
    public string AccessModifier { get; set; }
    
    /// <summary>
    /// The namespace of the method.
    /// </summary>
    public string Namespace { get; set; }
        
    /// <summary>
    /// The name of the class containing these Update Attributes.
    /// </summary>
    public string ClassName { get; set; }

    /// <summary>
    /// Multiple Attributes that are used to define the order of the system.
    /// [UpdateBefore(typeof(CustomSystem))]
    /// </summary>
    public IList<ITypeSymbol> UpdateBefore { get; set; } 
        
    /// <summary>
    /// Multiple Attributes that are used to define the order of the system.
    /// [UpdateAfter(typeof(CustomSystem))]
    /// </summary>
    public IList<ITypeSymbol> UpdateAfter { get; set; }
        
    /// <summary>
    /// The type of the SystemGroup or CustomType
    /// </summary>
    public ITypeSymbol UpdateInGroup { get; set; }
}