// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses.Namespace;

public class CellPhoneDictionary
{
    [YAXSerializeAs("TheName")]
    public string? DeviceBrand { get; set; }

    public string? Os { get; set; }

    public Dictionary<string, double> Prices { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static CellPhoneDictionary GetSampleInstance()
    {
        var prices = new Dictionary<string, double> { {"red", 120 }, { "blue", 110 }, { "black", 140 } };

        return new CellPhoneDictionary {
            DeviceBrand = "HTC",
            Os = "Windows Phone 8",
            Prices = prices
        };
    }
}