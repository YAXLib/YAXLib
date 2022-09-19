// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

internal class ClassWithDuplicateYaxAttribute
{
    [YAXAttributeForClass]
    [YAXSerializeAs("test")]
    public string? Test1 { get; set; }

    [YAXAttributeForClass]
    [YAXSerializeAs("test")]
    public string? Test2 { get; set; }

    public static ClassWithDuplicateYaxAttribute GetSampleInstance()
    {
        return new ClassWithDuplicateYaxAttribute {
            Test1 = "test1",
            Test2 = "test2"
        };
    }
}