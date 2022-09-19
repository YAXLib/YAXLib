// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

[YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AllFields)]
public class NullableSample1
{
    public string? Text { get; set; }
    public Sample1Enum TestEnumProperty { get; set; }
    public Sample1Enum? TestEnumNullableProperty { get; set; }
    public Sample1Enum TestEnumField;
    public Sample1Enum? TestEnumNullableField;

    public static NullableSample1 GetSampleInstance()
    {
        return new() {
            Text = "Hello World",
            TestEnumProperty = Sample1Enum.EnumOne,
            TestEnumNullableProperty = Sample1Enum.EnumTwo,
            TestEnumField = Sample1Enum.EnumOne,
            TestEnumNullableField = Sample1Enum.EnumThree
        };
    }
}

public enum Sample1Enum
{
    [YAXEnum("yax-enum-for-EnumOne")] EnumOne,
    [YAXEnum("yax-enum-for-EnumTwo")] EnumTwo,
    [YAXEnum("yax-enum-for-EnumThree")] EnumThree
}