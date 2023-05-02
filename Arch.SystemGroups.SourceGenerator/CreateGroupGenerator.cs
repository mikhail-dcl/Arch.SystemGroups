namespace Arch.SystemGroups.SourceGenerator;

public static class CreateGroupGenerator
{
    public static string GetTryGetCreateGroup(in GroupInfo groupInfo) =>
        $"worldBuilder.TryCreateGroup<{groupInfo.ClassName}>(typeof({groupInfo.UpdateInGroup}), {EdgesGenerator.AddEdgesCachedFieldName});";
}