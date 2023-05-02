using Microsoft.CodeAnalysis;

namespace Arch.SystemGroups.SourceGenerator;

public class CommonUtils
{
    /// <summary>
    ///     Convert a <see cref="RefKind"/> to its code string equivalent.
    /// </summary>
    /// <param name="refKind">The <see cref="RefKind"/>.</param>
    /// <returns>The code string equivalent.</returns>
    public static string RefKindToString(RefKind refKind)
    {
        switch (refKind)
        {
            case RefKind.None:
                return "";
            case RefKind.Ref:
                return "ref";
            case RefKind.In:
                return "in";
            case RefKind.Out:
                return "out";
        }
        return null;
    }

    public static string GetTypeReferenceInGlobalNotation(ITypeSymbol typeSymbol)
    {
        return IsPrimitiveAlias(typeSymbol) ? typeSymbol.Name : "global::" + typeSymbol;
    }
    
    public static bool IsPrimitiveAlias(ITypeSymbol typeSymbol)
    {
        // Check if the type is a primitive type or an alias to a primitive type
        if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol.IsGenericType)
            {
                namedTypeSymbol = namedTypeSymbol.ConstructedFrom;
            }
            return namedTypeSymbol.SpecialType switch
            {
                SpecialType.System_Boolean => true,
                SpecialType.System_Byte => true,
                SpecialType.System_Char => true,
                SpecialType.System_Decimal => true,
                SpecialType.System_Double => true,
                SpecialType.System_Int16 => true,
                SpecialType.System_Int32 => true,
                SpecialType.System_Int64 => true,
                SpecialType.System_Object => true,
                SpecialType.System_SByte => true,
                SpecialType.System_Single => true,
                SpecialType.System_String => true,
                SpecialType.System_UInt16 => true,
                SpecialType.System_UInt32 => true,
                SpecialType.System_UInt64 => true,
                _ => false,
            };
        }
        return false;
    }
}