using System.Text;

namespace Arch.SystemGroups.SourceGenerator
{
    public static class InjectToWorldGenerator
    {
        public static StringBuilder GetMethodArgumentsWithoutWorld(in SystemInfo systemInfo)
        {
            var sb = new StringBuilder();
            // Assume that the first parameter is always the world
            for (var i = 0; i < systemInfo.ConstructorParams.Length; i++)
            {
                sb.Append(", ");
                var param = systemInfo.ConstructorParams[i];
                sb.Append($"{CommonUtils.RefKindToString(param.RefKind)} {CommonUtils.GetTypeReferenceInGlobalNotation(param.Type)} {param.Name}");
            }

            return sb;
        }

        public static StringBuilder GetPassArguments(in SystemInfo systemInfo)
        {
            var sb = new StringBuilder();
            // Assume that the first parameter is always the world
            for (var i = 0; i < systemInfo.ConstructorParams.Length; i++)
            {
                sb.Append(", ");
                var param = systemInfo.ConstructorParams[i];
                sb.Append($"{CommonUtils.RefKindToString(param.RefKind)} {param.Name}");
            }

            return sb;
        }

        /// <summary>
        /// If the group is not a SystemGroup then the message to call that group creation will be called
        /// </summary>
        /// <param name="systemInfo"></param>
        /// <returns></returns>
        public static string GetGroupInjectionInvocation(in SystemInfo systemInfo)
        {
            var updateInGroup = systemInfo.UpdateInGroup;
            
            if (updateInGroup.BaseType?.Name == "SystemGroup" && updateInGroup.BaseType?.ContainingNamespace?.ToString() == "Arch.SystemGroups")
                return string.Empty;

            return $"{systemInfo.UpdateInGroup}.TryCreateGroup(ref worldBuilder);";
        }

        public static StringBuilder GetAddToGroup(in SystemInfo systemInfo)
        {
            var sb = new StringBuilder();

            sb.Append("worldBuilder.AddToGroup(");
            sb.Append("system, ");
            sb.Append($"typeof({systemInfo.UpdateInGroup}), ");
            sb.Append($"typeof({systemInfo.ClassName}), ");
            sb.Append(EdgesGenerator.AddEdgesCachedFieldName);
            sb.Append(");");
            
            return sb;
        }

        public static StringBuilder GetSystemInstantiation(in SystemInfo systemInfo)
        {
            var sb = new StringBuilder("var system = new ");
            sb.Append(systemInfo.ClassName);
            sb.Append("(");
            sb.Append("worldBuilder.World");
            for (var i = 0; i < systemInfo.ConstructorParams.Length; i++)
            {
                sb.Append(", ");
                var param = systemInfo.ConstructorParams[i];
                sb.Append($"{CommonUtils.RefKindToString(param.RefKind)} {param.Name}");
            }
            sb.Append(");");
            return sb;
        }
    }
}