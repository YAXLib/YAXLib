// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

public class OneLetterAlias
{
    [YAXValueFor("./T")] public string? Title { get; set; }

    [YAXValueFor("./A")] public string? Author { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static OneLetterAlias GetSampleInstance()
    {
        return new OneLetterAlias {
            Title = "Inside C#",
            Author = "Tom Archer & Andrew Whitechapel"
        };
    }
}