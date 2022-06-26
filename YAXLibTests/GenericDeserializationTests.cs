// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using YAXLib;
using YAXLibTests.SampleClasses;

namespace YAXLibTests
{
    [TestFixture]
    public class GenericDeserializationTests
    {
        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [Test]
        public void GenericDeserializationTest()
        {
            const string xml =
                @"<!-- This example demonstrates serailizing a very simple class -->
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
