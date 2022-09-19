// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib.Customization;

namespace YAXLib.KnownTypes;

/// <summary>
/// Abstract base class for predefined serializers and deserializers for known base types.
/// Classes derived from the base class of type <typeparamref name="T" /> will be serialized and deserialized.
/// </summary>
/// <typeparam name="T">The underlying known type</typeparam>
public abstract class KnownBaseTypeBase<T> : IKnownType
{
    /// <inheritdoc />
    public Type Type { get; } = typeof(T);

    /// <inheritdoc />
    void IKnownType.Serialize(object? obj, XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        Serialize((T?) obj, elem, overridingNamespace, serializationContext);
    }

    /// <inheritdoc />
    object? IKnownType.Deserialize(XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        return Deserialize(elem, overridingNamespace, serializationContext);
    }

    /// <summary>
    /// Serializes the object into the specified XML element.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="elem">The XML element.</param>
    /// <param name="overridingNamespace">The namespace the element belongs to.</param>
    /// <param name="serializationContext">The serialization context.</param>
    public abstract void Serialize(T? obj, XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext);

    /// <summary>
    /// Deserializes the specified XML element to the known type.
    /// </summary>
    /// <param name="elem">The XML element to deserialize object from.</param>
    /// <param name="overridingNamespace">The namespace the element belongs to.</param>
    /// <param name="serializationContext">The serialization context.</param>
    /// <returns>The deserialized object</returns>
    public abstract T? Deserialize(XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext);
}