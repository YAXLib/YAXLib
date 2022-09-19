// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;

namespace YAXLibTests.SampleClasses.CustomSerialization;

/// <summary>
/// Combines the universal <see cref="FieldLevelCombinedSerializer" /> for each field
/// with more attributes that determine how properties will be serialized.
/// </summary>
public class FieldLevelCombinedSample
{
    [YAXCustomSerializer(typeof(FieldLevelCombinedSerializer))]
    [YAXAttributeForClass]
    public string? Id { get; set; }

    /// <summary>
    /// Serialize as element
    /// </summary>
    [YAXCustomSerializer(typeof(FieldLevelCombinedSerializer))]
    public string? Title { get; set; }

    [YAXCustomSerializer(typeof(FieldLevelCombinedSerializer))]
    [YAXValueForClass]
    public string? Body { get; set; }

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}