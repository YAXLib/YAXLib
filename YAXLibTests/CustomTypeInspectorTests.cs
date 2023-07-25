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
public class CustomTypeInspectorTests
{
    internal class CustomTypeInspector : DefaultTypeInspector
    {
        public override IEnumerable<IMemberDescriptor> GetMembers(Type type, SerializerOptions options, bool includePrivateMembersFromBaseTypes)
        {
            var members = base.GetMembers(type, options, includePrivateMembersFromBaseTypes);
            return members.Where(member => !string.Equals("PublishYear", member.Name, StringComparison.OrdinalIgnoreCase));
        }
    }

    [Test]
    public void SkipPublishYearPropertySerializationTest()
    {
        var serializer = new YAXSerializer<Book>(
            new SerializerOptions
            {
                TypeInspector = new CustomTypeInspector()
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
