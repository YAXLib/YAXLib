// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

public interface ISample
{
    int IntInInterface { get; set; }
}

public class Class1 : ISample
{
    public int IntInInterface { get; set; }
    public double DoubleInClass1 { get; set; }
}

public class Class2 : ISample
{
    public int IntInInterface { get; set; }
    public string? StringInClass2 { get; set; }
}

public class Class31 : Class1
{
    public string? StringInClass31 { get; set; }
}

[ShowInDemoApplication]
[YAXComment("""
    This example shows serialization and deserialization of
    objects through a reference to their base class or interface
    """)]
public class CollectionOfInterfacesSample
{
    public ISample? SingleRef { get; set; }
    public List<ISample> ListOfSamples { get; set; } = new();
    public Dictionary<ISample, int> DictSample2Int { get; set; } = new();
    public Dictionary<int, ISample> DictInt2Sample { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static CollectionOfInterfacesSample GetSampleInstance()
    {
        var c1 = new Class1 { IntInInterface = 1, DoubleInClass1 = 1.0 };
        var c2 = new Class2 { IntInInterface = 2, StringInClass2 = "Class2" };
        var c3 = new Class31 { DoubleInClass1 = 3.0, IntInInterface = 3, StringInClass31 = "Class3_1" };

        var lstOfSamples = new List<ISample> {
            c1,
            c2,
            c3
        };

        var dicSample2Int = new Dictionary<ISample, int> {
            { c1, 1 },
            { c2, 2 },
            { c3, 3 }
        };

        var dicInt2Sample = new Dictionary<int, ISample> {
            { 1, c1 },
            { 2, c2 },
            { 3, c3 }
        };


        return new CollectionOfInterfacesSample {
            SingleRef = new Class2 { IntInInterface = 22, StringInClass2 = "SingleRef" },
            ListOfSamples = lstOfSamples,
            DictSample2Int = dicSample2Int,
            DictInt2Sample = dicInt2Sample
        };
    }
}
