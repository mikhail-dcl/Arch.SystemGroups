using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Arch.SystemGroups.SourceGenerator;

public static class MetadataGenerator
{
    private static string GenerateAttributesInstances(
        IReadOnlyList<IGrouping<ISymbol, AttributeData>> groupedAttributes)
    {
        var sb = new StringBuilder();

        foreach (var groupedAttribute in groupedAttributes)
        {
            sb.AppendLine(
                $"public static readonly IReadOnlyList<{groupedAttribute.Key}> {groupedAttribute.Key.Name}s = new {groupedAttribute.Key}[]");

            sb.AppendLine("{");

            foreach (var attributeData in groupedAttribute)
            {
                sb.Append("    new(");
                sb.Append(string.Join(", ", attributeData.ConstructorArguments.Select(x => x.ToCSharpStringSemanticallyValid())));
                sb.AppendLine("),");
            }

            sb.AppendLine("};");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static string GenerateGetFunction(IReadOnlyList<IGrouping<ISymbol, AttributeData>> groupedAttributes)
    {
        var sb = new StringBuilder();

        sb.AppendLine("public override T GetAttribute<T>()");
        sb.AppendLine("{");

        foreach (var groupedAttribute in groupedAttributes)
        {
            sb.AppendLine(
                $"    if (typeof(T) == typeof({groupedAttribute.Key}))");
            sb.AppendLine(
                $"        return (T)(object){groupedAttribute.Key.Name}s[0];");
        }

        sb.AppendLine("return null;");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string GenerateGetListFunction(IReadOnlyList<IGrouping<ISymbol, AttributeData>> groupedAttributes)
    {
        var sb = new StringBuilder();

        sb.AppendLine("public override IReadOnlyList<T> GetAttributes<T>()");
        sb.AppendLine("{");

        foreach (var groupedAttribute in groupedAttributes)
        {
            sb.AppendLine(
                $"    if (typeof(T) == typeof({groupedAttribute.Key}))");
            sb.AppendLine(
                $"        return (IReadOnlyList<T>){groupedAttribute.Key.Name}s;");
        }

        sb.AppendLine("return Array.Empty<T>();");
        sb.AppendLine("}");

        return sb.ToString();
    }

    internal static string GenerateAttributesInfo(ITypeSymbol classSymbol, ITypeSymbol updateInGroupSymbol)
    {
        // TODO collect all attributes that can be inherited from base classes

        var attributes = classSymbol.GetAttributes().GroupBy(a => a.AttributeClass, SymbolEqualityComparer.Default)
            .ToList();
        var attributesInstances = GenerateAttributesInstances(attributes);
        var getFunction = GenerateGetFunction(attributes);
        var getListFunction = GenerateGetListFunction(attributes);

        return
            $$"""
            public class AttributesInfo : AttributesInfoBase
            {
                public override Type UpdateInGroup { get; } = typeof({{updateInGroupSymbol}});
        
                public override AttributesInfoBase GroupMetadata => {{updateInGroupSymbol}}.Metadata;

                {{attributesInstances}}

                {{getFunction}}

                {{getListFunction}}
            }
    
            private static AttributesInfo _attributesInfoCached;

            public static readonly AttributesInfo Metadata = _attributesInfoCached ??= new ();
            """;
    }
}