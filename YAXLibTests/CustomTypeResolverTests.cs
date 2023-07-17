// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using YAXLib;
using YAXLib.Options;
using YAXLibTests.SampleClasses;

namespace YAXLibTests;
public class CustomTypeResolverTests
{
    internal class CustomResolver : ITypeResolver
    {
        public IList<IMemberInfo> ResolveMembers(IList<IMemberInfo> sourceMembers, Type underlyingType, SerializerOptions options)
        {
            return sourceMembers.Where(member => !string.Equals("PublishYear", member.Name, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public string GetTypeName(string proposedName, Type udtType, SerializerOptions serializerOptions)
        {
            return proposedName;
        }
    }

    [Test]
    public void SkipPublishYearPropertySerializationTest()
    {
        var serializer = new YAXSerializer<Book>(
            new SerializerOptions
            {
                TypeResolver = new CustomResolver()
            });

        var result = serializer.Serialize(Book.GetSampleInstance());

        Assert.AreEqual(
            """
            <!-- This example demonstrates serializing a very simple class -->
            <Book>
              <Title>Inside C#</Title>
              <Author>Tom Archer &amp; Andrew Whitechapel</Author>
              <Price>30.5</Price>
            </Book>
            """, result);
    }
}
