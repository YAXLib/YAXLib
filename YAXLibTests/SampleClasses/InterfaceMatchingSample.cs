// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

[YAXComment("""
    This example shows serialization and deserialization of objects
    through a reference to their base class or interface while used in 
    collection classes
    """)]
public class InterfaceMatchingSample
{
    [YAXAttributeForClass] public int? SomeNumber { get; set; }

    [YAXCollection(YAXCollectionSerializationTypes.Serially)]
    public List<int?> ListOfSamples { get; set; } = new();

    [YAXDictionary(SerializeKeyAs = YAXNodeTypes.Attribute, SerializeValueAs = YAXNodeTypes.Attribute)]
    public Dictionary<int, double?> DictInt2Nullable { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static InterfaceMatchingSample GetSampleInstance()
    {
        var lstOfSamples = new List<int?> {
            2,
            4,
            8
        };

        var dicInt2Sample = new Dictionary<int, double?> {
            { 1, 1.0 },
            { 2, 2.0 },
            { 3, null }
        };


        return new InterfaceMatchingSample {
            SomeNumber = 10,
            ListOfSamples = lstOfSamples,
            DictInt2Nullable = dicInt2Sample
        };
    }
}
