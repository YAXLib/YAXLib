// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

public class CollectionWithNoContainingElement {
    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement)]
    public List<CollectionWithNoContainingElementItem>? Items { get; set; }

    public static CollectionWithNoContainingElement GetSampleInstance() {
        return new CollectionWithNoContainingElement {
            Items = new() {
                new() {
                    PropertyA = "0",
                    PropertyB = "1",
                },
                new() {
                    PropertyA = "2",
                    PropertyB = "3",
                },
            },
        };
    }
}

public class CollectionWithNoContainingElementItem {
    public string? PropertyA { get; set; }
    public string? PropertyB { get; set; }
}