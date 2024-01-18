// Copyright (c) 2024 citrus - https://unrealist.org
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EpicGames.UHT.Tables;
using EpicGames.UHT.Types;
using EpicGames.UHT.Utils;
using static Citrus.Plugins.SpecifierReferenceViewer.ReferenceGenerator.Tag;

namespace Citrus.Plugins.SpecifierReferenceViewer.ReferenceGenerator;

/// <summary>
/// Provides information about a valid specifier for one or more tags.
/// </summary>
internal sealed record KnownSpecifierInfo(string SpecifierName, Tag Tag, UhtSpecifierValueType ValueType);

/// <summary>
/// Provides a method for finding all supported specifiers using reflection.
/// </summary>
internal static class SpecifierFinder
{
    /// <summary>
    /// A list of assemblies to search for specifiers.
    /// </summary>
    private static readonly IReadOnlyList<Assembly> AssembliesToSearch = new[]
    {
        // The UHT itself is included.
        typeof(UhtType).Assembly
    };

    /// <summary>
    /// A map between a table name to one or more tags that can use the specifier.
    /// </summary>
    private static readonly IReadOnlyDictionary<string, Tag> TableNameToTag = new Dictionary<string, Tag>
    {
        [UhtTableNames.ClassBase] = UCLASS | UINTERFACE,
        [UhtTableNames.Class] = UCLASS,
        [UhtTableNames.Default] = UPROPERTY | UCLASS | USTRUCT | UENUM | UMETA | UPARAM | UINTERFACE | UDELEGATE | UFUNCTION,
        [UhtTableNames.Enum] = UENUM,
        [UhtTableNames.Field] = UCLASS | UENUM | UFUNCTION | USTRUCT | UDELEGATE | UINTERFACE,
        [UhtTableNames.Function] = UFUNCTION | UDELEGATE,
        [UhtTableNames.Interface] = UINTERFACE,
        [UhtTableNames.PropertyArgument] = UPARAM,
        [UhtTableNames.PropertyMember] = UPROPERTY,
        [UhtTableNames.ScriptStruct] = USTRUCT
    };

    /// <summary>
    /// Finds all supported specifiers.
    /// </summary>
    public static IEnumerable<KnownSpecifierInfo> FindAllSpecifiers()
    {
        Dictionary<string, KnownSpecifierInfo> dedupedSpecifiers = new();

        foreach (KnownSpecifierInfo specifier in FindAllSpecifiersInAssemblies())
        {
            if (dedupedSpecifiers.TryGetValue(specifier.SpecifierName, out KnownSpecifierInfo? existingSpecifierInfo))
            {
                dedupedSpecifiers[specifier.SpecifierName] =
                    existingSpecifierInfo with
                    {
                        Tag = specifier.Tag | existingSpecifierInfo.Tag
                    };
            }
            else
            {
                dedupedSpecifiers.Add(specifier.SpecifierName, specifier);
            }
        }

        return dedupedSpecifiers.Values;
    }

    /// <summary>
    /// Uses reflection against UHT to find all supported specifiers. This will produce duplicate specifiers for each
    /// supported tag.
    /// </summary>
    private static IEnumerable<KnownSpecifierInfo> FindAllSpecifiersInAssemblies()
    {
        const BindingFlags methodSearchFlags
                    = BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.DeclaredOnly
                    | BindingFlags.Instance
                    | BindingFlags.Static;

        foreach (Assembly assembly in AssembliesToSearch)
        {
            var specifiers = from type in assembly.GetTypes()
                             from method in type.GetMethods(methodSearchFlags)
                             from attribute in method.GetCustomAttributes<UhtSpecifierAttribute>()
                             select CreateSpecifierInfo(type, method, attribute);

            foreach (var specifier in specifiers)
            {
                if (specifier is not null)
                {
                    yield return specifier;
                }
            }
        }
    }

    /// <summary>
    /// Extracts information about a specifier from reflection data.
    /// </summary>
    private static KnownSpecifierInfo? CreateSpecifierInfo(
        Type @class, MethodInfo methodInfo, UhtSpecifierAttribute attribute)
    {
        // Use the explicit name of the specifier if set; otherwise, get it from the method name.
        string specifierName = UhtLookupTableBase.GetSuffixedName(@class, methodInfo, attribute.Name, "Specifier");

        if (!string.IsNullOrEmpty(attribute.Extends) && attribute.ValueType is not UhtSpecifierValueType.NotSet)
        {
            return new KnownSpecifierInfo(specifierName, TableNameToTag[attribute.Extends], attribute.ValueType);
        }

        return null;
    }
}
