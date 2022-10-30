// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses.PolymorphicSerialization;

public class BaseContainer
{
    [YAXCollection(YAXCollectionSerializationTypes.RecursiveWithNoContainingElement)]
    public BaseItem[]? Items { get; set; }
}

public class DerivedContainer : BaseContainer
{
}

public class BaseItem
{
    [YAXAttributeForClass] public string? Data { get; set; }
}

public class DerivedItem : BaseItem
{
}