# Specifier Reference Viewer
This is a [UBT plugin](https://unrealist.org/uht-plugins) that generates a list of all `UPROPERTY`, `UFUNCTION`, `USTRUCT`, `UENUM`, `UMETA`, `UPARAM`, `UINTERFACE`, and `UDELEGATE` specifiers based on their usage in the source for the engine, game, and all plugins.

For your convenience, I've included the generated output in this repo as [`specifiers.json`](https://github.com/the-unrealist/specifier-reference-viewer/blob/main/specifiers.json). This file contains every specifier in use as of Unreal Engine 5.3.

At this time, it only exports to a JSON file. At some point in the future, I plan to integrate the result into the Unreal Editor in various ways.

-----
After installing the plugin, enable the plugin only for the `Editor` target in your `.uproject`.

```json
{
    "Plugins": [
        {
            "Name": "SpecifierReferenceViewer",
            "Enabled": true,
            "TargetAllowList": [
                "Editor"
            ]
        }
    ]
}
```
-----

This exporter performs two separate tasks:
1. Finds all non-metadata specifiers by using reflection against the UHT.
2. Finds all metadata specifier usages by scanning each `UObject` parsed by the UHT, and then analyzes this usage data to determine the value type of each metadata specifier.
