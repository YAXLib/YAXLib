// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using YAXLib.Options;

namespace YAXLib.Customization;

/// <summary>
/// The type context interface provides information about the attributes of a type and type metadata.
/// </summary>
public interface ITypeContext
{
    /// <summary>
    /// The type's <see cref="System.Type" /> for serialization and deserialization.
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Gets the <see cref="MemberContext" /> for each serializable field of the <see cref="Type" />.
    /// <para>
    /// Fields are filtered according to settings and attributes (if any),
    /// and ordered by the <see cref="Attributes.YAXElementOrder" /> attribute (if any).
    /// </para>
    /// </summary>
    /// <returns>An <see cref="IEnumerable" /> of the fields' <see cref="MemberContext" />.</returns>
    IEnumerable<MemberContext> GetFieldsForSerialization();

    /// <inheritdoc cref="TypeContext.GetFieldsForSerialization" />
    IEnumerable<MemberContext> GetFieldsForDeserialization();

    /// <summary>
    /// Serializes the <paramref name="obj" /> of type <see cref="Type" /> to an <see cref="XElement" />.
    /// <para>
    /// The calling <see cref="KnownTypes.IKnownType" /> or <see cref="ICustomSerializer{T}" /> will
    /// never be invoked recursively. Instead, the default serialization is executed.
    /// </para>
    /// </summary>
    /// <param name="obj">The object of type <see cref="Type" /> to serialize.</param>
    /// <param name="options">
    /// The <see cref="SerializerOptions" /> or <see langword="null" /> to take options from the parent
    /// <seealso cref="YAXSerializer" />.
    /// </param>
    /// <returns>The <see cref="XElement" /> representation of the object graph.</returns>
    XElement Serialize(object obj, SerializerOptions? options = null);

    /// <summary>
    /// Deserializes the <paramref name="element" /> to a new instance of <see cref="Type" />.
    /// <para>
    /// The calling <see cref="KnownTypes.IKnownType" /> or <see cref="ICustomSerializer{T}" /> will
    /// never be invoked recursively. Instead, the default deserialization is executed.
    /// </para>
    /// </summary>
    /// <param name="element">The object of type <see cref="Type" /> to serialize.</param>
    /// <param name="options">
    /// The <see cref="SerializerOptions" /> or <see langword="null" /> to take options from the parent
    /// <seealso cref="YAXSerializer" />.
    /// </param>
    /// <returns>A new instance of <see cref="Type" /> created from the <paramref name="element" /></returns>
    object? Deserialize(XElement element, SerializerOptions? options = null);
}
