using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Arch.SystemGroups;

/// <summary>
/// Contains information about the specified dependencies
/// which do not belong to the given hierarchy
/// </summary>
public readonly struct DisconnectedDependenciesInfo
{
    /// <summary>
    /// Denotes a pair of dependency type and the type it was declared on
    /// </summary>
    public readonly struct WrongTypeBinding
    {
        /// <summary>
        /// Type of the dependency
        /// </summary>
        public readonly Type DependencyType;
        
        /// <summary>
        /// Type the dependency was declared on
        /// </summary>
        [CanBeNull] public readonly Type DeclaredOn;

        internal WrongTypeBinding(Type dependencyType, Type declaredOn)
        {
            DependencyType = dependencyType;
            DeclaredOn = declaredOn;
        }
    }
    
    /// <summary>
    /// Type of the group
    /// </summary>
    public readonly Type GroupType;

    /// <summary>
    /// 
    /// </summary>
    public readonly IReadOnlyList<WrongTypeBinding> WrongDependencies;

    internal DisconnectedDependenciesInfo(Type groupType, IReadOnlyList<WrongTypeBinding> wrongDependencies)
    {
        GroupType = groupType;
        WrongDependencies = wrongDependencies;
    }
}