using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Arch.SystemGroups.SourceGenerator
{
    [Generator]
    public class SourceGenerator : IIncrementalGenerator
    {
        private const string UpdateInGroupAttr = "UpdateInGroup";
        private const string UpdateInGroupAttrFullName = "UpdateInGroupAttribute";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Filter Types marked by [UpdateInGroup], [UpdateAfter], [UpdateBefore]
            IncrementalValuesProvider<TypeDeclarationSyntax> typeDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    HasUpdateInGroupAttribute,
                    static (ctx, _) => (TypeDeclarationSyntax) ctx.Node
                );
            
            // Combine methods with compilation
            IncrementalValueProvider<(Compilation, ImmutableArray<TypeDeclarationSyntax>)> compilationAndTypes =
                context.CompilationProvider.Combine(typeDeclarations.WithComparer(TypeComparer.Instance).Collect());
            
            context.RegisterSourceOutput(compilationAndTypes, (spc, source) => Generate(source.Item1, source.Item2, spc));
        }

        private static bool HasUpdateInGroupAttribute(SyntaxNode syntaxNode, CancellationToken cancellationToken)
        {
            if (syntaxNode is not TypeDeclarationSyntax typeDeclarationSyntax) return false;
            if (typeDeclarationSyntax.AttributeLists.Count == 0) return false;
            
            foreach (var attributeListSyntax in typeDeclarationSyntax.AttributeLists)
            {
                for (var i = 0; i < attributeListSyntax.Attributes.Count; i++)
                {
                    var attr = attributeListSyntax.Attributes[i];

                    string name = null;
                    switch (attr.Name)
                    {
                        case SimpleNameSyntax simpleNameSyntax: 
                            name = simpleNameSyntax.Identifier.Text;
                            break;
                        case QualifiedNameSyntax qualifiedNameSyntax:
                            name = qualifiedNameSyntax.Right.Identifier.Text;
                            break;
                    }

                    if (name is UpdateInGroupAttr or UpdateInGroupAttrFullName)
                        return true;
                }
            }

            return false;
        }

        private void Generate(Compilation compilation, ImmutableArray<TypeDeclarationSyntax> types,
            SourceProductionContext context)
        {
            if (types.IsDefaultOrEmpty) return;

            var worldsInfo = new WorldsInfo();

            foreach (var typeDeclarationSyntax in types)
            {
                ITypeSymbol typeSymbol;
                try
                {
                    var semanticModel = compilation.GetSemanticModel(typeDeclarationSyntax.SyntaxTree);
                    typeSymbol = (ITypeSymbol) ModelExtensions.GetDeclaredSymbol(semanticModel, typeDeclarationSyntax);
                }
                catch
                {
                    continue;
                }
                    
                if (typeSymbol == null) continue;

                var template = PartialClassGenerator.ProcessType(typeSymbol, worldsInfo);
                if (template == null) continue;
                var fileName = typeSymbol.ToDisplayString(SymbolDisplayFormat.CSharpShortErrorMessageFormat).Replace('<', '{').Replace('>', '}');
                context.AddSource($"{fileName}.g.cs", CSharpSyntaxTree.ParseText(template).GetRoot().NormalizeWhitespace().ToFullString());
            }

            var allSystemsGen = AddAllSystemsGenerator.GenerateExtensions(worldsInfo);
            context.AddSource("SystemWorlds.g.cs", CSharpSyntaxTree.ParseText(allSystemsGen).GetRoot().NormalizeWhitespace().ToFullString());
        }

        private class TypeComparer : IEqualityComparer<TypeDeclarationSyntax>
        {
            public static readonly TypeComparer Instance = new();
            
            public bool Equals(TypeDeclarationSyntax x, TypeDeclarationSyntax y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(TypeDeclarationSyntax obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}