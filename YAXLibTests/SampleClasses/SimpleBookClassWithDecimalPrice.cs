// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[YAXComment("This example demonstrates serializing a very simple class")]
public class SimpleBookClassWithDecimalPrice
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public int PublishYear { get; set; }
    public decimal Price { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static SimpleBookClassWithDecimalPrice GetSampleInstance()
    {
        return new SimpleBookClassWithDecimalPrice {
            Title = "Inside C#",
            Author = "Tom Archer & Andrew Whitechapel",
            PublishYear = 2002,
            Price = 32.20m //30.5
        };
    }
}