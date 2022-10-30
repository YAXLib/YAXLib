// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
[YAXComment("This example shows how to apply format strings to a class properties")]
public class FormattingExample
{
    [YAXFormat("D")] public DateTime CreationDate { get; set; }

    [YAXFormat("d")] public DateTime ModificationDate { get; set; }

    [YAXFormat("F05")] public double Pi { get; set; }

    [YAXFormat("F03")] public List<double> NaturalExp { get; set; } = new();

    [YAXDictionary(KeyFormatString = "F02", ValueFormatString = "F05")]
    public Dictionary<double, double> SomeLogarithmExample { get; set; } = new();


    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static FormattingExample GetSampleInstance()
    {
        var lstNe = new List<double>();
        for (var i = 1; i <= 4; ++i)
            lstNe.Add(Math.Exp(i));

        var dicLog = new Dictionary<double, double>();
        for (var d = 1.5; d <= 10; d *= 2)
            dicLog.Add(d, Math.Log(d));

        return new FormattingExample {
            CreationDate = new DateTime(2007, 3, 14),
            ModificationDate = new DateTime(2007, 3, 18),
            Pi = Math.PI,
            NaturalExp = lstNe,
            SomeLogarithmExample = dicLog
        };
    }
}