// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using YAXLib.Attributes;
using YAXLib.Options;

namespace YAXLib;

/// <summary>
/// Provides type-specific information during serialization/deserialization.
/// <para/>
/// It is recommended to derive a custom <see cref="ITypeInspector"/> from the <see cref="DefaultTypeInspector"/>.
/// <see cref="DefaultTypeInspector.GetMembers"/> will then return the default members for further processing.
/// <see cref="DefaultTypeInspector.GetTypeName"/> lets you define the type names.
/// customization.
/// </summary>
public interface ITypeInspector
{
    /// <summary>
    /// Retrieves the members to be serialized for the given type.
    /// </summary>
    /// <param name="type">The type for which to retrieve the member information.</param>
    /// <param name="options">Serializer options for controlling the serialization process.</param>
    /// <param name="includePrivateMembersFromBaseTypes" cref="YAXSerializableTypeAttribute.IncludePrivateMembersFromBaseTypes">
    /// Specifies whether to include private members from base types in the collection.
    /// </param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IMemberDescriptor"/> containing the member information for the specified type.</returns>
    IEnumerable<IMemberDescriptor> GetMembers(Type type, SerializerOptions options, bool includePrivateMembersFromBaseTypes);

    /// <summary>
    /// Gets the custom type name for the given type during serialization.
    /// </summary>
    /// <param name="type">The type for which to retrieve the type name.</param>
    /// <param name="options">Serializer options for controlling the serialization process.</param>
    /// <returns>A string representing the type name for the specified type.</returns>
    string GetTypeName(Type type, SerializerOptions options);
}
