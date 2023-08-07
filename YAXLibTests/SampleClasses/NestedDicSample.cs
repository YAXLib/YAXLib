// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Linq;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
[YAXComment("""
    This example demonstrates usage of recursive collection serialization
    and deserialization. In this case a Dictionary whose Key, or Value is 
    another dictionary or collection has been used.
    """)]
public class NestedDicSample
{
    public Dictionary<Dictionary<double, Dictionary<int, int>>, Dictionary<Dictionary<string, string>, List<double>>
    > SomeDic { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static NestedDicSample GetSampleInstance()
    {
        var dicKv1 = new Dictionary<int, int> {
            { 1, 1 },
            { 2, 2 },
            { 3, 3 },
            { 4, 4 }
        };

        var dicKv2 = new Dictionary<int, int> {
            { 9, 1 },
            { 8, 2 }
        };

        var dicVk1 = new Dictionary<string, string> {
            { "Test", "123" },
            { "Test2", "456" }
        };

        var dicVk2 = new Dictionary<string, string> {
            { "Num1", "123" },
            { "Num2", "456" }
        };

        var dicK = new Dictionary<double, Dictionary<int, int>> {
            { 0.99999, dicKv1 },
            { 3.14, dicKv2 }
        };

        var dicV = new Dictionary<Dictionary<string, string>, List<double>> {
            { dicVk1, new[] { 0.98767, 232, 13.124 }.ToList() },
            { dicVk2, new[] { 9.8767, 23.2, 1.34 }.ToList() }
        };

        var mainDic =
            new Dictionary<Dictionary<double, Dictionary<int, int>>,
                Dictionary<Dictionary<string, string>, List<double>>> { { dicK, dicV } };

        return new NestedDicSample {
            SomeDic = mainDic
        };
    }
}
