using System;
using System.Collections.Generic;
using System.Linq;

namespace Arch.SystemGroups
{
    /// <summary>
    /// Wrong dependencies were detected on traversal of the dependency graph
    /// </summary>
    public class DisconnectedDependenciesFoundException : Exception
    {
        private readonly IReadOnlyList<DisconnectedDependenciesInfo> _disconnectedDependencies;

        internal DisconnectedDependenciesFoundException(IReadOnlyList<DisconnectedDependenciesInfo> disconnectedDependencies)
        {
            _disconnectedDependencies = disconnectedDependencies;
        }

        public override string Message => string.Join(Environment.NewLine, _disconnectedDependencies.Select(PrintDependencyInfo));

        private static string PrintDependencyInfo(DisconnectedDependenciesInfo disconnectedDependenciesInfo)
        {
            static string PrintWrongBinding(DisconnectedDependenciesInfo.WrongTypeBinding wrongTypeBinding)
            {
                return wrongTypeBinding.DeclaredOn == null
                    ? $"{wrongTypeBinding.DependencyType.Name} (declared on a group)"
                    : $"{wrongTypeBinding.DependencyType.Name} (declared on {wrongTypeBinding.DeclaredOn.Name})";
            }
        
            return $"The following dependencies don't belong to the group {disconnectedDependenciesInfo.GroupType.Name}: " +
                   $"{string.Join(", ", disconnectedDependenciesInfo.WrongDependencies.Select(PrintWrongBinding))}";
        }
    }
}