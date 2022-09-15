// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.IO;
using System.Xml;
using NUnit.Framework;
using YAXLib;
using YAXLib.Enums;
using YAXLib.Exceptions;
using YAXLib.Options;
using YAXLibTests.SampleClasses;

namespace YAXLibTests
{
    internal class ExceptionTests
    {
        [Test]
        public void AttributeAlreadyExistsExceptionTest()
        {
            var ex = Assert.Throws<YAXAttributeAlreadyExistsException>(() =>
            {
                var serializer = new YAXSerializer<ClassWithDuplicateYaxAttribute>(new SerializerOptions {
                    ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors,
                    ExceptionBehavior = YAXExceptionTypes.Error
                });
                serializer.Serialize(ClassWithDuplicateYaxAttribute.GetSampleInstance());
            });

            Assert.AreEqual("test", ex?.AttrName);
            StringAssert.Contains("'test'", ex?.Message);
        }

        [Test]
        public void MalformedXmlStringThatThrows()
        {
            var badXml = @"<!-- This example demonstrates serializing a very simple class -->
<Book>
  <Title>Inside C#</Title>
  <Author>Tom Archer &amp; Andrew Whitechapel</Author>
  <PublishYear>2002</PublishYear>
  <Price>BADDATA<Price>
</Book>";
            var ex = Assert.Throws<YAXBadlyFormedXML>(() =>
            {
                var serializer = new YAXSerializer<Book>(new SerializerOptions {
                    ExceptionBehavior = YAXExceptionTypes.Error,
                    ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors
                });
                serializer.Deserialize(badXml);
            });

            // YAXBadlyFormedXML exception doesn't need YAXSerializationOptions.DisplayLineInfoInExceptions to get the line numbers
            Assert.True(ex!.HasLineInfo);
            Assert.That(ex.LineNumber, Is.EqualTo(7));
            Assert.That(ex.LinePosition, Is.EqualTo(3));
            Assert.That(ex.Message, Does.Contain("not properly formatted"));
        }

        [Test]
        public void MalformedXmlStringThatDoesNotThrow()
        {
            const string xml =
                @"<Book></MalformedXml>";

            var serializer = new YAXSerializer<Book>(new SerializerOptions
            {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
            });

            object? result = "";
            Assert.That(code: () => { result = serializer.Deserialize(xml); }, Throws.Nothing);
            Assert.That(result, Is.Null);
            Assert.That(serializer.ParsingErrors.ToString(), Does.Contain("not properly formatted"));
        }

        [Test]
        public void DeserializeMalformedXmlFromTextReaderShouldThrow()
        {
            const string xml = @"<Book></MalformedXml>";
            var serializer = new YAXSerializer<Book>(new SerializerOptions
            {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });

            using var stream = new MemoryStream();
            using var streamWriter = new StreamWriter(stream);
            using var streamReader = new StreamReader(stream);
            streamWriter.Write(xml);
            streamWriter.Flush();
            stream.Position = 0;

            Assert.Throws<YAXBadlyFormedXML>(code: () =>
            {
                serializer.Deserialize(streamReader);
            });
        }

        [Test]
        public void DeserializeMalformedXmlFromTextReaderDoesNotThrow()
        {
            const string xml = @"<Book></MalformedXml>";
            var serializer = new YAXSerializer<Book>(new SerializerOptions
            {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });

            using var stream = new MemoryStream();
            using var streamWriter = new StreamWriter(stream);
            using var streamReader = new StreamReader(stream);
            streamWriter.Write(xml);
            streamWriter.Flush();
            stream.Position = 0;

            object? result = "";
            Assert.That(code: () => { result = serializer.Deserialize(streamReader); }, Throws.Nothing);
            Assert.That(result, Is.Null);
            Assert.That(serializer.ParsingErrors.ToString(), Does.Contain("not properly formatted"));
        }

