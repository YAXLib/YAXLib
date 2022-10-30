// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using YAXLib.Options;

namespace YAXLib.Customization;

/// <summary>
/// Provides information about <see cref="Type" />s and/or its members being serialized or deserialized.
/// </summary>
public interface ISerializationContext
{
    /// <inheritdoc cref="ITypeContext" />
    ITypeContext TypeContext { get; }

    /// <inheritdoc cref="IMemberContext" />
    IMemberContext? MemberContext { get; }

    /// <summary>
    /// Gets the <see cref="Options.SerializerOptions" /> of the <see cref="YAXSerializer" /> instance.
    /// </summary>
    SerializerOptions SerializerOptions { get; }

    /// <inheritdoc cref="IRecursionCounter.RecursionCount" />
    int RecursionCount { get; }
}