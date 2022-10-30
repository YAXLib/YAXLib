// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses.PolymorphicSerialization;

public class MultipleYaxTypeAttributesWithSameType
{
    [YAXType(typeof(string))]
    [YAXType(typeof(string))]
    public object? Object { get; set; }
}