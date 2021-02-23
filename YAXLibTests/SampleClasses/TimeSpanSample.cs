﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [ShowInDemoApplication]
    [YAXComment("This example shows serialization and deserialization of TimeSpan obejcts")]
    public class TimeSpanSample
    {
        public TimeSpan TheTimeSpan { get; set; }
        public TimeSpan AnotherTimeSpan { get; set; }

        public Dictionary<TimeSpan, int> DicTimeSpans { get; set; }

        public override string ToString()
        {
            return GeneralToStringProvider.GeneralToString(this);
        }

        public static TimeSpanSample GetSampleInstance()
        {
            var dic = new Dictionary<TimeSpan, int>();
            dic.Add(new TimeSpan(2, 3, 45, 2, 300), 1);
            dic.Add(new TimeSpan(3, 1, 40, 1, 200), 2);

            return new TimeSpanSample
            {
                TheTimeSpan = new TimeSpan(2, 3, 45, 2, 300),
                AnotherTimeSpan = new TimeSpan(1863023000000),
                DicTimeSpans = dic
            };
        }
    }
}