// Copyright (c) 2024 citrus - https://unrealist.org
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Citrus.Plugins.SpecifierReferenceViewer.ReferenceGenerator;

/// <summary>
/// Provides information about a metadata specifier for one or more tags.
/// </summary>
internal sealed record KnownMetadataInfo(string MetadataName, Tag Tag, MetadataValueType ValueType);

/// <summary>
/// Indicates the value a metadata should have.
/// </summary>
enum MetadataValueType
{
    /// <summary>
    /// The metadata should have no value.
    /// </summary>
    Flag = 1,

    /// <summary>
    /// The metadata should have a boolean value.
    /// </summary>
    Bool,

    /// <summary>
    /// The metadata should have either a boolean value or no value.
    /// </summary>
    FlagOrBool,

    /// <summary>
    /// The metadata should have a string (or arbitrary) value.
    /// </summary>
    String
}

/// <summary>
/// Provides a method for generating a list of all unique metadata specifiers from metadata usage data.
/// </summary>
internal static class MetadataFinder
{
    /// <summary>
    /// Finds all unique metadata specifiers from the provided metadata usage data.
    /// </summary>
    public static IEnumerable<KnownMetadataInfo> FindAllMetadata(
        IEnumerable<MetadataUsageInfo> metadataUsages)
    {
        Dictionary<string, KnownMetadataInfo> dedupedMetadata = new(StringComparer.OrdinalIgnoreCase);

        foreach (MetadataUsageInfo usageInfo in metadataUsages)
        {
            if (dedupedMetadata.TryGetValue(usageInfo.Key, out KnownMetadataInfo? existingMetadata))
            {
                dedupedMetadata[usageInfo.Key] =
                    existingMetadata with
                    {
                        Tag = usageInfo.Tag | existingMetadata.Tag,
                        ValueType = CombineValueTypes(DetermineValueType(usageInfo.Value), existingMetadata.ValueType)
                    };
            }
            else
            {
                dedupedMetadata.Add(usageInfo.Key,
                    new KnownMetadataInfo(usageInfo.Key, usageInfo.Tag, DetermineValueType(usageInfo.Value)));
            }
        }

        return dedupedMetadata.Values;
    }

    /// <summary>
    /// Deduces the value type of a metadata specifier from the provided metadata value.
    /// </summary>
    private static MetadataValueType DetermineValueType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return MetadataValueType.Flag;
        }

        if (bool.TryParse(value, out _))
        {
            return MetadataValueType.Bool;
        }


        return MetadataValueType.String;
    }

    /// <summary>
    /// Deduces a new value type by merging two value types.
    /// </summary>
    private static MetadataValueType CombineValueTypes(MetadataValueType left, MetadataValueType right)
    {
        if (left == right)
        {
            return left;
        }

        static bool IsFlagOrBool(MetadataValueType value)
            => value == MetadataValueType.Flag
            || value == MetadataValueType.Bool
            || value == MetadataValueType.FlagOrBool;

        if (IsFlagOrBool(left) && IsFlagOrBool(right))
        {
            return MetadataValueType.FlagOrBool;
        }

        return MetadataValueType.String;
    }
}
