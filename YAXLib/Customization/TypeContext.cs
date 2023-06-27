// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using YAXLib.Options;
using YAXLib.Pooling.YAXLibPools;

namespace YAXLib.Customization;

/// <summary>
/// The type context provides information about the attributes of a type and type metadata.
/// </summary>
public class TypeContext : ITypeContext
{
    private readonly UdtWrapper _udtWrapper;
    private readonly YAXSerializer _serializer;

    /// <summary>
    /// Creates a new type context instance.
    /// </summary>
    /// <param name="udtWrapper">The <see cref="UdtWrapper" />.</param>
    /// <param name="serializer">The <see cref="YAXSerializer" />.</param>
    internal TypeContext(UdtWrapper udtWrapper, YAXSerializer serializer)
    {
        _udtWrapper = udtWrapper;
        _serializer = serializer;
        Type = _udtWrapper.UnderlyingType;
    }

    /// <inheritdoc />
    public Type Type { get; }

    /// <inheritdoc />
    public IEnumerable<MemberContext> GetFieldsForSerialization()
    {
        return _udtWrapper.GetFieldsForSerialization().Select(member => new MemberContext(member, _serializer));
    }

    /// <inheritdoc />
    public IEnumerable<MemberContext> GetFieldsForDeserialization()
    {
        return _udtWrapper.GetFieldsForDeserialization().Select(member => new MemberContext(member, _serializer));
    }

    /// <inheritdoc />
    public XElement Serialize(object? obj, SerializerOptions? options = null)
    {
        using var serializerPoolObject = SerializerPool.Instance.Get(out var serializer);
        InitializeAsChildSerializer(serializer, options);

        try
        {
            ((IRecursionCounter) _serializer).RecursionCount++;
            return serializer.SerializeToXDocument(obj).Root!;
        }
        finally
        {
            ((IRecursionCounter) _serializer).RecursionCount--;
        }
    }

    /// <inheritdoc />
    public object? Deserialize(XElement element, SerializerOptions? options = null)
    {
        using var serializerPoolObject = SerializerPool.Instance.Get(out var serializer);
        InitializeAsChildSerializer(serializer, options);

        try
        {
            ((IRecursionCounter) _serializer).RecursionCount++;
            return serializer.Deserialize(element);
        }
        finally
        {
            ((IRecursionCounter) _serializer).RecursionCount--;
        }
    }

    /// <summary>
    /// Initialize similar to <see cref="YAXSerializer.InitializeAsChildSerializer" />,
    /// but without overriding namespace and insert location.
    /// </summary>
    private void InitializeAsChildSerializer(YAXSerializer serializer, SerializerOptions? options)
    {
        serializer.Initialize(Type, options ?? _serializer.Options);

        serializer.DocumentDefaultNamespace = _serializer.DocumentDefaultNamespace;
        ((IRecursionCounter) serializer).RecursionCount = ((IRecursionCounter) _serializer).RecursionCount + 1;
    }
}
