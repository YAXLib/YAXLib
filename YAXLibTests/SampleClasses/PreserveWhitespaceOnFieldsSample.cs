// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

public class PreserveWhitespaceOnClassSample
{
    [YAXPreserveWhitespace] public string? Str1 { get; set; }

    [YAXPreserveWhitespace] public string? Str2 { get; set; }

    [YAXPreserveWhitespace]
    [YAXValueFor("SomeElem")]
    public string? Str3 { get; set; }

    [YAXPreserveWhitespace] public string[]? Strings { get; set; }

    [YAXPreserveWhitespace] public Dictionary<string, int>? StringDic { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static PreserveWhitespaceOnClassSample GetSampleInstance()
    {
        return new PreserveWhitespaceOnClassSample {
            Str1 = "       ",
            Str2 = "         ",
            Str3 = "         ",
            Strings = new[] { "abc", "     ", "efg" },
            StringDic = new Dictionary<string, int> { { "abc", 1 }, { "    ", 2 }, { "efg", 3 } }
        };
    }
}