using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Arch.SystemGroups.SourceGenerator;

public static class AttributesUtils
{
    public readonly struct ActionOnAttribute
    {
        public readonly Action<AttributeData> Action;
        public readonly Predicate<AttributeData> Predicate;
        public readonly bool Recursively;

        public ActionOnAttribute(Action<AttributeData> action, Predicate<AttributeData> predicate, bool recursively)
        {
            Action = action;
            Recursively = recursively;
            Predicate = predicate;
        }

        public static implicit operator ActionOnAttribute((Action<AttributeData> action, Predicate<AttributeData> predicate, bool recursively) tuple)
        {
            return new ActionOnAttribute(tuple.action, tuple.predicate, tuple.recursively);
        }
    }

    public static void DoOnAttributes(ITypeSymbol type, params ActionOnAttribute[] actions)
    {
        static void DoOnAttributesInternal(ITypeSymbol currentType, ActionOnAttribute[] actions, ref int resolvedMask)
        {
            var attributes = currentType.GetAttributes();
            
            for (var index = 0; index < actions.Length; index++)
            {
                // already resolved
                if ((resolvedMask & (1 << index)) > 0)
                    continue;
                
                var action = actions[index];

                foreach (var attribute in attributes)
                {
                    if (action.Predicate(attribute))
                    {
                        action.Action(attribute);
                        resolvedMask |= 1 << index;
                        break;
                    }
                }
                
                if (!action.Recursively)
                    resolvedMask |= 1 << index;
            }
            
            // if there are unresolved attributes call onto the base type
            if (resolvedMask != (~0 >> (sizeof(int) * 8 - actions.Length)) && currentType.BaseType != null)
            {
                DoOnAttributesInternal(currentType.BaseType, actions, ref resolvedMask);
            }
        }

        var flags = 0;
        DoOnAttributesInternal(type, actions, ref flags);
    }
}