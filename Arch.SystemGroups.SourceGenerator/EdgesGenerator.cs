using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Arch.SystemGroups.SourceGenerator
{
    public static class EdgesGenerator
    {
        public const string AddEdgesCachedFieldName = "_addEdgesCached";
        public const string ValidateEdgesCachedFieldName = "_validateEdgesCached";
        public const string ValidateEdgesMethodName = "ValidateEdges";
        public const string DisconnectedDependenciesFieldName = "disconnectedDependencies";
        
        public static string GetValidateEdgesCachedField() =>
            $"private static readonly Action<List<DisconnectedDependenciesInfo.WrongTypeBinding>> {ValidateEdgesCachedFieldName} = {ValidateEdgesMethodName};";

        public static string GetValidateEdgesBody(IList<ITypeSymbol> updateBefore, IList<ITypeSymbol> updateAfter, 
            ITypeSymbol group, string className, ITypeSymbol thisType, string typeGenericArguments)
        {
            var builder = new StringBuilder();

            foreach (var dependency in updateBefore)
                InsertValidateEdge(dependency);
            
            foreach (var dependency in updateAfter)
                InsertValidateEdge(dependency);

            return builder.ToString();

            void InsertValidateEdge(ITypeSymbol dependency)
            {
                // Filter out references to self
                if (dependency.Equals(thisType, SymbolEqualityComparer.Default))
                    return;

                builder.AppendFormat(
                    $"ArchSystemsSorter.ValidateEdge({DisconnectedDependenciesFieldName}, typeof({className}{typeGenericArguments}), typeof({group}), typeof({dependency}), {dependency}.Metadata.UpdateInGroup);");
            }
        }
        
        public static string GetAddEdgesCachedField() =>
            $"private static readonly Action<Dictionary<Type, List<Type>>> {AddEdgesCachedFieldName} = AddEdges;";
        
        public static StringBuilder GetAddEdgesBody(IList<ITypeSymbol> updateBefore, IList<ITypeSymbol> updateAfter, string className, ITypeSymbol thisType, string typeGenericArguments)
        {
            var builder = new StringBuilder();

            foreach (var typeSymbol in updateAfter)
            {
                // Filter out references to self
                if (typeSymbol.Equals(thisType, SymbolEqualityComparer.Default))
                    continue;
                
                // Update After = from That Type to This
                builder.AppendLine(
                    $"ArchSystemsSorter.AddEdge(typeof({typeSymbol}), typeof({className}{typeGenericArguments}), edgesMap);");
            }

            foreach (var typeSymbol in updateBefore)
            {
                if (typeSymbol.Equals(thisType, SymbolEqualityComparer.Default))
                    continue;
                
                // Update Before = from This Type to That
                builder.AppendLine(
                    $"ArchSystemsSorter.AddEdge(typeof({className}{typeGenericArguments}), typeof({typeSymbol}), edgesMap);");
            }

            if (builder.Length == 0)
                builder.AppendLine("// No Dependencies");

            return builder;
        }
    }
}