// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

public class CollectionWithNoContainingElementWithExtraProperties {
    public string? Id { get; set; }
    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement)]
    public List<CollectionWithNoContainingElementWithExtraPropertiesItem>? Items { get; set; }

    public static CollectionWithNoContainingElementWithExtraProperties GetSampleInstance() {
        return new CollectionWithNoContainingElementWithExtraProperties {
            Id = "id-0",
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

public class CollectionWithNoContainingElementWithExtraPropertiesItem {
    public string? PropertyA { get; set; }
    public string? PropertyB { get; set; }
}