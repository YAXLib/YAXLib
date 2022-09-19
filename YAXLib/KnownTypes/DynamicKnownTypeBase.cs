// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib.Customization;

namespace YAXLib.KnownTypes;

/// <summary>
/// A <see cref="DynamicKnownTypeBase" /> is the base class for <see cref="Type" />s,
/// for which the full type name is defined at compile time.
/// The actual type is determined at runtime.
/// </summary>
public abstract class DynamicKnownTypeBase : IKnownType
{
    private Type? _type;

    /// <summary>
    /// Gets or sets the name of the <seealso cref="Type" />.
    /// </summary>
    public abstract string TypeName { get; }

    /// <inheritdoc />
    public Type Type => _type ??= ReflectionUtils.GetTypeByName(TypeName) ??
                                  throw new InvalidOperationException($"Type for name '{TypeName}' not found.");

    /// <inheritdoc />
    public abstract void Serialize(object? obj, XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext);

    /// <inheritdoc />
    public abstract object? Deserialize(XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext);
}