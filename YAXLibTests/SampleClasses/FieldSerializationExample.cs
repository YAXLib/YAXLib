// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Text;
using YAXLib.Attributes;
using YAXLib.Enums;

namespace YAXLibTests.SampleClasses;

[ShowInDemoApplication]
[YAXComment("This example shows how to choose the fields to be serialized")]
[YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AttributedFieldsOnly)]
public class FieldSerializationExample
{
    [YAXSerializableField] private readonly int _someInt;

    [YAXSerializableField] private readonly double _someDouble;

    public FieldSerializationExample()
    {
        _someInt = 8;
        _someDouble = 3.14;
        SomePrivateStringProperty = "Hi";
        SomePublicPropertyThatIsNotSerialized = "Public";
    }

    [YAXSerializableField] private string SomePrivateStringProperty { get; }

    public string SomePublicPropertyThatIsNotSerialized { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine("_someInt: " + _someInt);
        sb.AppendLine("_someDouble: " + _someDouble);
        sb.AppendLine("SomePrivateStringProperty: " + SomePrivateStringProperty);
        sb.AppendLine("SomePublicPropertyThatIsNotSerialized: " + SomePublicPropertyThatIsNotSerialized);

        return sb.ToString();
    }

    public static FieldSerializationExample GetSampleInstance()
    {
        return new FieldSerializationExample();
    }
}