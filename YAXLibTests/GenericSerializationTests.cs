// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Globalization;
using NUnit.Framework;
using YAXLib;
using YAXLibTests.SampleClasses;

namespace YAXLibTests
{
    [TestFixture]
    public class GenericSerializationTests
    {
        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
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
