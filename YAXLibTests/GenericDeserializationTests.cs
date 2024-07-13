// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using NUnit.Framework;
using YAXLib;
using YAXLib.Options;
using YAXLib.Pooling.YAXLibPools;
using YAXLibTests.SampleClasses;
using YAXLibTests.TestHelpers;

namespace YAXLibTests;

[TestFixture]
public class GenericDeserializationTests : DeserializationTestBase
{
    [OneTimeSetUp]
    public void TestFixtureSetup()
    {
        // Clear the pool for tests in here
        SerializerPool.Instance.Clear();
    }

    [OneTimeTearDown]
    public void TestFixtureFinalize()
    {
        Console.WriteLine(
            $"{nameof(SerializerPool.Instance.Pool.CountAll)}: {SerializerPool.Instance.Pool.CountAll}");
        Console.WriteLine(
            $"{nameof(SerializerPool.Instance.Pool.CountActive)}: {SerializerPool.Instance.Pool.CountActive}");
        Console.WriteLine(
            $"{nameof(SerializerPool.Instance.Pool.CountInactive)}: {SerializerPool.Instance.Pool.CountInactive}");
    }

    protected override IYAXSerializer<object> CreateSerializer<T>(SerializerOptions? options = null)
    {
        return new GenericSerializerWrapper<T>(new YAXSerializer<T>(options ?? new SerializerOptions()));
    }

    protected override YAXSerializer CreateSerializer(Type type, SerializerOptions? options = null)
    {
        return new YAXSerializer(type, options ?? new SerializerOptions());
    }

    [Test]
    public void GenericDeserializationTest()
    {
        const string xml =
            """
                <!-- This example demonstrates serailizing a very simple class -->
                <Book>
                  <Title>Inside C#</Title>
                  <Author>Tom Archer &amp; Andrew Whitechapel</Author>
                  <PublishYear>2002</PublishYear>
                  <Price>30.5</Price>
                </Book>
                """;
        var serializer = new YAXSerializer<Book>();
        var got = serializer.Deserialize(xml);
        Assert.Multiple(() =>
        {
            Assert.That(got, Is.Not.Null);
            Assert.That(Book.GetSampleInstance(), Is.EqualTo(got));
        });
    }
}
