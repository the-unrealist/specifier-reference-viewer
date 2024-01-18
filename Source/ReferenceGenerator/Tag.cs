// Copyright (c) 2024 citrus - https://unrealist.org
// Licensed under the MIT License.

using System;

namespace Citrus.Plugins.SpecifierReferenceViewer.ReferenceGenerator;

/// <summary>
/// Indicates the tag that contains a specifier or metadata.
/// </summary>
[Flags]
public enum Tag
{
    UPROPERTY = 1,
    UCLASS = 2,
    USTRUCT = 4,
    UENUM = 8,
    UMETA = 16,
    UPARAM = 32,
    UINTERFACE = 64,
    UDELEGATE = 128,
    UFUNCTION = 256,
}