        [Test]
        public void DeserializeMalformedXmlFromXmlReaderShouldThrow()
        {
            const string xml = @"<Book></MalformedXml>";
            var serializer = new YAXSerializer<Book>(new SerializerOptions
            {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });

            using var stream = new MemoryStream();
            using var streamWriter = new StreamWriter(stream);
            using var xmlWriter = XmlWriter.Create(stream);
            streamWriter.Write(xml);
            streamWriter.Flush();
            stream.Position = 0;

            using var xmlReader = XmlReader.Create(stream);
            Assert.Throws<YAXBadlyFormedXML>(code: () =>
            {
                serializer.Deserialize(xmlReader);
            });
        }

        [Test]
        public void DeserializeMalformedXmlFromXmlReaderDoesNotThrow()
        {
            const string xml = @"<Book></MalformedXml>";
            var serializer = new YAXSerializer<Book>(new SerializerOptions
            {
                ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.DoNotThrow,
                ExceptionBehavior = YAXExceptionTypes.Warning,
                SerializationOptions = YAXSerializationOptions.SerializeNullObjects
            });

            using var stream = new MemoryStream();
            using var streamWriter = new StreamWriter(stream);
            using var xmlWriter = XmlWriter.Create(stream);
            streamWriter.Write(xml);
            streamWriter.Flush();
            stream.Position = 0;

            using var xmlReader = XmlReader.Create(stream);
            object? result = "";
            Assert.That(code: () => { result = serializer.Deserialize(xmlReader); }, Throws.Nothing);
            Assert.That(result, Is.Null);
            Assert.That(serializer.ParsingErrors.ToString(), Does.Contain("not properly formatted"));
        }

        [Test]
        public void MalformedInputWithLineNumbersTest()
        {
            const string bookXml =
                @"<!-- This example demonstrates serailizing a very simple class -->
<Book>
  <Title>Inside C#</Title>
  <Author>Tom Archer &amp; Andrew Whitechapel</Author>
  <PublishYear>2002</PublishYear>
  <Price>BADDATA</Price>
</Book>";

            var ex = Assert.Throws<YAXBadlyFormedInput>(() =>
            {
                var serializer = new YAXSerializer<Book>(new SerializerOptions {
                    ExceptionBehavior = YAXExceptionTypes.Error,
                    ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors,
                    SerializationOptions = YAXSerializationOptions.DisplayLineInfoInExceptions
                });
                serializer.Deserialize(bookXml);
            });
            Assert.True(ex?.HasLineInfo);
            Assert.AreEqual(6, ex?.LineNumber);
            Assert.AreEqual(4, ex?.LinePosition);
            StringAssert.Contains("The format of the value specified for the property", ex?.Message);
        }

        [Test]
        public void MalformedInputWithoutLineNumbersTest()
        {
            const string bookXml =
                @"<!-- This example demonstrates serailizing a very simple class -->
<Book>
  <Title>Inside C#</Title>
  <Author>Tom Archer &amp; Andrew Whitechapel</Author>
  <PublishYear>2002</PublishYear>
  <Price>BADDATA</Price>
</Book>";

            var ex = Assert.Throws<YAXBadlyFormedInput>(() =>
            {
                var serializer = new YAXSerializer<Book>(new SerializerOptions {
                    ExceptionBehavior = YAXExceptionTypes.Error,
                    ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors
                });
                serializer.Deserialize(bookXml);
            });
            Assert.False(ex?.HasLineInfo);
            Assert.AreEqual(0, ex?.LineNumber);
            Assert.AreEqual(0, ex?.LinePosition);
            StringAssert.Contains("The format of the value specified for the property", ex?.Message);
        }

        [Test]
        public void ObjectTypeMismatchExceptionTest()
        {
            var ex = Assert.Throws<YAXObjectTypeMismatch>(() =>
            {
                var serializer = new YAXSerializer(typeof(Book), new SerializerOptions {
                    ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowErrorsOnly
                });
                serializer.Serialize(new ClassWithDuplicateYaxAttribute());
            });

            Assert.AreEqual(typeof(Book), ex?.ExpectedType);
            Assert.AreEqual(typeof(ClassWithDuplicateYaxAttribute), ex?.ReceivedType);
            StringAssert.Contains("'Book'", ex?.Message);
            StringAssert.Contains("'ClassWithDuplicateYaxAttribute'", ex?.Message);
        }


