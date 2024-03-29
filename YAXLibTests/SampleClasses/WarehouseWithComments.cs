﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
public class WarehouseWithComments
{
    [YAXComment("Comment for name")]
    [YAXElementFor("foo/bar/one/two")]
    public string? Name { get; set; }

    [YAXComment("Comment for OwnerName")]
    [YAXElementFor("foo/bar/one")]
    public string? OwnerName { get; set; }

    [YAXComment("This will not be shown, because it is an attribute")]
    [YAXSerializeAs("address")]
    [YAXAttributeFor("SiteInfo")]
    public string? Address { get; set; }

    [YAXSerializeAs("SurfaceArea")]
    [YAXElementFor("SiteInfo")]
    public double Area { get; set; }

    [YAXCollection(YAXCollectionSerializationTypes.Serially, SeparateBy = ", ")]
    [YAXSerializeAs("StoreableItems")]
    public PossibleItems[]? Items { get; set; }

    [YAXComment("This dictionary is serilaized without container")]
    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement)]
    [YAXDictionary(EachPairName = "ItemInfo", KeyName = "Item", ValueName = "Count",
        SerializeKeyAs = YAXNodeTypes.Attribute,
        SerializeValueAs = YAXNodeTypes.Attribute)]
    [YAXSerializeAs("ItemQuantities")]
    public Dictionary<PossibleItems, int>? ItemQuantitiesDic { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static WarehouseWithComments GetSampleInstance()
    {
        var dicItems = new Dictionary<PossibleItems, int> {
            { PossibleItems.Item3, 10 },
            { PossibleItems.Item6, 120 },
            { PossibleItems.Item9, 600 },
            { PossibleItems.Item12, 25 }
        };

        var w = new WarehouseWithComments {
            Name = "Foo Warehousing Ltd.",
            OwnerName = "John Doe",
            Address = "No. 10, Some Ave., Some City, Some Country",
            Area = 120000.50, // square meters
            Items = new[] { PossibleItems.Item3, PossibleItems.Item6, PossibleItems.Item9, PossibleItems.Item12 },
            ItemQuantitiesDic = dicItems
        };

        return w;
    }
}