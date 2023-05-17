using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Arch.SystemGroups.SourceGenerator;

internal class WorldInfo : IEqualityComparer<ImmutableArray<IParameterSymbol>>
{
    internal class GroupByArgs
    {
        public IList<ITypeSymbol> Types { get; set; }
        public string PassString { get; set; }
        
        public string ArgsString { get; set; }
    }

    internal ITypeSymbol WorldType { get; }
        
    internal Dictionary<ImmutableArray<IParameterSymbol>, GroupByArgs> Groups { get; }

    internal void AddSystem(ITypeSymbol systemType, ImmutableArray<IParameterSymbol> parameters, string args, string argsPass)
    {
        if (!Groups.TryGetValue(parameters, out var groupByArgs))
            Groups[parameters] = groupByArgs = new GroupByArgs {Types = new List<ITypeSymbol>(), PassString = argsPass, ArgsString = args};
        
        groupByArgs.Types.Add(systemType);
    }
        
    public WorldInfo(ITypeSymbol worldType)
    {
        Groups = new Dictionary<ImmutableArray<IParameterSymbol>, GroupByArgs>(this);
        WorldType = worldType;
    }

    public bool Equals(ImmutableArray<IParameterSymbol> x, ImmutableArray<IParameterSymbol> y)
    {
        if (x.Length != y.Length)
            return false;

        for (int i = 0; i < x.Length; i++)
        {
            if (!AreParameterTypesEqual(x[i], y[i]))
                return false;
        }

        return true;
    }

    public int GetHashCode(ImmutableArray<IParameterSymbol> parameters)
    {
        unchecked
        {
            int hash = 17;

            foreach (var parameter in parameters)
            {
                hash = hash * 23 + (parameter.Type?.GetHashCode() ?? 0);
            }

            return hash;
        }
    }

    private bool AreParameterTypesEqual(IParameterSymbol parameter1, IParameterSymbol parameter2)
    {
        if (ReferenceEquals(parameter1, parameter2))
            return true;

        if (parameter1 is null || parameter2 is null)
            return false;

        // Compare only the types of the parameters
        return parameter1.Type.Equals(parameter2.Type, SymbolEqualityComparer.Default);
    }
}

internal class WorldsInfo
{
    internal Dictionary<ITypeSymbol, WorldInfo> Worlds { get; } = new(SymbolEqualityComparer.Default);

    public WorldInfo GetWorldInfo(ITypeSymbol type) => Worlds.TryGetValue(type, out var world)
        ? world
        : Worlds[type] = new WorldInfo(type);
}