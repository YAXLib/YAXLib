// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

#nullable enable
using System;
using System.Reflection;
using YAXLib.Options;

namespace YAXLib
{
    /// <summary>
    /// Provides information about which <see cref="Type"/>s and/or members being serialized or deserialized.
    /// </summary>
    public interface ISerializationContext
    {
        /// <summary>
        /// The member's <see cref="FieldInfo"/> for property serialization, else <see langword="null"/>.
        /// </summary>
        FieldInfo? FieldInfo { get; }

        /// <summary>
        /// The member's <see cref="MemberInfo"/> for member serialization, else <see langword="null"/>.
        /// </summary>
        MemberInfo? MemberInfo { get; }

        /// <summary>
        /// The member's <see cref="PropertyInfo"/> for property serialization, else <see langword="null"/>.
        /// </summary>
        PropertyInfo? PropertyInfo { get; }

        /// <summary>
        /// The member's <see cref="Type"/> for member serialization, else <see langword="null"/>.
        /// </summary>
        Type? MemberType { get; }

        /// <summary>
        /// The class' <see cref="Type"/> for class level serialization, else <see langword="null"/>.
        /// </summary>
        Type? ClassType { get; }

        /// <summary>
        /// Gets the <see cref="Options.SerializerOptions"/> of the <see cref="YAXSerializer"/> instance.
        /// </summary>
        SerializerOptions SerializerOptions { get; }

        /// <inheritdoc cref="IRecursionCounter.RecursionCount"/>
        int RecursionCount { get; }
    }
}
