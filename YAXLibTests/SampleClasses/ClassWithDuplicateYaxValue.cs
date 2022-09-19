// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

internal class ClassWithDuplicateYaxValue
{
    [YAXSerializableField]
    [YAXValueForClass]
    public string? Value1 { get; set; }

    [YAXSerializableField]
    [YAXValueForClass]
    public string? Value2 { get; set; }

    public static ClassWithDuplicateYaxValue GetSampleInstance()
    {
        return new ClassWithDuplicateYaxValue {
            Value1 = "lorum ipsum",
            Value2 = "lorum oopsum"
        };
    }
}

internal class ClassWithInvalidFormat
{
    [YAXSerializableField]
    [YAXFormat("fancyFormat")]
    public int Value1 { get; set; }

    public static ClassWithInvalidFormat GetSampleInstance()
    {
        return new ClassWithInvalidFormat {
            Value1 = 500
        };
    }
}