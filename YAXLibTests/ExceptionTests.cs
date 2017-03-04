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
        public void YAXAttributeAlreadyExistsExceptionTest2()
        {
            Assert.Throws<YAXAttributeAlreadyExistsException>(() =>
            {
                var serializer = new YAXSerializer(typeof(ClassWithDuplicateYaxAttribute2), YAXExceptionHandlingPolicies.ThrowErrorsOnly);
                serializer.Serialize(new ClassWithDuplicateYaxAttribute2());
            });
        }

    }
}
