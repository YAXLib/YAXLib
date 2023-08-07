// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

public class Author3
{
    [YAXSerializeAs("AuthorName")]
    [YAXAttributeFor("../PublishYear")]
    public string? Name { get; set; }

    [YAXSerializeAs("AuthorAge")]
    [YAXElementFor("..")]
    public int Age { get; set; }
}

[ShowInDemoApplication]
[YAXComment("""
    This example shows how to serialize collection objects while
    not serializing the element for their enclosing collection itself
    """)]
public class MoreComplexBook3
{
    public string? Title { get; set; }

    [YAXComment("Comment for author")] public Author3? Author { get; set; }

    public int PublishYear { get; set; }
    public double Price { get; set; }

    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement, EachElementName = "Editor")]
    public string[]? Editors { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static MoreComplexBook3 GetSampleInstance()
    {
        var auth = new Author3 { Age = 30, Name = "Tom Archer" };
        return new MoreComplexBook3 {
            Title = "Inside C#",
            Author = auth,
            Editors = new[] { "Mark Twain", "Timothy Jones", "Oliver Twist" },
            PublishYear = 2002,
            Price = 30.5
        };
    }
}
