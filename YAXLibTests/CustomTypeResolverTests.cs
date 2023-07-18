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
    internal class CustomResolver : ITypeInfoResolver
    {
        public IList<IMemberInfo> ResolveMembers(IList<IMemberInfo> proposedMembers, Type type, SerializerOptions options)
        {
            return proposedMembers.Where(member => !string.Equals("PublishYear", member.Name, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public string GetTypeName(string proposedName, Type type, SerializerOptions serializerOptions)
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
                TypeInfoResolver = new CustomResolver()
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
