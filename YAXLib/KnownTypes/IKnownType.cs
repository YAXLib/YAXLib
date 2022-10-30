// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib.Customization;

namespace YAXLib.KnownTypes;

/// <summary>
/// Interface for all known type classes.
/// </summary>
public interface IKnownType
{
    /// <summary>
    /// Gets the underlying known type.
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Serializes the object into the specified XML element.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="elem">The XML element.</param>
    /// <param name="overridingNamespace">The namespace the element belongs to.</param>
    /// <param name="serializationContext">Contains information about the type and members of the <paramref name="obj" />.</param>
    void Serialize(object? obj, XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext);

    /// <summary>
    /// Deserializes the specified XML element to the known type.
    /// </summary>
    /// <param name="elem">The XML element to deserialize object from.</param>
    /// <param name="overridingNamespace">The namespace the element belongs to.</param>
    /// <param name="serializationContext">Contains information about the type and members of the object to deserialize.</param>
    /// <returns>The deserialized object</returns>
    object? Deserialize(XElement elem, XNamespace overridingNamespace, ISerializationContext serializationContext);
}