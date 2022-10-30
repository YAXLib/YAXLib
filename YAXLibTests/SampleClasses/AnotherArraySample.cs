// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
[YAXComment("This example shows usage of jagged multi-dimensional arrays")]
public class AnotherArraySample
{
    public int[][,]? Array1 { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static AnotherArraySample GetSampleInstance()
    {
        var ar0 = new int[2, 3];
        for (var i = 0; i < ar0.GetLength(0); i++)
        for (var j = 0; j < ar0.GetLength(1); j++)
            ar0[i, j] = i * j + 1;

        var ar1 = new int[3, 2];
        for (var i = 0; i < ar1.GetLength(0); i++)
        for (var j = 0; j < ar1.GetLength(1); j++)
            ar1[i, j] = i * j + 3;

        var ar00 = new int[2][,];
        ar00[0] = ar0;
        ar00[1] = ar1;

        return new AnotherArraySample { Array1 = ar00 };
    }
}