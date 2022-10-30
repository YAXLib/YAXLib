// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

[Flags]
public enum Seasons
{
    [YAXEnum("Spring")] First = 1,

    [YAXEnum("Summer")] Second = 2,

    [YAXEnum(" Autumn or fall ")] Third = 4,

    [YAXEnum("Winter")] Fourth = 8
}

[ShowInDemoApplication]
[YAXComment("This example shows how to define aliases for enum members")]
public class EnumsSample
{
    [YAXAttributeForClass] public Seasons OneInstance { get; set; }

    [YAXCollection(YAXCollectionSerializationTypes.Serially, SeparateBy = ";", IsWhiteSpaceSeparator = false)]
    public Seasons[]? TheSeasonSerially { get; set; }

    [YAXCollection(YAXCollectionSerializationTypes.Recursive)]
    public Seasons[]? TheSeasonRecursive { get; set; }

    public Dictionary<Seasons, int> DicSeasonToInt { get; set; } = new();

    public Dictionary<int, Seasons> DicIntToSeason { get; set; } = new();

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }

    public static EnumsSample GetSampleInstance()
    {
        var dicSeas2Int = new Dictionary<Seasons, int> {
            { Seasons.First, 1 },
            { Seasons.Second, 2 },
            { Seasons.Third, 3 },
            { Seasons.Fourth, 4 }
        };

        var dicInt2Seas = new Dictionary<int, Seasons> {
            { 1, Seasons.First },
            { 2, Seasons.Second | Seasons.First },
            { 3, Seasons.Third },
            { 4, Seasons.Fourth }
        };

        return new EnumsSample {
            OneInstance = Seasons.First | Seasons.Second,
            TheSeasonRecursive = new[] { Seasons.First, Seasons.Second, Seasons.Third, Seasons.Fourth },
            TheSeasonSerially = new[] { Seasons.First, Seasons.Second, Seasons.Third, Seasons.Fourth },
            DicSeasonToInt = dicSeas2Int,
            DicIntToSeason = dicInt2Seas
        };
    }
}