// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Reflection;
using System.Xml.Linq;

namespace YAXLib.Customization;

/// <summary>
/// A wrapper around an <see cref="ICustomSerializer{T}" />.
/// It's methods are invoked with <see cref="System.Reflection" />.
/// </summary>
internal class CustomSerializerWrapper : ICustomSerializer<object>
{
    private readonly object _csInstance;

    /// <summary>
    /// The custom serializer type.
    /// </summary>
    public Type Type { get; }

    public CustomSerializerWrapper(Type csType)
    {
        Type = csType;
        // Create the instance by invoking a public or non-public constructor
        _csInstance = Activator.CreateInstance(csType, true) ?? throw new InvalidOperationException($"Can't create instance for type '{Type.Name}'");
    }

    /// <inheritdoc />
    public void SerializeToAttribute(object? objectToSerialize, XAttribute attrToFill,
        ISerializationContext serializationContext)
    {
        using var _ = new Locker(Type);

        Type.InvokeMember(nameof(ICustomSerializer<object>.SerializeToAttribute),
            BindingFlags.InvokeMethod, null, _csInstance,
            new[] {
                objectToSerialize, attrToFill, serializationContext
            });
    }

    /// <inheritdoc />
    public void SerializeToElement(object? objectToSerialize, XElement elemToFill,
        ISerializationContext serializationContext)
    {
        using var _ = new Locker(Type);

        Type.InvokeMember(nameof(ICustomSerializer<object>.SerializeToElement),
            BindingFlags.InvokeMethod, null, _csInstance,
            new[] {
                objectToSerialize, elemToFill, serializationContext
            });
    }

    /// <inheritdoc />
    public string SerializeToValue(object? objectToSerialize, ISerializationContext serializationContext)
    {
        using var _ = new Locker(Type);

        return (string) Type.InvokeMember(nameof(ICustomSerializer<object>.SerializeToValue),
            BindingFlags.InvokeMethod, null,
            _csInstance,
            new[] { objectToSerialize, serializationContext })!;
    }

    /// <inheritdoc />
    public object DeserializeFromAttribute(XAttribute attribute, ISerializationContext serializationContext)
    {
        using var _ = new Locker(Type);

        return Type.InvokeMember(nameof(ICustomSerializer<object>.DeserializeFromAttribute),
            BindingFlags.InvokeMethod, null, _csInstance,
            new object[] { attribute, serializationContext })!;
    }

    /// <inheritdoc />
    public object DeserializeFromElement(XElement element, ISerializationContext serializationContext)
    {
        using var _ = new Locker(Type);

        return Type.InvokeMember(nameof(ICustomSerializer<object>.DeserializeFromElement),
            BindingFlags.InvokeMethod, null, _csInstance,
            new object[] { element, serializationContext })!;
    }

    /// <inheritdoc />
    public object DeserializeFromValue(string value, ISerializationContext serializationContext)
    {
        using var _ = new Locker(Type);

        return Type.InvokeMember(nameof(ICustomSerializer<object>.DeserializeFromValue),
            BindingFlags.InvokeMethod, null, _csInstance,
            new object[] { value, serializationContext })!;
    }
}
