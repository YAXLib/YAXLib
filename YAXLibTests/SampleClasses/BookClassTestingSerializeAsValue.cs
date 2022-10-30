// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

[YAXSerializableType(Options = YAXSerializationOptions.DontSerializeNullObjects)]
public class SomeCollectionItem
{
    public string? Value { get; set; }

    public string? SomeElement { get; set; }
}

[ShowInDemoApplication]
public class BookClassTestingSerializeAsValue
{
    [YAXValueFor(".")] public double Price { get; set; }

    public int PublishYear { get; set; }

    [YAXValueFor(".")] public string? Comments { get; set; }

    public string? Author { get; set; }

    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement)]
    public List<SomeCollectionItem> TheCollection { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static BookClassTestingSerializeAsValue GetSampleInstance()
    {
        var theCollection = new List<SomeCollectionItem> {
            new SomeCollectionItem { Value = "value1", SomeElement = "elem1" },
            new SomeCollectionItem { Value = "value2", SomeElement = "elem2" },
            new SomeCollectionItem { Value = "value3", SomeElement = "elem3" }
        };

        return new BookClassTestingSerializeAsValue {
            Author = "Tom Archer & Andrew Whitechapel",
            PublishYear = 2002,
            Price = 30.5,
            Comments = "SomeComment",
            TheCollection = theCollection
        };
    }
}