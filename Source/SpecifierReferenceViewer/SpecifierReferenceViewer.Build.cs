// Copyright (c) 2024 citrus - https://unrealist.org
// Licensed under the MIT License.

using UnrealBuildTool;

public class SpecifierReferenceViewer : ModuleRules
{
    public SpecifierReferenceViewer(ReadOnlyTargetRules Target) : base(Target)
    {
        PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;

        PrivateDependencyModuleNames.AddRange(
            new[]
            {
                "CoreUObject"
            });

        PublicDefinitions.Add("SPECIFIER_EXPORT_JSON=1");
    }
}
