// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
[YAXComment("This example shows the serialization of Dictionary")]
public class WarehouseWithDictionary
{
    [YAXAttributeForClass] public string? Name { get; set; }

    [YAXSerializeAs("address")]
    [YAXAttributeFor("SiteInfo")]
    public string? Address { get; set; }

    [YAXSerializeAs("SurfaceArea")]
    [YAXElementFor("SiteInfo")]
    public double Area { get; set; }

    [YAXCollection(YAXCollectionSerializationTypes.Serially, SeparateBy = ", ")]
    [YAXSerializeAs("StoreableItems")]
    public PossibleItems[]? Items { get; set; }

    [YAXDictionary(EachPairName = "ItemInfo", KeyName = "Item", ValueName = "Count",
        SerializeKeyAs = YAXNodeTypes.Attribute,
        SerializeValueAs = YAXNodeTypes.Attribute)]
    [YAXSerializeAs("ItemQuantities")]
    public Dictionary<PossibleItems, int> ItemQuantitiesDic { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static WarehouseWithDictionary GetSampleInstance()
    {
        var dicItems = new Dictionary<PossibleItems, int>();
        dicItems.Add(PossibleItems.Item3, 10);
        dicItems.Add(PossibleItems.Item6, 120);
        dicItems.Add(PossibleItems.Item9, 600);
        dicItems.Add(PossibleItems.Item12, 25);

        var w = new WarehouseWithDictionary {
            Name = "Foo Warehousing Ltd.",
            Address = "No. 10, Some Ave., Some City, Some Country",
            Area = 120000.50, // square meters
            Items = new[] { PossibleItems.Item3, PossibleItems.Item6, PossibleItems.Item9, PossibleItems.Item12 },
            ItemQuantitiesDic = dicItems
        };

        return w;
    }
}