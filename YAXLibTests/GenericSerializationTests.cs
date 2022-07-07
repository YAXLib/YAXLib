// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Globalization;
using NUnit.Framework;
using YAXLib;
using YAXLib.Options;
using YAXLibTests.SampleClasses;
using YAXLibTests.TestHelpers;

namespace YAXLibTests
{
    [TestFixture]
    public class GenericSerializationTests : SerializationTestBase
    {
        protected override IYAXSerializer<object> CreateSerializer<T>(SerializerOptions options = null)
        {
            return new GenericSerializerWrapper<T>(new YAXSerializer<T>(options ?? new SerializerOptions()));
        }

        protected override YAXSerializer CreateSerializer(Type type, SerializerOptions options = null)
        {
            return new YAXSerializer(type, options ?? new SerializerOptions());
        }

        [Test]
        public void GenericSerializationTest()
        {
            const string result =
                @"<!-- This example demonstrates serializing a very simple class -->
<Book>
  <Title>Inside C#</Title>
  <Author>Tom Archer &amp; Andrew Whitechapel</Author>
  <PublishYear>2002</PublishYear>
  <Price>30.5</Price>
</Book>";
            var serializer = new YAXSerializer<Book>();
            var got = serializer.Serialize(Book.GetSampleInstance());
            Assert.That(got, Is.EqualTo(result));
        }

        [Test]
        public void GenericDeserializationTest()
        {
            const string xml =
                @"
<Book>
  <Title>Inside C#</Title>
  <Author>Tom Archer &amp; Andrew Whitechapel</Author>
  <PublishYear>2002</PublishYear>
  <Price>30.5</Price>
</Book>";
            var serializer = new YAXSerializer<Book>();
            var got = serializer.Deserialize(xml);
            Assert.NotNull(got);
            Assert.AreEqual(got, Book.GetSampleInstance());
        }
    }
}
