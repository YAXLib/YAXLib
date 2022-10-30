// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses.Namespace;

[YAXSerializeAs("MobilePhone")]
public class CellPhoneCollectionNamespaceForAllItems
{
    public string? DeviceBrand { get; set; }
    public string? Os { get; set; }

    [YAXNamespace("app", "http://namespace.org/apps")]
    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement,
        EachElementName = "{http://namespace.org/appName}AppName")]
    public List<string> IntalledApps { get; set; } = new();

    [YAXNamespace("cls", "http://namespace.org/colorCol")]
    [YAXCollection(YAXCollectionSerializationTypes.Recursive,
        EachElementName = "{http://namespace.org/color}TheColor")]
    public List<string> AvailableColors { get; set; } = new();

    [YAXNamespace("mdls", "http://namespace.org/modelCol")]
    [YAXCollection(YAXCollectionSerializationTypes.Serially,
        EachElementName = "{http://namespace.org/color}TheModel", // should be ignored
        IsWhiteSpaceSeparator = false, SeparateBy = ",")]
    public List<string> AvailableModels { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static CellPhoneCollectionNamespaceForAllItems GetSampleInstance()
    {
        return new CellPhoneCollectionNamespaceForAllItems {
            DeviceBrand = "Samsung Galaxy Nexus",
            Os = "Android",
            IntalledApps = new List<string> { "Google Map", "Google+", "Google Play" },
            AvailableColors = new List<string> { "red", "black", "white" },
            AvailableModels = new List<string> { "S1", "MII", "SXi", "NoneSense" }
        };
    }
}