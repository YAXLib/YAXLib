// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses.Namespace;

public class CellPhoneDictionaryNamespaceForAllItems
{
    [YAXSerializeAs("{http://namespace.org/brand}Brand")]
    public string? DeviceBrand { get; set; }

    public string? Os { get; set; }

    [YAXSerializeAs("{http://namespace.org/prices}ThePrices")]
    [YAXDictionary(EachPairName = "{http://namespace.org/pricepair}PricePair",
        KeyName = "{http://namespace.org/color}TheColor",
        ValueName = "{http://namespace.org/pricevalue}ThePrice")]
    public Dictionary<string, double> Prices { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static CellPhoneDictionaryNamespaceForAllItems GetSampleInstance()
    {
        var prices = new Dictionary<string, double> { { "red", 120 }, { "blue", 110 }, { "black", 140 } };
        return new CellPhoneDictionaryNamespaceForAllItems {
            DeviceBrand = "Samsung Galaxy Nexus",
            Os = "Android",
            Prices = prices
        };
    }
}