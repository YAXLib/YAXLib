// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Text;
using YAXLib;

namespace YAXLibTests.SampleClasses
{
    [ShowInDemoApplication]
    [YAXComment("This example shows how to choose the fields to be serialized")]
    [YAXSerializableType(FieldsToSerialize = YAXSerializationFields.AttributedFieldsOnly)]
    public class FieldSerializationExample
    {
        [YAXSerializableField] private readonly int m_someInt;
        
        [YAXSerializableField] private readonly double m_someDouble;

        public FieldSerializationExample()
        {
            m_someInt = 8;
            m_someDouble = 3.14;
            SomePrivateStringProperty = "Hi";
            SomePublicPropertyThatIsNotSerialized = "Public";
        }

        [YAXSerializableField] private string SomePrivateStringProperty { get; }

        public string SomePublicPropertyThatIsNotSerialized { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("m_someInt: " + m_someInt);
            sb.AppendLine("m_someDouble: " + m_someDouble);
            sb.AppendLine("SomePrivateStringProperty: " + SomePrivateStringProperty);
            sb.AppendLine("SomePublicPropertyThatIsNotSerialized: " + SomePublicPropertyThatIsNotSerialized);

            return sb.ToString();
        }

        public static FieldSerializationExample GetSampleInstance()
        {
            return new FieldSerializationExample();
        }
    }
}