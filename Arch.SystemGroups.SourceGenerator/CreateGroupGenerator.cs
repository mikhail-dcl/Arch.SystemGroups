namespace Arch.SystemGroups.SourceGenerator;

public static class CreateGroupGenerator
{
    public static string GetTryGetCreateGroup(in GroupInfo groupInfo)
    {
        return groupInfo.Behaviour is { IsCustom: true, HasParameterlessConstructor: false } 
            ? $"worldBuilder.TryRegisterGroup<{groupInfo.ClassName}>(typeof({groupInfo.UpdateInGroup}), {EdgesGenerator.AddEdgesCachedFieldName}, {EdgesGenerator.ValidateEdgesCachedFieldName}, {groupInfo.ThrottlingEnabled.ToString().ToLower()});" 
            : $"worldBuilder.TryCreateGroup<{groupInfo.ClassName}>(typeof({groupInfo.UpdateInGroup}), {EdgesGenerator.AddEdgesCachedFieldName}, {EdgesGenerator.ValidateEdgesCachedFieldName}, {groupInfo.ThrottlingEnabled.ToString().ToLower()});";
    }
}