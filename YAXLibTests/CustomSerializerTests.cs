// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Reflection;
using NUnit.Framework;
using YAXLib;
using YAXLib.Exceptions;
using YAXLib.Options;
using YAXLibTests.SampleClasses.CustomSerialization;

namespace YAXLibTests
{
    [TestFixture]
    public class CustomSerializerTests
    {
        [Test]
        public void Custom_Serializer_Not_Implementing_Proper_Interface_Should_Throw()
        {
            var s = new YAXSerializer(typeof(ISampleInterface));
            var original = new IllegalTypeOfClassSerializer();
            Assert.That(code: ()=> s.Serialize(original), Throws.TypeOf<YAXObjectTypeMismatch>());
        }

        [Test]
        public void Custom_Interface_Serializer_Should_Throw_If_Missing_Interface()
        {
            var s = new YAXSerializer(typeof(ISampleInterface));
            var original = new GenericClassWithoutInterface<int>
            { 
                Something = 9876,
                Id = 12345,
                Name = "The " + nameof(ISampleInterface.Name)
            };
            Assert.That(code: ()=> s.Serialize(original), Throws.TypeOf<YAXObjectTypeMismatch>());
        }

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
            
            var original = new PropertyLevelSample
            {
                Id = "1234", // default serializer
                Title = "This is the title", // default serializer
                Body = "Just a short message body" // use custom serializer
            };
            var s = new YAXSerializer(typeof(PropertyLevelSample));
            var xml = s.Serialize(original);
            var deserialized = (PropertyLevelSample) s.Deserialize(xml);
            var expectedXml = 
                @"<PropertyLevelSample>
  <Id>1234</Id>
  <Title>This is the title</Title>
  <Body>ELE__Just a short message body</Body>
</PropertyLevelSample>";
            
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

        [Test]
        public void Custom_NonGenericClass_Interface_Serializer()
        {
            // Use an interface as type to serialize a non-generic class

            var s = new YAXSerializer(typeof(ISampleInterface));
            var original = (ISampleInterface) new NonGenericClassWithInterface
            { 
                Id = 12345,
                Name = "The " + nameof(ISampleInterface.Name)
            };
            var xml = s.Serialize(original);
            var deserialized = (ISampleInterface) s.Deserialize(xml);
            var expectedXmlPart = 
                $@"
  <Id>{original.Id}</Id>
  <Name>{original.Name}</Name>
</ISampleInterface>";
            Assert.That(xml, Does.Contain(expectedXmlPart), "Serialized XML");
            Assert.That(deserialized.ToString(), Is.EqualTo(original.ToString()), "Deserialized Object");
        }

        [Test]
        public void Custom_GenericClass_Interface_Serializer()
        {
            // Use an interface as type to serialize a generic class

            var s = new YAXSerializer(typeof(ISampleInterface));
            var original = new GenericClassWithInterface<int>
            { 
                Something = 9876,
                Id = 12345,
                Name = "The " + nameof(ISampleInterface.Name)
            };
            var xml = s.Serialize(original);
            var deserialized = (ISampleInterface) s.Deserialize(xml);
            var expectedXmlPart = 
                $@"
  <Id>{original.Id}</Id>
  <Name>{original.Name}</Name>
</ISampleInterface>";

            Assert.That(xml, Does.Contain(expectedXmlPart), "Serialized XML");
            Assert.That(deserialized.ToString(), Is.EqualTo(original.ToString()), "Deserialized Object");
        }
    }
}
