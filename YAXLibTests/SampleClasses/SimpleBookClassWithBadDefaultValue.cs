// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication(SortKey = "001")]
[YAXComment("This example demonstrates serailizing a very simple class")]
public class BookWithBadDefaultValue
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public int PublishYear { get; set; }

    [YAXErrorIfMissed(YAXExceptionTypes.Ignore, DefaultValue = "NOTDOUBLE")]
    public double Price { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static Book GetSampleInstance()
    {
        return new Book {
            Title = "Inside C#",
            Author = "Tom Archer & Andrew Whitechapel",
            PublishYear = 2002,
            Price = 30.5
        };
    }
}