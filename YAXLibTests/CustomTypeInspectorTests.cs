// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using YAXLib;
using YAXLib.Enums;
using YAXLib.Options;
using YAXLibTests.SampleClasses;

namespace YAXLibTests;
public class CustomTypeInspectorTests
{
    internal class CustomTypeInspector : DefaultTypeInspector
    {
        public override IEnumerable<IMemberDescriptor> GetMembers(Type type, SerializerOptions options, bool includePrivateMembersFromBaseTypes)
        {
            if (base.GetTypeName(type, options) == nameof(Book))
                return GetMembersForBookType(type, options, includePrivateMembersFromBaseTypes);

            // Different handling for other types could be added here
            
            return base.GetMembers(type, options, includePrivateMembersFromBaseTypes);
        }

        private IEnumerable<IMemberDescriptor> GetMembersForBookType(Type type, SerializerOptions options, bool includePrivateMembersFromBaseTypes)
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

        Assert.That(
result, Is.EqualTo("""
            <!-- This example demonstrates serializing a very simple class -->
            <Book>
              <Title>Inside C#</Title>
              <Author>Tom Archer &amp; Andrew Whitechapel</Author>
              <Price>30.5</Price>
            </Book>
            """));
    }

    [Test]
    public void SkipPublishYearPropertyDeserializationTest()
    {
        var serializer = new YAXSerializer<Book>(
            new SerializerOptions
            {
                TypeInspector = new CustomTypeInspector()
            });

        var result = serializer.Deserialize("""
            <!-- This example demonstrates serializing a very simple class -->
            <Book>
              <Title>Inside C#</Title>
              <Author>Tom Archer &amp; Andrew Whitechapel</Author>
              <Price>30.5</Price>
              <!-- Won't be deserialized -->
              <PublishYear>2023</PublishYear>
            </Book>
            """);

        // The PublishYear property is not deserialized, so the property has its default value
        Assert.That(result!.PublishYear, Is.EqualTo(0));
    }
}
