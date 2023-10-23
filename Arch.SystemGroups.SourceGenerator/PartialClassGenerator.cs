using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Arch.SystemGroups.SourceGenerator;

public static class PartialClassGenerator
{
    private const string TypesAttrNamespace = "Arch.SystemGroups";
    private const string ThrottlingNamespace = "Arch.SystemGroups.Throttling";

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
        ProcessAttributes(typeSymbol, updateAfter, updateBefore, out bool throttlingEnabled);

        var constructors = typeSymbol.GetConstructors().ToImmutableArray();
        
        GroupInfo.BehaviourInfo GetGroupBehaviourInfo(ITypeSymbol currentType)
        {
            var baseType = currentType.BaseType;
                
            if (baseType == null)
                return new GroupInfo.BehaviourInfo {IsCustom = false, HasParameterlessConstructor = true};

            if (baseType.IsGenericType && baseType.ContainingNamespace.ToString() == "Arch.SystemGroups"
                                       && baseType.Name == "CustomGroupBase" && baseType.TypeArguments.Length == 1)
            {
                return new GroupInfo.BehaviourInfo
                {
                    IsCustom = true,
                    HasParameterlessConstructor =
                        constructors.Length == 0 || constructors.Any(c => c.Parameters.Length == 0)
                };
            }

            return GetGroupBehaviourInfo(baseType);
        }

        // Differentiate between Group And System
        // If type symbol implements ISystem<float> interface then it is a system
        // We don't consider other than float argument
        var isSystem = typeSymbol.AllInterfaces.Any(x => x.IsGenericType && x.ContainingNamespace.ToString() == "Arch.System" 
                        && x.Name == "ISystem" && x.TypeArguments.Length == 1 && x.TypeArguments[0].Name is "Single" or "float");

        var groupBehaviourInfo = GetGroupBehaviourInfo(typeSymbol);
        
        if (isSystem && !groupBehaviourInfo.IsCustom)
        {
            // Find the first constructor, ignore other constructors
            // If there is no explicit constructor there is always one empty constructor with no parameters
            var constructorParams = constructors.FirstOrDefault()?.Parameters ?? ImmutableArray<IParameterSymbol>.Empty;
            
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
            
            bool InheritsFromPlayerLoopSystem(ITypeSymbol currentType)
            {
                var baseType = currentType.BaseType;
                
                if (baseType == null)
                    return false;
                
                if (baseType.IsGenericType && baseType.ContainingNamespace.ToString() == "Arch.SystemGroups" 
                    && baseType.Name == "PlayerLoopSystem" && baseType.TypeArguments.Length == 1)
                    return true;

                return InheritsFromPlayerLoopSystem(baseType);
            }
            
            var inheritsFromPlayerLoopSystem = InheritsFromPlayerLoopSystem(typeSymbol);

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
                WorldType = worldType,
                ThrottlingEnabled = throttlingEnabled,
                InheritsFromPlayerLoopSystem = inheritsFromPlayerLoopSystem
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
            Behaviour = groupBehaviourInfo,
            UpdateInGroup = updateInGroup,
            UpdateAfter = updateAfter,
            UpdateBefore = updateBefore,
            ThrottlingEnabled = throttlingEnabled
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
            if (attrNamespaceName == TypesAttrNamespace && attrName == updateInGroupAttrName)
            {
                if (attribute.ConstructorArguments[0].Value is ITypeSymbol updateInGroupType)
                    return updateInGroupType;
            }
        }

