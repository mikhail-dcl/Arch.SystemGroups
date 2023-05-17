using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Arch.SystemGroups.SourceGenerator;

public static class PartialClassGenerator
{
    private const string AttrNamespace = "Arch.SystemGroups";

    internal static string ProcessType(ITypeSymbol typeSymbol, WorldsInfo worldsInfo)
    {
        // Common parameters
        var isGlobalNamespace = typeSymbol.ContainingNamespace.IsGlobalNamespace;
        var namespc = typeSymbol.ContainingNamespace.ToString();
        var accessModifier = AccessibilityToString(typeSymbol.DeclaredAccessibility);
        var className = typeSymbol.Name;
        var updateInGroup = GetUpdateInGroupType(typeSymbol);
        
        // UpdateAfter and UpdateBefore
        var updateAfter = new List<ITypeSymbol>();
        var updateBefore = new List<ITypeSymbol>();
        GetTypesFromAttributes(typeSymbol, updateAfter, updateBefore);

        // Differentiate between Group And System
        // If type symbol implements ISystem<float> interface then it is a system
        // We don't consider other than float argument
        var isSystem = typeSymbol.AllInterfaces.Any(x => x.IsGenericType && x.ContainingNamespace.ToString() == "Arch.System" 
                        && x.Name == "ISystem" && x.TypeArguments.Length == 1 && x.TypeArguments[0].Name is "Single" or "float");
        if (isSystem)
        {
            // Find the first constructor, ignore other constructors
            var members = typeSymbol.GetMembers();
            // If there is no explicit constructor there is always one empty constructor with no parameters
            var constructorParams = members.FirstOrDefault(x => x is IMethodSymbol { MethodKind: MethodKind.Constructor }) is IMethodSymbol methodSymbol
                ? methodSymbol.Parameters
                : ImmutableArray<IParameterSymbol>.Empty;
            
            // There should be World parameter for all systems
            // if the system implements BaseSystem{W,T}'2 then W is the world type
            // Otherwise just the first constructor argument is the world

            ITypeSymbol FindWorldType(ITypeSymbol currentType)
            {
                var baseType = currentType.BaseType;
                
                if (baseType == null)
                    return null;
                
                if (baseType.IsGenericType && baseType.ContainingNamespace.ToString() == "Arch.System" 
                    && baseType.Name == "BaseSystem" && baseType.TypeArguments.Length == 2)
                    return baseType.TypeArguments[0];

                return FindWorldType(baseType);
            }

            var worldType = FindWorldType(typeSymbol) ?? constructorParams.ElementAtOrDefault(0)?.Type;
            if (worldType == null)
            {
                // We can't do anything without the world type
                return null;
            }

            constructorParams = constructorParams.RemoveAt(0);

            var systemInfo = new SystemInfo
            {
                This = typeSymbol,
                IsGlobalNamespace = isGlobalNamespace,
                Namespace = namespc,
                AccessModifier = accessModifier,
                ClassName = className,
                UpdateInGroup = updateInGroup,
                UpdateAfter = updateAfter,
                UpdateBefore = updateBefore,
                ConstructorParams = constructorParams,
                WorldType = worldType
            };
            return GetSystemPartialClass(in systemInfo, worldsInfo.GetWorldInfo(worldType));
        }

        var groupInfo = new GroupInfo
        {
            This = typeSymbol,
            IsGlobalNamespace = isGlobalNamespace,
            Namespace = namespc,
            AccessModifier = accessModifier,
            ClassName = className,
            UpdateInGroup = updateInGroup,
            UpdateAfter = updateAfter,
            UpdateBefore = updateBefore
        };
        return GetGroupPartialClass(in groupInfo);
    }
    
    private static ITypeSymbol GetUpdateInGroupType(ITypeSymbol typeSymbol)
    {
        // Roslyn reports the fully qualified name
        const string updateInGroupAttrName = "UpdateInGroupAttribute";
        var attributes = typeSymbol.GetAttributes();
        foreach (var attribute in attributes)
        {
            var attrName = attribute.AttributeClass.Name;
            var attrNamespaceName = attribute.AttributeClass.ContainingNamespace.ToString();
            if (attrNamespaceName == AttrNamespace && attrName == updateInGroupAttrName)
            {
                if (attribute.ConstructorArguments[0].Value is ITypeSymbol updateInGroupType)
                    return updateInGroupType;
            }
        }

        throw new ArgumentException("UpdateInGroup attribute is not found");
    }
    
