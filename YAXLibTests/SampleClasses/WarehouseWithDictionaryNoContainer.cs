// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
[YAXComment("""
    This example shows how dictionary objects can be serialized without
    their enclosing element
    """)]
public class WarehouseWithDictionaryNoContainer
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

    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement)]
    [YAXDictionary(EachPairName = "ItemInfo", KeyName = "Item", ValueName = "Count",
        SerializeKeyAs = YAXNodeTypes.Attribute,
        SerializeValueAs = YAXNodeTypes.Attribute)]
    [YAXSerializeAs("ItemQuantities")]
    public Dictionary<PossibleItems, int> ItemQuantitiesDic { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static WarehouseWithDictionaryNoContainer GetSampleInstance()
    {
        var dicItems = new Dictionary<PossibleItems, int> {
            { PossibleItems.Item3, 10 },
            { PossibleItems.Item6, 120 },
            { PossibleItems.Item9, 600 },
            { PossibleItems.Item12, 25 }
        };

        var w = new WarehouseWithDictionaryNoContainer {
            Name = "Foo Warehousing Ltd.",
            Address = "No. 10, Some Ave., Some City, Some Country",
            Area = 120000.50, // square meters
            Items = new[] { PossibleItems.Item3, PossibleItems.Item6, PossibleItems.Item9, PossibleItems.Item12 },
            ItemQuantitiesDic = dicItems
        };

        return w;
    }
}
