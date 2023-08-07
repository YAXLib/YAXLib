// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
[YAXComment("How multi-line comments are serialized as multiple XML comments")]
public class MultipleCommentsTest
{
    [YAXComment("""
        Using raw string style 
        comments for multiline comments
        """)]
    public int Dummy { get; set; }

    [YAXComment("Comment 1 for member\nComment 2 for member")]
    public int SomeInt { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static MultipleCommentsTest GetSampleInstance()
    {
        return new MultipleCommentsTest { SomeInt = 10 };
    }
}
