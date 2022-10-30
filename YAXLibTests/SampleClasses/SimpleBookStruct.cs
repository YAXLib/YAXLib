// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication(SortKey = "002")]
[YAXComment("This example demonstrates serializing a very simple struct")]
public struct BookStruct
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int PublishYear { get; set; }
    public double Price { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static BookStruct GetSampleInstance()
    {
        return new BookStruct {
            Title = "Reinforcement Learning an Introduction",
            Author = "R. S. Sutton & A. G. Barto",
            PublishYear = 1998,
            Price = 38.75
        };
    }
}