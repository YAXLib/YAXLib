// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

#nullable enable
using System;
using System.Reflection;
using YAXLib.Options;

namespace YAXLib;

/// <inheritdoc/>
internal class SerializationContext : ISerializationContext
{
    /// <summary>
    /// Creates a new serialization context instance.
    /// </summary>
    /// <param name="memberWrapper"></param>
    /// <param name="udtWrapper"></param>
    /// <param name="serializer"></param>
    public SerializationContext(MemberWrapper? memberWrapper, UdtWrapper? udtWrapper, YAXSerializer serializer)
    {
        SerializerOptions = serializer.Options;
        RecursionCount = ((IRecursionCounter) serializer).RecursionCount;

        // Class level serialization
        ClassType = udtWrapper?.UnderlyingType;
        
        if (memberWrapper == null) return;
        
        // Member level serialization
        FieldInfo = memberWrapper.FieldInfo;
        MemberInfo = memberWrapper.MemberInfo;
        PropertyInfo = memberWrapper.PropertyInfo;
        MemberType = memberWrapper.MemberType;
    }

    /// <inheritdoc/>
    public FieldInfo? FieldInfo { get; }

    /// <inheritdoc/>
    public MemberInfo? MemberInfo { get; }

    /// <inheritdoc/>
    public PropertyInfo? PropertyInfo { get; }

    /// <inheritdoc/>
    public Type? MemberType { get; }

    /// <inheritdoc/>
    public Type? ClassType { get; }

    /// <inheritdoc/>
    public SerializerOptions SerializerOptions { get; }

    /// <inheritdoc cref="IRecursionCounter.RecursionCount"/>
    public int RecursionCount { get; }
}
