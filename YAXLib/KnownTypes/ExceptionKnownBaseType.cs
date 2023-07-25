// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;
using YAXLib.Customization;
using YAXLib.Enums;
using YAXLib.Options;

namespace YAXLib.KnownTypes;

/// <summary>
/// Class for serialization and deserialization of <see cref="Exception" />s and its derived classes.
/// <para>
/// We <b>must not</b> use a custom serializer for Exception types, as this known type class is expected
/// to process exceptions. Doing different may lead to an infinite loop.
/// </para>
/// <para>
/// <see cref="YAXSerializationOptions.SuppressMetadataAttributes" /> <b>must not</b> be set during
/// serialization, when deserialization of exceptions is intended.
/// </para>
/// </summary>
internal class ExceptionKnownBaseType : KnownBaseTypeBase<Exception>
{
    private int _recursionCount;
    private int _maxRecursion;

    /// <inheritdoc />
    public override void Serialize(Exception? obj, XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        _recursionCount = serializationContext.RecursionCount;
        _maxRecursion = serializationContext.SerializerOptions.MaxRecursion;
        Serialize(obj, elem, overridingNamespace, serializationContext, false);
    }

    /// <summary>
    /// Serialize an <see cref="Exception" />.
    /// May be called recursively
    /// </summary>
    private void Serialize(Exception? obj, XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext, bool isRecursive)
    {
        if (obj == null) return;

        if (isRecursive && _maxRecursion < _recursionCount) return;

        foreach (var member in serializationContext.TypeContext.GetFieldsForSerialization())
        {
            if (!ReflectionUtils.IsBaseClassOrSubclassOf(member.TypeContext.Type, "System.Exception"))
            {
                SerializeNonExceptionField(obj, member, elem, overridingNamespace);
            }
            else
            {
                SerializeExceptionField(obj, member, elem, overridingNamespace, serializationContext);
            }
        }
    }

    private void SerializeExceptionField(Exception obj, MemberContext member, XElement elem,
        XNamespace overridingNamespace, ISerializationContext serializationContext)
    {
        var value = member.GetValue(obj); // value may be null

        var exceptionElement = member.TypeContext.Serialize(value, new SerializerOptions { MaxRecursion = 1 });

        exceptionElement.Name = member.MemberDescriptor.Name == nameof(Exception.InnerException)
            ? XName.Get(member.MemberDescriptor.Name)
            : XName.Get(nameof(Exception));

        if (value is Exception exceptionValue)
        {
            _recursionCount++;
            Serialize(exceptionValue, exceptionElement, overridingNamespace, serializationContext, true);
            _recursionCount--;
        }

        elem.Add(exceptionElement);
    }

    private static void SerializeNonExceptionField(Exception obj, MemberContext member, XElement elem,
        XNamespace overridingNamespace)
    {
        var value = member.GetValue(obj);

        if (value == null || ReflectionUtils.IsBasicType(value.GetType()))
        {
            elem.Add(new XElement(XName.Get(member.MemberDescriptor.Name), overridingNamespace, value));
        }
        else if (member.MemberDescriptor.Name == nameof(Exception.TargetSite))
        {
            elem.Add(new XElement(XName.Get(member.MemberDescriptor.Name), overridingNamespace, value));
        }
        else
        {
            // The serializer does not call this method recursively
            var parent = new XElement(XName.Get(member.MemberDescriptor.Name), overridingNamespace);
            var element = member.TypeContext.Serialize(value, new SerializerOptions { MaxRecursion = 4 }).Document!.Root;
            if (!XMLUtils.IsElementCompletelyEmpty(element)) parent.Add(element);
            elem.Add(parent);
        }
    }

    /// <inheritdoc />
    public override Exception? Deserialize(XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext)
    {
        _recursionCount = serializationContext.RecursionCount;
        _maxRecursion = serializationContext.SerializerOptions.MaxRecursion;
        return Deserialize(elem, overridingNamespace, serializationContext, false);
    }

    /// <summary>
    /// Deserialize an <see cref="XElement" /> containing exception details.
    /// May be called recursively
    /// </summary>
    private Exception? Deserialize(XElement elem, XNamespace overridingNamespace,
        ISerializationContext serializationContext, bool isRecursive)
    {
        if (isRecursive && _maxRecursion < _recursionCount) return null;

        if (elem.Name.LocalName != nameof(Exception) &&
            elem.Name.LocalName != nameof(Exception.InnerException)) return null;

        if (!TryGetRealType(elem, serializationContext, out var exceptionType)) return null;

        if (Activator.CreateInstance(exceptionType!, Array.Empty<object>()) is not Exception exInstance)
            return null;

        if (TryGetChildElement(elem, XName.Get(nameof(Exception.Message)), out var child))
        {
            ReflectionUtils.SetFieldValue(exInstance, "_message", child.Value);
        }

        if (TryGetChildElement(elem, XName.Get(nameof(Exception.InnerException)), out child))
        {
            // Recursive call to the current method
            _recursionCount++;
            var innerException = Deserialize(child, overridingNamespace, serializationContext, true);
            ReflectionUtils.SetFieldValue(exInstance, "_innerException", innerException);
            _recursionCount--;
        }

        return exInstance;
    }

    private static bool TryGetChildElement(XElement elem, XName name, out XElement child)
    {
        child = new XElement(name);
        var c = elem.Element(name);
        if (c != null) child = c;

        return true;
    }

    private static bool TryGetRealType(XElement elem, ISerializationContext serializationContext,
        out Type? exceptionType)
    {
        exceptionType = null;
        var realTypeAttr = elem.Attribute_NamespaceSafe(
            serializationContext.SerializerOptions.Namespace.Uri +
            serializationContext.SerializerOptions.AttributeName.RealType, XNamespace.None);

        if (realTypeAttr == null || string.IsNullOrEmpty(realTypeAttr.Value)) return false;
        exceptionType = ReflectionUtils.GetTypeByName(realTypeAttr.Value);

        return exceptionType != null;
    }
}
