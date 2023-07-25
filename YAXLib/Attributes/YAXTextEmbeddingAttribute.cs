// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Xml.Linq;
using YAXLib.Enums;

namespace YAXLib.Attributes;

/// <summary>
/// Specifies how to de/serialize a string value.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class YAXTextEmbeddingAttribute : YAXBaseAttribute, IYaxMemberLevelAttribute
{
    private static readonly Type[] CompatibleAttributes = {
        typeof(YAXTextEmbeddingAttribute), typeof(YAXDontSerializeAttribute), typeof(YAXDontSerializeIfNullAttribute),
        typeof(YAXSerializeAsAttribute), typeof(YAXCommentAttribute), typeof(YAXSerializableFieldAttribute),
        typeof(YAXNamespaceAttribute), typeof(YAXErrorIfMissedAttribute), typeof(YAXElementOrder),
        typeof(YAXElementForAttribute)
    };

    /// <summary>
    /// Determines how to embed a value of an XML <see cref="XElement" /> or <see cref="XAttribute" />.
    /// </summary>
    /// <param name="embedding">
    /// The kind of <see cref="TextEmbedding" /> to use for the value.
    /// The attribute can be omitted, if embedding is <see cref="TextEmbedding.None" />.
    /// </param>
    public YAXTextEmbeddingAttribute(TextEmbedding embedding)
    {
        Embedding = embedding;
    }

    /// <inheritdoc cref="TextEmbedding" />
    public TextEmbedding Embedding { get; }

    /// <inheritdoc />
    void IYaxMemberLevelAttribute.Setup(MemberWrapper memberWrapper)
    {
        if (memberWrapper.MemberDescriptor.GetCustomAttributes()
            .Where(a => a is YAXBaseAttribute)
            .Select(a => a.GetType())
            .Except(CompatibleAttributes)
            .Any())
        {
            throw new InvalidOperationException(
                $"{nameof(YAXTextEmbeddingAttribute)} can only be combined with {string.Join(", ", CompatibleAttributes.AsEnumerable())}");
        }

        if (memberWrapper.MemberType != typeof(string))
            throw new InvalidOperationException(
                $"Only fields or properties of type string may be decorated with {nameof(YAXTextEmbeddingAttribute)}.");

        memberWrapper.TextEmbedding = Embedding;
    }
}
