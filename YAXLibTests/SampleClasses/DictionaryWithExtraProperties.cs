// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[YAXDictionary(EachPairName = "Pair")]
public class DictionaryWithExtraProperties : Dictionary<int, string>
{
    public string? Prop1 { get; set; }
    public double Prop2 { get; set; }

    public static DictionaryWithExtraProperties GetSampleInstance()
    {
        var inst = new DictionaryWithExtraProperties {
            Prop1 = "Prop1",
            Prop2 = 2.234
        };
        inst.Add(1, "One");
        inst.Add(2, "Two");
        inst.Add(3, "Three");

        return inst;
    }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this)
               + string.Format("Prop1: {0}, Prop2: {1}", Prop1, Prop2);
    }
}