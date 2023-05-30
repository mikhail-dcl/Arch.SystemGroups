using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Arch.SystemGroups.SourceGenerator
{
    public static class EdgesGenerator
    {
        public const string AddEdgesCachedFieldName = "_addEdgesCached";
        
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