    private static void GetTypesFromAttributes(ITypeSymbol analyzedType, IList<ITypeSymbol> updateAfter, IList<ITypeSymbol> updateBefore)
    {
        const string updateAfterAttrName = "UpdateAfterAttribute";
        const string updateBeforeAttrName = "UpdateBeforeAttribute";

        var attributes = analyzedType.GetAttributes();
        foreach (var attribute in attributes)
        {
            var attrName = attribute.AttributeClass.Name;
            var attrNamespaceName = attribute.AttributeClass.ContainingNamespace.ToString();
            if (attrNamespaceName == AttrNamespace)
            {
                switch (attrName)
                {
                    case updateAfterAttrName:
                    {
                        if (attribute.ConstructorArguments[0].Value is ITypeSymbol updateAfterType)
                            updateAfter.Add(updateAfterType);
                        break;
                    }
                    case updateBeforeAttrName:
                    {
                        if (attribute.ConstructorArguments[0].Value is ITypeSymbol updateBeforeType)
                            updateBefore.Add(updateBeforeType);
                        break;
                    }
                }
            }
        }
    }

    private static string AccessibilityToString(Accessibility accessibility)
    {
        switch (accessibility)
        {
            case Accessibility.Private:
                return "private";
            case Accessibility.Internal:
                return "internal";
            case Accessibility.Protected:
                return "protected";
            case Accessibility.ProtectedOrInternal:
                return "protected internal";
            default:
                return "public";
        }
    }
    
    private static string GetSystemPartialClass(in SystemInfo systemInfo, WorldInfo worldInfo)
    {
        var addEdgesFieldDeclaration = EdgesGenerator.GetAddEdgesCachedField();
        var addEdgesBody = EdgesGenerator.GetAddEdgesBody(systemInfo.UpdateBefore, systemInfo.UpdateAfter, systemInfo.ClassName, systemInfo.This);

        var injectToWorldMethodParams = InjectToWorldGenerator.GetMethodArgumentsWithoutWorld(in systemInfo).ToString();
        var passArguments = InjectToWorldGenerator.GetPassArguments(in systemInfo).ToString();
        var systemInstantiation = InjectToWorldGenerator.GetSystemInstantiation(in systemInfo);
        var createGroup = InjectToWorldGenerator.GetGroupInjectionInvocation(in systemInfo);
        var addToGroup = InjectToWorldGenerator.GetAddToGroup(in systemInfo);
        var worldType = systemInfo.WorldType;
        
        worldInfo.AddSystem(systemInfo.This, systemInfo.ConstructorParams, injectToWorldMethodParams, passArguments);
        
        var template =
            $$"""
            using System;
            using System.Collections.Generic;
            using Arch.SystemGroups.DefaultSystemGroups;
            using Arch.SystemGroups;

            {{(!systemInfo.IsGlobalNamespace ? $"namespace {systemInfo.Namespace} {{" : "")}}

                {{systemInfo.AccessModifier}} static class {{systemInfo.ClassName}}InjectionExtensions 
                {
                    public static ref ArchSystemsWorldBuilder<{{worldType}}> Add{{systemInfo.ClassName}}(this ref ArchSystemsWorldBuilder<{{worldType}}> worldBuilder{{injectToWorldMethodParams}})
                    {
                        {{systemInfo.ClassName}}.InjectToWorld(ref worldBuilder{{passArguments}});
                        return ref worldBuilder;
                    }
                }

                {{systemInfo.AccessModifier}} partial class {{systemInfo.ClassName}}{
                    
                {{addEdgesFieldDeclaration}}

                public static {{systemInfo.ClassName}} InjectToWorld(ref ArchSystemsWorldBuilder<{{worldType}}> worldBuilder{{injectToWorldMethodParams}})
                {
                    {{systemInstantiation}}
                    {{createGroup}}
                    {{addToGroup}}
                    return system;
                }

                private static void AddEdges(Dictionary<Type, List<Type>> edgesMap)
                {
                    {{addEdgesBody}}
                }
            {{(!systemInfo.IsGlobalNamespace ? "}" : "")}}
            }
            """;

        return template;
    }

    public static string GetGroupPartialClass(in GroupInfo groupInfo)
    {
        var addEdgesFieldDeclaration = EdgesGenerator.GetAddEdgesCachedField();
        var addEdgesBody = EdgesGenerator.GetAddEdgesBody(groupInfo.UpdateBefore, groupInfo.UpdateAfter, groupInfo.ClassName, groupInfo.This);

        var createGroup = CreateGroupGenerator.GetTryGetCreateGroup(in groupInfo);
        
        var template =
            $$"""
            using System;
            using System.Collections.Generic;
            using Arch.System;
            using Arch.SystemGroups;

            {{(!groupInfo.IsGlobalNamespace ? $"namespace {groupInfo.Namespace} {{" : "")}}
                {{groupInfo.AccessModifier}} partial class {{groupInfo.ClassName}} : CustomGroup<float> {
                    
                {{addEdgesFieldDeclaration}}

                public static void TryCreateGroup<T>(ref ArchSystemsWorldBuilder<T> worldBuilder)
                {                    
                    {{createGroup}}
                }

                private static void AddEdges(Dictionary<Type, List<Type>> edgesMap)
                {
                    {{addEdgesBody}}
                }
            {{(!groupInfo.IsGlobalNamespace ? "}" : "")}}
            }
            """;

        return template;
    }
        
}