// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

[YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
public class NullableSample3
{
    public string? Text { get; set; }
    public Sample3Enum TestEnumProperty { get; set; }
    public Sample3Enum? TestEnumNullableProperty { get; set; }
    public Sample3Enum TestEnumField;
    public Sample3Enum? TestEnumNullableField;

    public static NullableSample3 GetSampleInstance()
    {
        return new() {
            Text = "Hello World",
            TestEnumProperty = Sample3Enum.EnumOne,
            TestEnumNullableProperty = Sample3Enum.EnumTwo,
            TestEnumField = Sample3Enum.EnumThree,
            TestEnumNullableField = Sample3Enum.EnumFour
        };
    }
}

public enum Sample3Enum
{
    [YAXEnum("yax-enum-for-EnumOne")] EnumOne,
    [YAXEnum("yax-enum-for-EnumTwo")] EnumTwo,
    [YAXEnum("yax-enum-for-EnumThree")] EnumThree,

    [YAXEnum("yax-enum-for-EnumThree")] // duplicate alias
    EnumFour
}