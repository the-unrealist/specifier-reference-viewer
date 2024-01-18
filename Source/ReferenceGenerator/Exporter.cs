// Copyright (c) 2024 citrus - https://unrealist.org
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using EpicGames.Core;
using EpicGames.UHT.Tables;
using EpicGames.UHT.Utils;

namespace Citrus.Plugins.SpecifierReferenceViewer.ReferenceGenerator;

/// <summary>
/// Contains a UHT exporter that generates a reference list containing every specifier and metadata used in the engine
/// and all modules.
/// </summary>
[UnrealHeaderTool]
public static class Exporter
{
    /// <summary>
    /// The name of the plugin that contains this tool and the viewer module.
    /// </summary>
    private const string PluginName = "SpecifierReferenceViewer";

    /// <summary>
    /// The name of the Unreal module in the plugin. The generated files will appear under this module's Intermediate
    /// directory. This allows the viewer to access the generated reference list.
    /// </summary>
    private const string ViewerModuleName = "SpecifierReferenceViewer";

    /// <summary>
    /// The name of this UHT exporter.
    /// </summary>
    private const string ExporterName = "SpecifierReferenceGenerator";

    /// <summary>
    /// Exports a reference list of specifiers and metadata used in the source code.
    /// </summary>
    [UhtExporter(Name = ExporterName, ModuleName = ViewerModuleName, Options = UhtExporterOptions.Default)]
    public static void GenerateReferenceList(IUhtExportFactory factory)
    {
        if (!factory.Session.IsPluginEnabled(PluginName, includeTargetCheck: true))
        {
            return;
        }

        try
        {
            factory.Session.LogInfo("The specifier reference list is being generated.");

            var knownSpecifiers = SpecifierFinder.FindAllSpecifiers();
            var metadataUsages = MetadataUsageFinder.FindAllMetadataUsages(factory.Session.Packages);
            var knownMetadata = MetadataFinder.FindAllMetadata(metadataUsages);

            ExportMetadataUsagesAsCodeForViewer(factory, metadataUsages);
            ExportKnownListsAsCodeForViewer(factory, knownSpecifiers, knownMetadata);

            if (factory.PluginModule!.TryGetDefine("SPECIFIER_EXPORT_JSON", out int exportJson) && exportJson == 1)
            {
                ExportKnownListsAsJson(factory, knownSpecifiers, knownMetadata);
            }

            factory.Session.LogInfo("Finished generating the specifier reference list.");
        }
        catch (Exception ex)
        {
            factory.Session.LogError(ex.ToString());
            throw;
        }
    }

    /// <summary>
    /// Exports a <c>metadata_usages.inc</c> file that contains all metadata usages.
    /// </summary>
    private static void ExportMetadataUsagesAsCodeForViewer(IUhtExportFactory factory,
        IEnumerable<MetadataUsageInfo> metadataUsages)
    {
        using BorrowStringBuilder borrower = new(StringBuilderCache.Big);

        // TODO: Generate cpp code.
        factory.Session.LogWarning($"{nameof(ExportMetadataUsagesAsCodeForViewer)} is not implemented.");

        Export(factory, "references.inc", borrower.StringBuilder.ToString());
    }

    /// <summary>
    /// Exports a <c>reference_lists.inc</c> file that contains all specifiers.
    /// </summary>
    private static void ExportKnownListsAsCodeForViewer(IUhtExportFactory factory,
        IEnumerable<KnownSpecifierInfo> knownSpecifiers, IEnumerable<KnownMetadataInfo> knownMetadata)
    {
        using BorrowStringBuilder borrower = new(StringBuilderCache.Big);

        // TODO: Generate cpp code.
        factory.Session.LogWarning($"{nameof(ExportKnownListsAsCodeForViewer)} is not implemented.");

        Export(factory, "specifiers.inc", borrower.StringBuilder.ToString());
    }

    /// <summary>
    /// Exports all known specifier and metadata as a JSON file for external use.
    /// </summary>
    private static void ExportKnownListsAsJson(IUhtExportFactory factory,
        IEnumerable<KnownSpecifierInfo> knownSpecifiers, IEnumerable<KnownMetadataInfo> knownMetadata)
    {
        using BorrowStringBuilder borrower = new(StringBuilderCache.Big);

        borrower.StringBuilder.Append(JsonSerializer.Serialize(
            new
            {
                Specifiers = knownSpecifiers,
                Metadata = knownMetadata
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            }));

        Export(factory, "specifiers.json", borrower.StringBuilder.ToString());
    }

    /// <summary>
    /// Exports a file to the Intermediate directory of the plugin.
    /// </summary>
    private static void Export(IUhtExportFactory factory, string relativePath, StringView contents)
    {
        string fullPath = Path.Combine(factory.PluginModule!.OutputDirectory, relativePath);
        factory.CommitOutput(fullPath, contents);
        factory.Session.LogInfo($"Exported file {fullPath}");
    }
}
