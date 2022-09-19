// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[YAXPreserveWhitespace]
public class PreserveWhitespaceOnFieldsSample
{
    public string? Str1 { get; set; }

    public string? Str2 { get; set; }

    [YAXValueFor("SomeElem")] public string? Str3 { get; set; }

    public string[]? Strings { get; set; }

    public Dictionary<string, int>? StringDic { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static PreserveWhitespaceOnFieldsSample GetSampleInstance()
    {
        return new PreserveWhitespaceOnFieldsSample {
            Str1 = "       ",
            Str2 = "         ",
            Str3 = "         ",
            Strings = new[] { "abc", "     ", "efg" },
            StringDic = new Dictionary<string, int> { { "abc", 1 }, { "    ", 2 }, { "efg", 3 } }
        };
    }
}