        throw new ArgumentException("UpdateInGroup attribute is not found");
    }
    
    private static void ProcessAttributes(ITypeSymbol analyzedType, IList<ITypeSymbol> updateAfter, IList<ITypeSymbol> updateBefore, out bool throttlingEnabled)
    {
        const string updateAfterAttrName = "UpdateAfterAttribute";
        const string updateBeforeAttrName = "UpdateBeforeAttribute";
        const string throttlingEnabledAttrName = "ThrottlingEnabledAttribute";
        
        throttlingEnabled = false;

        var throttling = false;
        
        AttributesUtils.DoOnAttributes(analyzedType, 
            (attr => updateAfter.Add((ITypeSymbol) attr.ConstructorArguments[0].Value), attr => attr.AttributeClass.Name == updateAfterAttrName && attr.AttributeClass.ContainingNamespace.ToString() == TypesAttrNamespace, false),
            (attr => updateBefore.Add((ITypeSymbol) attr.ConstructorArguments[0].Value), attr => attr.AttributeClass.Name == updateBeforeAttrName && attr.AttributeClass.ContainingNamespace.ToString() == TypesAttrNamespace, false),
            (_ => throttling = true, attr => attr.AttributeClass.Name == throttlingEnabledAttrName && attr.AttributeClass.ContainingNamespace.ToString() == ThrottlingNamespace, true)
        );

        throttlingEnabled = throttling;
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
        var typeGenericArguments = CommonUtils.GetGenericArguments(systemInfo.This, false);
        var typeGenericArgumentsWhere = CommonUtils.GetGenericConstraintsString(systemInfo.This);
        var addEdgesFieldDeclaration = EdgesGenerator.GetAddEdgesCachedField();
        var addEdgesBody = EdgesGenerator.GetAddEdgesBody(systemInfo.UpdateBefore, systemInfo.UpdateAfter, systemInfo.ClassName, systemInfo.This, typeGenericArguments);

        var validateEdgesFieldDeclaration = EdgesGenerator.GetValidateEdgesCachedField();
        var validateEdgesBody = EdgesGenerator.GetValidateEdgesBody(systemInfo.UpdateBefore, systemInfo.UpdateAfter,
            systemInfo.UpdateInGroup, systemInfo.ClassName, systemInfo.This, typeGenericArguments);
        
        var injectToWorldMethodParams = InjectToWorldGenerator.GetMethodArgumentsWithoutWorld(in systemInfo).ToString();
        var passArguments = InjectToWorldGenerator.GetPassArguments(in systemInfo).ToString();
        var systemInstantiation = InjectToWorldGenerator.GetSystemInstantiation(in systemInfo, typeGenericArguments);
        var createGroup = InjectToWorldGenerator.GetGroupInjectionInvocation(systemInfo.UpdateInGroup);
        var addToGroup = InjectToWorldGenerator.GetAddToGroup(in systemInfo, typeGenericArguments);
        var worldType = systemInfo.WorldType;
        
        if (!systemInfo.This.IsGenericType())
            worldInfo.AddSystem(systemInfo.This, systemInfo.ConstructorParams, injectToWorldMethodParams, passArguments);

        var metadata = MetadataGenerator.GenerateAttributesInfo(systemInfo.This, systemInfo.UpdateInGroup);
        
        var getMetadataOverride = systemInfo.InheritsFromPlayerLoopSystem
            ? "protected override AttributesInfoBase GetMetadataInternal() => Metadata;"
            : string.Empty;
        
        var template =
            $$"""
            using System;
            using System.Collections.Generic;
            using Arch.SystemGroups.DefaultSystemGroups;
            using Arch.SystemGroups;
            using Arch.SystemGroups.Metadata;

            {{(!systemInfo.IsGlobalNamespace ? $"namespace {systemInfo.Namespace} {{" : "")}}

                {{systemInfo.AccessModifier}} static class {{systemInfo.ClassName}}InjectionExtensions 
                {
                    public static ref ArchSystemsWorldBuilder<{{worldType}}> Add{{systemInfo.ClassName}}{{typeGenericArguments}}(this ref ArchSystemsWorldBuilder<{{worldType}}> worldBuilder{{injectToWorldMethodParams}}) {{typeGenericArgumentsWhere}}
                    {
                        {{systemInfo.ClassName}}{{typeGenericArguments}}.InjectToWorld(ref worldBuilder{{passArguments}});
                        return ref worldBuilder;
                    }
                }

                {{systemInfo.AccessModifier}} partial class {{systemInfo.ClassName}}{{typeGenericArguments}}{
                    
                {{addEdgesFieldDeclaration}}
                
                {{validateEdgesFieldDeclaration}}

                public static {{systemInfo.ClassName}}{{typeGenericArguments}} InjectToWorld(ref ArchSystemsWorldBuilder<{{worldType}}> worldBuilder{{injectToWorldMethodParams}})
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
                
                private static void {{EdgesGenerator.ValidateEdgesMethodName}}(List<DisconnectedDependenciesInfo.WrongTypeBinding> {{EdgesGenerator.DisconnectedDependenciesFieldName}})
                {
                    {{validateEdgesBody}}
                }

                {{metadata}}

                {{getMetadataOverride}}

            {{(!systemInfo.IsGlobalNamespace ? "}" : "")}}
            }
            """;

        return template;
    }

    public static string GetGroupPartialClass(in GroupInfo groupInfo)
    {
        var addEdgesFieldDeclaration = EdgesGenerator.GetAddEdgesCachedField();
        var addEdgesBody = EdgesGenerator.GetAddEdgesBody(groupInfo.UpdateBefore, groupInfo.UpdateAfter, groupInfo.ClassName, groupInfo.This, string.Empty);

        var validateEdgesFieldDeclaration = EdgesGenerator.GetValidateEdgesCachedField();
        var validateEdgesBody = EdgesGenerator.GetValidateEdgesBody(groupInfo.UpdateBefore, groupInfo.UpdateAfter,
            groupInfo.UpdateInGroup, groupInfo.ClassName, groupInfo.This, string.Empty);
        
        // If there is no system directly attached to the group we need to create the tree of groups, otherwise the leaf system will be detached
        var groupInjection = InjectToWorldGenerator.GetGroupInjectionInvocation(groupInfo.UpdateInGroup);
        var createGroup = CreateGroupGenerator.GetTryGetCreateGroup(in groupInfo);
        var customBehaviour = groupInfo.Behaviour.IsCustom;
        
        var metadata = MetadataGenerator.GenerateAttributesInfo(groupInfo.This, groupInfo.UpdateInGroup);
        
        var template =
            $$"""
            using System;
            using System.Collections.Generic;
            using Arch.System;
            using Arch.SystemGroups;
            using Arch.SystemGroups.Metadata;

            {{(!groupInfo.IsGlobalNamespace ? $"namespace {groupInfo.Namespace} {{" : "")}}
                {{groupInfo.AccessModifier}} partial class {{groupInfo.ClassName}}{{(customBehaviour ? string.Empty : ": DefaultGroup<float>")}} {
                    
                {{addEdgesFieldDeclaration}}
                
                {{validateEdgesFieldDeclaration}}

                public static void TryCreateGroup<T>(ref ArchSystemsWorldBuilder<T> worldBuilder)
                {
                    {{groupInjection}}
                    {{createGroup}}
                }

                private static void AddEdges(Dictionary<Type, List<Type>> edgesMap)
                {
                    {{addEdgesBody}}
                }
                
                private static void {{EdgesGenerator.ValidateEdgesMethodName}}(List<DisconnectedDependenciesInfo.WrongTypeBinding> {{EdgesGenerator.DisconnectedDependenciesFieldName}})
                {
                    {{validateEdgesBody}}
                }

                {{metadata}}

                protected override AttributesInfoBase GetMetadataInternal() => Metadata;

            {{(!groupInfo.IsGlobalNamespace ? "}" : "")}}
            }
            """;

        return template;
    }
        
}