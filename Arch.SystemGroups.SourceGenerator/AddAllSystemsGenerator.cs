using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Arch.SystemGroups.SourceGenerator;

public static class AddAllSystemsGenerator
{
    internal static string GenerateExtensions(WorldsInfo worldsInfo)
    {
        var fileBuilder = new StringBuilder();

        foreach (var pair in worldsInfo.Worlds)
        {
            var worldType = pair.Key;
            var worldInfo = pair.Value;

            var methods = new StringBuilder();

            foreach (var worldInfoGroup in worldInfo.Groups)
            {
                GetInjectToWorldMethodForUniqueArgsCombination(methods, worldType, worldInfoGroup.Key,
                    worldInfoGroup.Value.PassString, worldInfoGroup.Value.Types);
            }

            var classDeclaration =
                $$"""
                public static class {{worldType.Name}}InjectionExtensions 
                {
                    {{methods}}
                }
                """;

            fileBuilder.AppendLine(classDeclaration);
        }

        var template =
            $$"""
            using System;
            namespace Arch.SystemGroups
            {
                {{fileBuilder}}
            }           
            """;

        return template;
    }

    private static void GetInjectToWorldMethodForUniqueArgsCombination(StringBuilder sb, ITypeSymbol worldType, string args, string pass,
        IList<ITypeSymbol> types)
    {
        var injectCalls = GetEachInjectToWorld(args, pass, types);
        
        var template = $$"""
                    public static ref ArchSystemsWorldBuilder<{{worldType}}> AddAllSystems(this ref ArchSystemsWorldBuilder<{{worldType}}> worldBuilder{{args}})
                    {
                        {{injectCalls}}
                        return ref worldBuilder;
                    }
        """;

        static StringBuilder GetEachInjectToWorld(string args, string pass,
            IList<ITypeSymbol> types)
        {
            var sb = new StringBuilder();
            foreach (var type in types)
            {
                sb.AppendLine(
                    $"{CommonUtils.GetTypeReferenceInGlobalNotation(type)}.InjectToWorld(ref worldBuilder{pass});");
            }

            return sb;
        }

        sb.Append(template);
    }
}