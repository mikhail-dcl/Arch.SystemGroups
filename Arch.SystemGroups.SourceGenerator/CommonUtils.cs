using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Arch.SystemGroups.SourceGenerator;

public static class CommonUtils
{
    /// <summary>
    /// uses the logic of <see cref="TypedConstantExtensions.ToCSharpString"/>
    /// fixed representation of types that are not converted properly 
    /// </summary>
    /// <param name="constant"></param>
    /// <returns></returns>
    public static string ToCSharpStringSemanticallyValid(this TypedConstant constant)
    {
        if (constant.IsNull)
            return "null";

        var str = constant.ToCSharpString();
        
        // fix array
        if (constant.Kind == TypedConstantKind.Array)
            return "new []" + str;
        
        return constant.Type.SpecialType switch
        {
            SpecialType.System_Single => str + "F",
            SpecialType.System_Double => str + "D",
            SpecialType.System_Delegate => str + "M",
            _ => str
        };
    }
    
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
        if (IsPrimitiveAlias(typeSymbol))
            return typeSymbol.Name;

        if (IsGenericArgument(typeSymbol))
            return typeSymbol.ToString();

        return "global::" + typeSymbol;
    }
    
    // Check if a symbol denotes a generic argument
    public static bool IsGenericArgument(ISymbol symbol)
    {
        if (symbol.ContainingSymbol is INamedTypeSymbol namedTypeSymbol)
        {
            foreach (ITypeSymbol typeArgument in namedTypeSymbol.TypeArguments)
            {
                if (SymbolEqualityComparer.Default.Equals(typeArgument, symbol))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static bool IsGenericType(this ITypeSymbol typeSymbol) => 
        typeSymbol is INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: > 0 };

    public static string GetGenericArguments(ITypeSymbol typeSymbol)
    {
        if (!(typeSymbol is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol) || namedTypeSymbol.TypeArguments.Length == 0)
            return string.Empty;
        
        var genericArgumentsBuilder = new StringBuilder("<");

        for (int i = 0; i < namedTypeSymbol.TypeArguments.Length; i++)
        {
            ITypeSymbol typeArgument = namedTypeSymbol.TypeArguments[i];
            genericArgumentsBuilder.Append(typeArgument.Name);

            if (i < namedTypeSymbol.TypeArguments.Length - 1)
            {
                genericArgumentsBuilder.Append(", ");
            }
        }

        genericArgumentsBuilder.Append(">");

        return genericArgumentsBuilder.ToString();
    }
    
    public static string GetGenericConstraintsString(ITypeSymbol typeSymbol)
    {
        if (!(typeSymbol is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol) || namedTypeSymbol.TypeArguments.Length == 0)
            return string.Empty;

        var constraintsBuilder = new StringBuilder();

        foreach (ITypeParameterSymbol typeParameter in namedTypeSymbol.TypeParameters)
        {
            void AppendWhere()
            {
                constraintsBuilder.Append(" where ");
                constraintsBuilder.Append(typeParameter.Name);
                constraintsBuilder.Append(" : ");
            }

            var typeConstraintBuilder = new StringBuilder();
            
            void AppendComma()
            {
                if (typeConstraintBuilder.Length > 0)
                    typeConstraintBuilder.Append(", ");
            }

            if (typeParameter.HasUnmanagedTypeConstraint)
            {
                typeConstraintBuilder.Append("unmanaged");
            }
            else if (typeParameter.HasValueTypeConstraint)
            {
                typeConstraintBuilder.Append("struct");
            }
            else if (typeParameter.HasReferenceTypeConstraint)
            {
                typeConstraintBuilder.Append("class");
            }

            if (typeParameter.ConstraintTypes.Length > 0)
            {
                AppendComma();
                
                for (int i = 0; i < typeParameter.ConstraintTypes.Length; i++)
                {
                    ITypeSymbol constraintType = typeParameter.ConstraintTypes[i];
                    typeConstraintBuilder.Append(GetTypeReferenceInGlobalNotation(constraintType));

                    if (i < typeParameter.ConstraintTypes.Length - 1)
                    {
                        typeConstraintBuilder.Append(", ");
                    }
                }
            }

            if (typeParameter.HasConstructorConstraint)
            {
                AppendComma();
                typeConstraintBuilder.Append("new()");
            }
            
            if (typeConstraintBuilder.Length > 0)
            {
                AppendWhere();
                constraintsBuilder.Append(typeConstraintBuilder);
            }
        }

        return constraintsBuilder.ToString();
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

    public static IEnumerable<IMethodSymbol> GetConstructors(this ITypeSymbol typeSymbol)
        => typeSymbol.GetMembers().Where(m => m is IMethodSymbol { MethodKind: MethodKind.Constructor })
            .Cast<IMethodSymbol>();
}