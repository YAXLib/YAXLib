// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses.Namespace;

[YAXNamespace("http://namespace.org/nsmain")]
public class CellPhoneDictionaryNamespace
{
    [YAXSerializeAs("TheName")]
    [YAXNamespace("x1", "http://namespace.org/x1")]
    public string? DeviceBrand { get; set; }

    public string? Os { get; set; }

    [YAXNamespace("p1", "namespace/for/prices/only")]
    public Dictionary<string, double> Prices { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static CellPhoneDictionaryNamespace GetSampleInstance()
    {
        var prices = new Dictionary<string, double> { { "red", 120 }, { "blue", 110 }, { "black", 140 } };

        return new CellPhoneDictionaryNamespace {
            DeviceBrand = "HTC",
            Os = "Windows Phone 8",
            Prices = prices
        };
    }
}