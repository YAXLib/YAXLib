// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Globalization;
using NUnit.Framework;
using YAXLib;
using YAXLibTests.SampleClasses;

namespace YAXLibTests
{
    internal class ExceptionTests
    {
        [Test]
        public void YAXAttributeAlreadyExistsExceptionTest()
        {
            var ex = Assert.Throws<YAXAttributeAlreadyExistsException>(() =>
            {
                var serializer = new YAXSerializer(typeof(ClassWithDuplicateYaxAttribute),
                    YAXExceptionHandlingPolicies.ThrowWarningsAndErrors, YAXExceptionTypes.Error);
                serializer.Serialize(ClassWithDuplicateYaxAttribute.GetSampleInstance());
            });

            Assert.AreEqual("test", ex.AttrName);
            StringAssert.Contains("'test'", ex.Message);
        }

        [Test]
        public void YAXBadlyFormedXMLTest()
        {
            var badXml = @"<!-- This example demonstrates serailizing a very simple class -->
<Book>
  <Title>Inside C#</Title>
  <Author>Tom Archer &amp; Andrew Whitechapel</Author>
  <PublishYear>2002</PublishYear>
  <Price>BADDATA<Price>
</Book>";

            var ex = Assert.Throws<YAXBadlyFormedXML>(() =>
            {
                var serializer = new YAXSerializer(typeof(Book), YAXExceptionHandlingPolicies.ThrowWarningsAndErrors,
                    YAXExceptionTypes.Error);
                serializer.Deserialize(badXml);
            });

            // YAXBadlyFormedXML exception doesn't need YAXSerializationOptions.DisplayLineInfoInExceptions to get the line numbers
            Assert.True(ex.HasLineInfo);
            Assert.AreEqual(7, ex.LineNumber);
            Assert.AreEqual(3, ex.LinePosition);
            StringAssert.Contains("not properly formatted", ex.Message);
        }


