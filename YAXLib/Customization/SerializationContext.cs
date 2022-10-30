// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Options;

namespace YAXLib.Customization;

/// <inheritdoc />
internal class SerializationContext : ISerializationContext
{
    /// <summary>
    /// Creates a new serialization context instance.
    /// </summary>
    /// <param name="memberWrapper"></param>
    /// <param name="udtWrapper"></param>
    /// <param name="serializer"></param>
    public SerializationContext(MemberWrapper? memberWrapper, UdtWrapper udtWrapper, YAXSerializer serializer)
    {
        SerializerOptions = serializer.Options;
        RecursionCount = ((IRecursionCounter) serializer).RecursionCount;

        // Class level serialization
        TypeContext = new TypeContext(udtWrapper, serializer);

        if (memberWrapper == null) return;

        // Member level serialization
        MemberContext = new MemberContext(memberWrapper, serializer);
    }

    /// <inheritdoc />
    public IMemberContext? MemberContext { get; }

    /// <inheritdoc />
    public ITypeContext TypeContext { get; }

    /// <inheritdoc />
    public SerializerOptions SerializerOptions { get; }

    /// <inheritdoc cref="IRecursionCounter.RecursionCount" />
    public int RecursionCount { get; }
}
