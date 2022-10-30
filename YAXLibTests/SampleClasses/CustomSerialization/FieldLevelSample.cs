// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses.CustomSerialization;

[YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
public class FieldLevelSample
{
    [YAXAttributeForClass] [YAXCustomSerializer(typeof(FieldLevelSerializer))]
    public string? Id;

    [YAXValueFor(nameof(Title))] [YAXCustomSerializer(typeof(FieldLevelSerializer))]
    public string? Title;

    [YAXElementFor(nameof(Body))]
    [YAXCustomSerializer(typeof(FieldLevelSerializer))]
    [YAXSerializeAs("ChildOfBody")]
    public string? Body;

    public override string ToString()
    {
        return GeneralToStringProvider.GeneralToString(this);
    }
}