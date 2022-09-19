// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses;

[YAXSerializeAs("dashed-sample")]
public class DashedSample
{
    [YAXSerializeAs("dashed-name")]
    [YAXAttributeForClass]
    public string? DashedName { get; set; }
}