        [Test]
        public void ElementValueMissingWithLineNumbersTest()
        {
            const string bookXml =
                @"<!-- This example demonstrates serailizing a very simple class -->
<Book>
  <Title>Inside C#</Title>
  <Author>Tom Archer &amp; Andrew Whitechapel</Author>
  <PublishYear>2002</PublishYear>
</Book>";

            var ex = Assert.Throws<YAXElementValueMissingException>(() =>
            {
                var serializer = new YAXSerializer<BookClassTestingSerializeAsValue>(new SerializerOptions {
                    ExceptionBehavior = YAXExceptionTypes.Error,
                    ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors,
                    SerializationOptions = YAXSerializationOptions.DisplayLineInfoInExceptions
                });
                serializer.Deserialize(bookXml);
            });
            Assert.True(ex?.HasLineInfo);
            Assert.AreEqual(2, ex?.LineNumber);
            Assert.AreEqual(2, ex?.LinePosition);
        }

        [Test]
        public void ElementMissingWithLineNumbersTest()
        {
            const string collectionXml =
                @"<!-- This example demonstrates serailizing a very simple class -->
<CollectionSeriallyAsAttribute>
  <Info names=""John Doe, Jane, Sina, Mike, Rich"" />
  <Location>
    <Countries> Iran,Australia,United States of America, France</Countries>
  </Location>
</CollectionSeriallyAsAttribute> ";

            var ex = Assert.Throws<YAXElementMissingException>(() =>
            {
                var serializer = new YAXSerializer<CollectionSeriallyAsAttribute>(new SerializerOptions {
                    ExceptionBehavior = YAXExceptionTypes.Error,
                    ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors,
                    SerializationOptions = YAXSerializationOptions.DisplayLineInfoInExceptions
                });
                serializer.Deserialize(collectionXml);
            });
            Assert.True(ex?.HasLineInfo);
            Assert.AreEqual(2, ex?.LineNumber);
            Assert.AreEqual(2, ex?.LinePosition);
        }

        [Test]
        public void DefaultValueCannotBeAssignedWithLineNumbersTest()
        {
            const string bookXml =
                @"<Book>
  <Title>Inside C#</Title>
  <Author>Tom Archer &amp; Andrew Whitechapel</Author>
  <PublishYear>2002</PublishYear>
</Book>";

            var ex = Assert.Throws<YAXDefaultValueCannotBeAssigned>(() =>
            {
                var serializer = new YAXSerializer<BookWithBadDefaultValue>(new SerializerOptions {
                    ExceptionBehavior = YAXExceptionTypes.Error,
                    ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors,
                    SerializationOptions = YAXSerializationOptions.DisplayLineInfoInExceptions
                });
                serializer.Deserialize(bookXml);
            });
            Assert.True(ex?.HasLineInfo);
            Assert.AreEqual(1, ex?.LineNumber);
            Assert.AreEqual(2, ex?.LinePosition);
        }

        [Test]
        public void BadLocationExceptionLegacyConstructor()
        {
            var testName = "Test";

            Exception? ex =
                Assert.Throws<YAXBadLocationException>(code: () => throw new YAXBadLocationException(testName));
            StringAssert.Contains(testName, ex?.Message);
        }

        [Test]
        public void AttributeAlreadyExistsExceptionLegacyConstructor()
        {
            var testName = "Test";
            var ex = Assert.Throws<YAXAttributeAlreadyExistsException>(code: () =>
                throw new YAXAttributeAlreadyExistsException(testName));
            StringAssert.Contains(testName, ex?.Message);
        }

