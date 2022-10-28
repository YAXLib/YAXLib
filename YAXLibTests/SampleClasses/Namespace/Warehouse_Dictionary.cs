// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses.Namespace;

[YAXNamespace("http://www.mywarehouse.com/warehouse/def/v3")]
public class WarehouseDictionary
{
    [YAXDictionary(EachPairName = "ItemInfo", KeyName = "Item", ValueName = "Count",
        SerializeKeyAs = YAXNodeTypes.Attribute,
        SerializeValueAs = YAXNodeTypes.Attribute)]
    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement)]
    [YAXSerializeAs("ItemQuantities")]
    public Dictionary<string, int> ItemQuantitiesDic { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static WarehouseDictionary GetSampleInstance()
    {
        return new WarehouseDictionary {
            ItemQuantitiesDic = new Dictionary<string, int> { { "Item1", 10 }, { "Item4", 30 }, { "Item2", 20 } }
        };
    }
}