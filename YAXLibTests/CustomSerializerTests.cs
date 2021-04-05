// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using NUnit.Framework;
using YAXLib;
using YAXLibTests.SampleClasses.CustomSerialization;

namespace YAXLibTests
{
    [TestFixture]
    public class CustomSerializerTests
    {
        [Test]
        public void Custom_Class_Level_Serializer_Element()
        {
            // The custom serializer for ClassLevelSample handles serialization options

            var original = new ClassLevelSampleAsElement
                {ClassLevelSample = new ClassLevelSample {Title = "The Title", MessageBody = "The Message"}};
            var s = new YAXSerializer(typeof(ClassLevelSampleAsElement));
            var xml = s.Serialize(original);
            var deserialized = (ClassLevelSampleAsElement) s.Deserialize(xml);
            
            Assert.That(xml, Is.EqualTo(@"<ClassLevelSampleAsElement>
  <ClassLevelSample>
    <Title>The Title</Title>
    <MessageBody>The Message</MessageBody>
  </ClassLevelSample>
</ClassLevelSampleAsElement>"));
            Assert.That(deserialized.ToString(), Is.EqualTo(original.ToString()));
        }
        
        [Test]
        public void Custom_Class_Level_Serializer_Attribute()
        {
            // The custom serializer for ClassLevelSample handles serialization options

            var original = new ClassLevelSampleAsAttribute
                {ClassLevelSample = new ClassLevelSample {Title = "The Title", MessageBody = "The Message"}};
            var s = new YAXSerializer(typeof(ClassLevelSampleAsAttribute));
            var xml = s.Serialize(original);
            var deserialized = (ClassLevelSampleAsAttribute) s.Deserialize(xml);
            
            Assert.That(xml, Is.EqualTo("<ClassLevelSampleAsAttribute ClassLevelSample=\"ATTR|The Title|The Message\" />"));
            Assert.That(deserialized.ToString(), Is.EqualTo(original.ToString()));
        }
        
        [Test]
        public void Custom_Class_Level_Serializer_Value()
        {
            // The custom serializer for ClassLevelSample handles serialization options
            
            var original = new ClassLevelSampleAsValue { ClassLevelSample = new ClassLevelSample { Title = "The Title", MessageBody = "The Message"}};
            var s = new YAXSerializer(typeof(ClassLevelSampleAsValue));
            var xml = s.Serialize(original);
            var deserialized = (ClassLevelSampleAsValue) s.Deserialize(xml);
            
            Assert.That(xml, Is.EqualTo("<ClassLevelSampleAsValue>VAL|The Title|The Message</ClassLevelSampleAsValue>"));
            Assert.That(deserialized.ToString(), Is.EqualTo(original.ToString()));
        }
        
        [Test]
        public void Custom_Field_Serializer()
        {
            // The custom serializer handles Body property
            
            var original = new FieldLevelSample
            {
                Id = "1234", // default serializer
                Title = "This is the title", // default serializer
                Body = "Just a short message body" // use custom serializer
            };
            var s = new YAXSerializer(typeof(FieldLevelSample));
            var xml = s.Serialize(original);
            var deserialized = (FieldLevelSample) s.Deserialize(xml);
            var expectedXml = 
                @"<FieldLevelSample>
  <Id>1234</Id>
  <Title>This is the title</Title>
  <Body>ELE__Just a short message body</Body>
</FieldLevelSample>";
            
            Assert.That(xml, Is.EqualTo(expectedXml));
            Assert.That(deserialized.ToString(), Is.EqualTo(original.ToString()));
        }
        
        [Test]
        public void Custom_Field_Serializer_Combined()
        {
            // All properties will use the same serializer
            // in combination with YAXAttributeForClass and YAXValueForClass
            
            var original = new FieldLevelCombinedSample
                {
                    Id = "1234", // serialize as class attribute
                    Title = "This is the title", // serialize as element
                    Body = "Just a short message body" // serialize as value
                };
            var s = new YAXSerializer(typeof(FieldLevelCombinedSample));
            var xml = s.Serialize(original);
            var deserialized = (FieldLevelCombinedSample) s.Deserialize(xml);
            var expectedXml = 
@"<FieldLevelCombinedSample Id=""ATTR_1234"">
  <Title>ELE__This is the title</Title>VAL__Just a short message body</FieldLevelCombinedSample>";
            
            Assert.That(xml, Is.EqualTo(expectedXml));
            Assert.That(deserialized.ToString(), Is.EqualTo(original.ToString()));
        }
    }
}