        [Test]
        public void AttributeMissingExceptionLegacyConstructor()
        {
            var testName = "Test";
            var ex = Assert.Throws<YAXAttributeMissingException>(code: () =>
                throw new YAXAttributeMissingException(testName));
            StringAssert.Contains(testName, ex?.Message);
        }

        [Test]
        public void ElementValueMissingExceptionLegacyConstructor()
        {
            var testName = "Test";
            var ex = Assert.Throws<YAXElementValueMissingException>(code: () =>
                throw new YAXElementValueMissingException(testName));
            StringAssert.Contains(testName, ex?.Message);
        }

        [Test]
        public void ElementMissingExceptionLegacyConstructor()
        {
            var testName = "Test";
            var ex = Assert.Throws<YAXElementMissingException>(code: () =>
                throw new YAXElementMissingException(testName));
            StringAssert.Contains(testName, ex?.Message);
        }

        [Test]
        public void MalformedInputLegacyConstructor()
        {
            var testName = "Test";
            var testInput = "BadInput";
            var ex = Assert.Throws<YAXBadlyFormedInput>(code: () =>
                throw new YAXBadlyFormedInput(testName, testInput));
            StringAssert.Contains(testName, ex?.Message);
            StringAssert.Contains(testInput, ex?.Message);
        }

        [Test]
        public void PropertyCannotBeAssignedToLegacyConstructor()
        {
            var testName = "Test";
            var ex = Assert.Throws<YAXPropertyCannotBeAssignedTo>(code: () =>
                throw new YAXPropertyCannotBeAssignedTo(testName));
            StringAssert.Contains(testName, ex?.Message);
        }

        [Test]
        public void CannotAddObjectToCollectionLegacyConstructor()
        {
            var testName = "Test";
            var testValue = 1;
            var ex = Assert.Throws<YAXCannotAddObjectToCollection>(code: () =>
                throw new YAXCannotAddObjectToCollection(testName, testValue));
            StringAssert.Contains(testName, ex?.Message);
            StringAssert.Contains(testValue.ToString(), ex?.Message);
        }

        [Test]
        public void DefaultValueCannotBeAssignedLegacyConstructor()
        {
            var testName = "Test";
            var testValue = 1;
            var ex = Assert.Throws<YAXDefaultValueCannotBeAssigned>(code: () =>
                throw new YAXDefaultValueCannotBeAssigned(testName, testValue, CultureInfo.InvariantCulture));
            StringAssert.Contains(testName, ex?.Message);
            StringAssert.Contains(testValue.ToString(), ex?.Message);
        }

        [Test]
        public void MalformedXmlLegacyConstructor()
        {
            var testName = "Test";
            var ex = Assert.Throws<YAXBadlyFormedXML>(() => { throw new YAXBadlyFormedXML(new Exception(testName)); });
            StringAssert.Contains(testName, ex?.Message);
        }

        [Test]
        public void CannotSerializeSelfReferentialTypesLegacyConstructor()
        {
            var testType = typeof(string);
            var ex = Assert.Throws<YAXCannotSerializeSelfReferentialTypes>(code: () =>
                throw new YAXCannotSerializeSelfReferentialTypes(testType));
            StringAssert.Contains(testType.Name, ex?.Message);
        }

        [Test]
        public void ObjectTypeMismatchLegacyConstructor()
        {
            var testType = typeof(string);
            var testType2 = typeof(int);
            var ex = Assert.Throws<YAXObjectTypeMismatch>(code: () =>
                throw new YAXObjectTypeMismatch(testType, testType2));
            StringAssert.Contains(testType.Name, ex?.Message);
            StringAssert.Contains(testType2.Name, ex?.Message);
        }

        [Test]
        public void PolymorphicExceptionLegacyConstructor()
        {
            var testName = "Test";
            var ex = Assert.Throws<YAXPolymorphicException>(code: () => throw new YAXPolymorphicException(testName));
            StringAssert.Contains(testName, ex?.Message);
        }
    }
}
