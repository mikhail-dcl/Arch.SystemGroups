using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Arch.SystemGroups.SourceGenerator;

internal class WorldInfo
{
    internal class GroupByArgs
    {
        public IList<ITypeSymbol> Types { get; set; }
        public string PassString { get; set; }
    }
        
    internal ITypeSymbol WorldType { get; }
        
    internal Dictionary<string, GroupByArgs> Groups { get; }

    internal void AddSystem(ITypeSymbol systemType, string args, string argsPass)
    {
        if (!Groups.TryGetValue(args, out var groupByArgs))
            Groups[args] = groupByArgs = new GroupByArgs {Types = new List<ITypeSymbol>(), PassString = argsPass};
        
        groupByArgs.Types.Add(systemType);
    }
        
    public WorldInfo(Dictionary<string, GroupByArgs> groups, ITypeSymbol worldType)
    {
        Groups = groups;
        WorldType = worldType;
    }
}

internal class WorldsInfo
{
    internal Dictionary<ITypeSymbol, WorldInfo> Worlds { get; } = new(SymbolEqualityComparer.Default);

    public WorldInfo GetWorldInfo(ITypeSymbol type) => Worlds.TryGetValue(type, out var world)
        ? world
        : Worlds[type] = new WorldInfo(new Dictionary<string, WorldInfo.GroupByArgs>(), type);
}