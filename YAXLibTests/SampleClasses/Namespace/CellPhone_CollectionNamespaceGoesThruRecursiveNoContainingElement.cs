// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses.Namespace;

[YAXSerializeAs("MobilePhone")]
public class CellPhoneCollectionNamespaceGoesThruRecursiveNoContainingElement
{
    public string? DeviceBrand { get; set; }
    public string? Os { get; set; }

    [YAXNamespace("app", "http://namespace.org/apps")]
    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement)]
    public List<string> IntalledApps { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static CellPhoneCollectionNamespaceGoesThruRecursiveNoContainingElement GetSampleInstance()
    {
        return new CellPhoneCollectionNamespaceGoesThruRecursiveNoContainingElement {
            DeviceBrand = "Samsung Galaxy Nexus",
            Os = "Android",
            IntalledApps = new List<string> { "Google Map", "Google+", "Google Play" }
        };
    }
}