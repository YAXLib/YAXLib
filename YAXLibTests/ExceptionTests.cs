using System;
using NUnit.Framework;
using YAXLib;
using YAXLibTests.SampleClasses;

namespace YAXLibTests
{
    class ExceptionTests
    {
        [Test]
        public void YAXAttributeAlreadyExistsExceptionTest()
        {
            var ex = Assert.Throws<YAXAttributeAlreadyExistsException>(() =>
            {
                var serializer = new YAXSerializer(typeof(ClassWithDuplicateYaxAttribute), YAXExceptionHandlingPolicies.ThrowWarningsAndErrors, YAXExceptionTypes.Error);
                serializer.Serialize(ClassWithDuplicateYaxAttribute.GetSampleInstance());
            });

            Assert.AreEqual("test", ex.AttrName);
            StringAssert.Contains("'test'", ex.Message);
        }

        [Test]
        public void YAXBadlyFormedXMLTest()
        {
            var ex = Assert.Throws<YAXBadlyFormedXML>(() =>
            {
                var serializer = new YAXSerializer(typeof(Book), YAXExceptionHandlingPolicies.ThrowWarningsAndErrors, YAXExceptionTypes.Error);
                serializer.Deserialize("some garbage!");
            });

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
                var serializer = new YAXSerializer(typeof(Book), YAXExceptionHandlingPolicies.ThrowWarningsAndErrors, YAXExceptionTypes.Error, YAXSerializationOptions.DisplayLineInfoInExceptions);
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
                var serializer = new YAXSerializer(typeof(Book), YAXExceptionHandlingPolicies.ThrowWarningsAndErrors, YAXExceptionTypes.Error);
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

    }
}