        [Test]
        public void YAXBadlyFormedInputWithLineNumbersTest()
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
                var serializer = new YAXSerializer(typeof(Book), YAXExceptionHandlingPolicies.ThrowWarningsAndErrors,
                    YAXExceptionTypes.Error, YAXSerializationOptions.DisplayLineInfoInExceptions);
                serializer.Deserialize(bookXml);
            });
            Assert.True(ex.HasLineInfo);
            Assert.AreEqual(6, ex.LineNumber);
            Assert.AreEqual(4, ex.LinePosition);
            StringAssert.Contains("The format of the value specified for the property", ex.Message);
        }

        [Test]
        public void YAXBadlyFormedInputWithoutLineNumbersTest()
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
                var serializer = new YAXSerializer(typeof(Book), YAXExceptionHandlingPolicies.ThrowWarningsAndErrors,
                    YAXExceptionTypes.Error);
                serializer.Deserialize(bookXml);
            });
            Assert.False(ex.HasLineInfo);
            Assert.AreEqual(0, ex.LineNumber);
            Assert.AreEqual(0, ex.LinePosition);
            StringAssert.Contains("The format of the value specified for the property", ex.Message);
        }

        [Test]
        public void YAXObjectTypeMismatchExceptionTest()
        {
            var ex = Assert.Throws<YAXObjectTypeMismatch>(() =>
            {
                var serializer = new YAXSerializer(typeof(Book), YAXExceptionHandlingPolicies.ThrowErrorsOnly);
                serializer.Serialize(new ClassWithDuplicateYaxAttribute());
            });

            Assert.AreEqual(typeof(Book), ex.ExpectedType);
            Assert.AreEqual(typeof(ClassWithDuplicateYaxAttribute), ex.ReceivedType);
            StringAssert.Contains("'Book'", ex.Message);
            StringAssert.Contains("'ClassWithDuplicateYaxAttribute'", ex.Message);
        }


        [Test]
        public void YAXElementValueMissingWithLineNumbersTest()
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
                var serializer = new YAXSerializer(typeof(BookClassTesgingSerializeAsValue),
                    YAXExceptionHandlingPolicies.ThrowWarningsAndErrors, YAXExceptionTypes.Error,
                    YAXSerializationOptions.DisplayLineInfoInExceptions);
                serializer.Deserialize(bookXml);
            });
            Assert.True(ex.HasLineInfo);
            Assert.AreEqual(2, ex.LineNumber);
            Assert.AreEqual(2, ex.LinePosition);
        }

        [Test]
        public void YAXElementMissingWithLineNumbersTest()
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
                var serializer = new YAXSerializer(typeof(CollectionSeriallyAsAttribute),
                    YAXExceptionHandlingPolicies.ThrowWarningsAndErrors, YAXExceptionTypes.Error,
                    YAXSerializationOptions.DisplayLineInfoInExceptions);
                serializer.Deserialize(collectionXml);
            });
            Assert.True(ex.HasLineInfo);
            Assert.AreEqual(2, ex.LineNumber);
            Assert.AreEqual(2, ex.LinePosition);
        }

        [Test]
        public void YAXDefaultValueCannotBeAssignedWithLineNumbersTest()
        {
            const string bookXml =
                @"<Book>
  <Title>Inside C#</Title>
  <Author>Tom Archer &amp; Andrew Whitechapel</Author>
  <PublishYear>2002</PublishYear>
</Book>";

            var ex = Assert.Throws<YAXDefaultValueCannotBeAssigned>(() =>
            {
                var serializer = new YAXSerializer(typeof(BookWithBadDefaultValue),
                    YAXExceptionHandlingPolicies.ThrowWarningsAndErrors, YAXExceptionTypes.Error,
                    YAXSerializationOptions.DisplayLineInfoInExceptions);
                serializer.Deserialize(bookXml);
            });
            Assert.True(ex.HasLineInfo);
            Assert.AreEqual(1, ex.LineNumber);
            Assert.AreEqual(2, ex.LinePosition);
        }

        [Test]
        public void YAXBadLocationExceptionLegacyConstructor()
        {
            var testName = "Test";

            Exception ex =
                Assert.Throws<YAXBadLocationException>(() => { throw new YAXBadLocationException(testName); });
            StringAssert.Contains(testName, ex.Message);
        }

        [Test]
        public void YAXAttributeAlreadyExistsExceptionLegacyConstructor()
        {
            var testName = "Test";
            var ex = Assert.Throws<YAXAttributeAlreadyExistsException>(() =>
            {
                throw new YAXAttributeAlreadyExistsException(testName);
            });
            StringAssert.Contains(testName, ex.Message);
        }

        [Test]
        public void YAXAttributeMissingExceptionLegacyConstructor()
        {
            var testName = "Test";
            var ex = Assert.Throws<YAXAttributeMissingException>(() =>
            {
                throw new YAXAttributeMissingException(testName);
            });
            StringAssert.Contains(testName, ex.Message);
        }

        [Test]
        public void YAXElementValueMissingExceptionLegacyConstructor()
        {
            var testName = "Test";
            var ex = Assert.Throws<YAXElementValueMissingException>(() =>
            {
                throw new YAXElementValueMissingException(testName);
            });
            StringAssert.Contains(testName, ex.Message);
        }

        [Test]
        public void YAXElementValueAlreadyExistsExceptionLegacyConstructor()
        {
            var testName = "Test";
            var ex = Assert.Throws<YAXElementValueAlreadyExistsException>(() =>
            {
                throw new YAXElementValueAlreadyExistsException(testName);
            });
            StringAssert.Contains(testName, ex.Message);
        }

        [Test]
        public void YAXElementMissingExceptionLegacyConstructor()
        {
            var testName = "Test";
            var ex = Assert.Throws<YAXElementMissingException>(() =>
            {
                throw new YAXElementMissingException(testName);
            });
            StringAssert.Contains(testName, ex.Message);
        }

        [Test]
        public void YAXBadlyFormedInputLegacyConstructor()
        {
            var testName = "Test";
            var testInput = "BadInput";
            var ex = Assert.Throws<YAXBadlyFormedInput>(() => { throw new YAXBadlyFormedInput(testName, testInput); });
            StringAssert.Contains(testName, ex.Message);
            StringAssert.Contains(testInput, ex.Message);
        }

        [Test]
        public void YAXPropertyCannotBeAssignedToLegacyConstructor()
        {
            var testName = "Test";
            var ex = Assert.Throws<YAXPropertyCannotBeAssignedTo>(() =>
            {
                throw new YAXPropertyCannotBeAssignedTo(testName);
            });
            StringAssert.Contains(testName, ex.Message);
        }

        [Test]
        public void YAXCannotAddObjectToCollectionLegacyConstructor()
        {
            var testName = "Test";
            var testValue = 1;
            var ex = Assert.Throws<YAXCannotAddObjectToCollection>(() =>
            {
                throw new YAXCannotAddObjectToCollection(testName, testValue);
            });
            StringAssert.Contains(testName, ex.Message);
            StringAssert.Contains(testValue.ToString(), ex.Message);
        }

        [Test]
        public void YAXDefaultValueCannotBeAssignedLegacyConstructor()
        {
            var testName = "Test";
            var testValue = 1;
            var ex = Assert.Throws<YAXDefaultValueCannotBeAssigned>(() =>
            {
                throw new YAXDefaultValueCannotBeAssigned(testName, (object) testValue, CultureInfo.InvariantCulture);
            });
            StringAssert.Contains(testName, ex.Message);
            StringAssert.Contains(testValue.ToString(), ex.Message);
        }

        [Test]
        public void YAXBadlyFormedXMLLegacyConstructor()
        {
            var testName = "Test";
            var ex = Assert.Throws<YAXBadlyFormedXML>(() => { throw new YAXBadlyFormedXML(new Exception(testName)); });
            StringAssert.Contains(testName, ex.Message);
        }

        [Test]
        public void YAXInvalidFormatProvidedLegacyConstructor()
        {
            var testInput = "BadInput";
            var testType = typeof(string);
            var ex = Assert.Throws<YAXInvalidFormatProvided>(() =>
            {
                throw new YAXInvalidFormatProvided(testType, testInput);
            });
            StringAssert.Contains(testType.Name, ex.Message);
            StringAssert.Contains(testInput, ex.Message);
        }

        [Test]
        public void YAXCannotSerializeSelfReferentialTypesLegacyConstructor()
        {
            var testType = typeof(string);
            var ex = Assert.Throws<YAXCannotSerializeSelfReferentialTypes>(() =>
            {
                throw new YAXCannotSerializeSelfReferentialTypes(testType);
            });
            StringAssert.Contains(testType.Name, ex.Message);
        }

        [Test]
        public void YAXObjectTypeMismatchLegacyConstructor()
        {
            var testType = typeof(string);
            var testType2 = typeof(int);
            var ex = Assert.Throws<YAXObjectTypeMismatch>(() =>
            {
                throw new YAXObjectTypeMismatch(testType, testType2);
            });
            StringAssert.Contains(testType.Name, ex.Message);
            StringAssert.Contains(testType2.Name, ex.Message);
        }

        [Test]
        public void YAXPolymorphicExceptionLegacyConstructor()
        {
            var testName = "Test";
            var ex = Assert.Throws<YAXPolymorphicException>(() => { throw new YAXPolymorphicException(testName); });
            StringAssert.Contains(testName, ex.Message);
        }
    }
}