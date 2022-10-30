// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
[YAXComment("This example shows serialization and deserialization of TimeSpan objects")]
public class TimeSpanSample
{
    public TimeSpan TheTimeSpan { get; set; }
    public TimeSpan AnotherTimeSpan { get; set; }

    public Dictionary<TimeSpan, int>? DicTimeSpans { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static TimeSpanSample GetSampleInstance()
    {
        var dic = new Dictionary<TimeSpan, int> {
            { new TimeSpan(2, 3, 45, 2, 300), 1 },
            { new TimeSpan(3, 1, 40, 1, 200), 2 }
        };

        return new TimeSpanSample {
            TheTimeSpan = new TimeSpan(2, 3, 45, 2, 300),
            AnotherTimeSpan = new TimeSpan(1863023000000),
            DicTimeSpans = dic
        };
    }
}