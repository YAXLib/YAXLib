// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses.CustomSerialization;

public class PropertyLevelSample
{
    public string? Id { get; set; }

    public string? Title { get; set; }

    [YAXCustomSerializer(typeof(PropertyLevelSerializer))]
    public string? Body { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}