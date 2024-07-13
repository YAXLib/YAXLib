// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using YAXLib.Attributes;
using YAXLib.Caching;
using YAXLib.Customization;
using YAXLib.Enums;
using YAXLib.Exceptions;

namespace YAXLib;

internal class Deserialization
{
    private readonly YAXSerializer _serializer;

    private const string KeyPropName = "Key";
    private const string ValuePropName = "Value";
    private static readonly char[] WhiteSpaceChars = [' ', '\t', '\r', '\n'];

    /// <summary>
    /// Reference to a preassigned deserialization base object
    /// Will always be set by the serializer using <see cref="SetDeserializationBaseObject(object?)" /> when deserialization
    /// starts.
    /// </summary>
    private object? _deserializationObject;

    /// <summary>
    /// Specifies whether an exception is occurred during the de-serialization of the current member
    /// </summary>
    private bool _exceptionOccurredDuringMemberDeserialization;

    /// <summary>
    /// This instance will be (re-) initialized it a way
    /// that it has the same virgin state like an instance that
    /// was created with one of the CTORs.
    /// </summary>
    internal void Initialize()
    {
        _deserializationObject = null;
        _exceptionOccurredDuringMemberDeserialization = false;
        IsCreatedToDeserializeANonCollectionMember = false;
        RemoveDeserializedXmlNodes = false;
    }

    public Deserialization(YAXSerializer serializer)
    {
        _serializer = serializer;
    }

    /// <summary>
    /// Sets the object used as the base object in the next stage of deserialization.
    /// This method enables multi-stage deserialization for YAXLib.
    /// </summary>
    /// <param name="obj">The object used as the base object in the next stage of deserialization.</param>
    public void SetDeserializationBaseObject(object? obj)
    {
        if (obj != null && !_serializer.Type.IsInstanceOfType(obj))
            throw new YAXObjectTypeMismatch(_serializer.Type, obj.GetType());

        _deserializationObject = obj;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is created to deserialize a non collection member of another
    /// object.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if this instance is created to deserialize a non collection member of another object;
    /// otherwise,
    /// <see langword="false" />.
    /// </value>
    private bool IsCreatedToDeserializeANonCollectionMember { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether XML elements or attributes should be removed after being deserialized
    /// </summary>
    private bool RemoveDeserializedXmlNodes { get; set; }

    /// <summary>
    /// The basic method which performs the whole job of deserialization.
    /// </summary>
    /// <param name="baseElement">The element to be deserialized.</param>
    /// <returns>object containing the deserialized data</returns>
    internal object? DeserializeBase(XElement? baseElement)
    {
        _serializer.IsSerializing = false;

        if (baseElement == null) return _deserializationObject;

        ProcessRealTypeAttribute(baseElement);

        // HasCustomSerializer must be tested AFTER analyzing any RealType attribute
        if (_serializer.UdtWrapper.HasCustomSerializer &&
            !Locker.IsLocked(_serializer.UdtWrapper.CustomSerializer!.Type))
            return _serializer.UdtWrapper.CustomSerializer!.DeserializeFromElement(baseElement,
                new SerializationContext(null, _serializer.UdtWrapper, _serializer));

        // Deserialize objects with special treatment

        if (_serializer.Type.IsGenericType && _serializer.Type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            return DeserializeKeyValuePair(baseElement);

        if (_serializer.UdtWrapper.IsKnownType && !Locker.IsLocked(_serializer.UdtWrapper.KnownType!.Type))
        {
            using var _ = new Locker(_serializer.UdtWrapper.KnownType!.Type);
            return _serializer.UdtWrapper.KnownType!.Deserialize(baseElement, _serializer.TypeNamespace,
                new SerializationContext(null, _serializer.UdtWrapper, _serializer));
        }

        if (TryDeserializeAsDictionary(baseElement, out var resultObject))
            return resultObject;

        if (TryDeserializeAsCollection(baseElement, out resultObject))
            return resultObject;

        if (ReflectionUtils.IsBasicType(_serializer.Type))
            return ReflectionUtils.ConvertBasicType(baseElement.Value, _serializer.Type, _serializer.Options.Culture);

        // Run the default deserialization algorithm
        return DeserializeDefault(baseElement);
    }

    /// <summary>
    /// Called when deserialization exception occurs. It applies the exception handling policies.
    /// </summary>
    /// <param name="ex">The exception that has occurred.</param>
    /// <param name="exceptionType">Type of the exception.</param>
    internal void OnExceptionOccurred(YAXException ex, YAXExceptionTypes exceptionType)
    {
        _exceptionOccurredDuringMemberDeserialization = true;
        if (exceptionType == YAXExceptionTypes.Ignore) return;

        _serializer.ParsingErrors.AddException(ex, exceptionType);
        if (_serializer.Options.ExceptionHandlingPolicies == YAXExceptionHandlingPolicies.ThrowWarningsAndErrors ||
            _serializer.Options.ExceptionHandlingPolicies == YAXExceptionHandlingPolicies.ThrowErrorsOnly &&
            exceptionType == YAXExceptionTypes.Error)
            throw ex;
    }

    /// <summary>
    /// The default serialization algorithm, which deserializes per field and
    /// from value, from attribute, from XML element,
    /// from custom serializers and known types.
    /// </summary>
    /// <param name="baseElement"></param>
    /// <returns>The deserialized object</returns>
    private object DeserializeDefault(XElement baseElement)
    {
        var resultObject = _deserializationObject ?? Activator.CreateInstance(_serializer.Type, Array.Empty<object>());
        if (resultObject == null) return null!;

        foreach (var member in _serializer.UdtWrapper.GetFieldsForDeserialization())
        {
            // reset handled exceptions status
            _exceptionOccurredDuringMemberDeserialization = false;

            /*
             * The following values will be set NOT NULL unless an exception occurred.
             * With exceptions, _exceptionOccurredDuringMemberDeserialization will be true
             */

            // The element value gathered at the first phase - must not be NULL at this time
            var deserializedRawValue = string.Empty;
            // The XElement instance gathered at the first phase
            XElement? xElementValue = null;
            // The XAttribute instance gathered at the first phase
            XAttribute? xAttributeValue = null;

            var isHelperElementCreated = false;

            var serializationLocation = member.SerializationLocation;

            if (member.IsSerializedAsAttribute)
            {
                deserializedRawValue = DeserializeFromAttribute(baseElement, ref xElementValue!, ref xAttributeValue!,
                    serializationLocation, member);
            }
            else if (member.IsSerializedAsValue)
            {
                deserializedRawValue =
                    DeserializeFromValue(baseElement, ref xElementValue!, serializationLocation, member);
            }
            else
            {
                if (DeserializeFromXmlElement(baseElement, serializationLocation, member, resultObject,
                        ref deserializedRawValue, ref isHelperElementCreated, ref xElementValue))
                    continue;
            }

            // Phase 2: Now try to retrieve deserializedValue,
            // based on values gathered in xElementValue, xAttributeValue, and deserializedValue
            // Each will be set according to their purpose (attribute, element, value)
            if (_exceptionOccurredDuringMemberDeserialization)
            {
                _ = TrySetDefaultValue(baseElement, resultObject, xAttributeValue, xElementValue, member);
            }
            else if (member.HasCustomSerializer && !Locker.IsLocked(member.CustomSerializer!.Type))
            {
                InvokeCustomDeserializer(member.CustomSerializer, baseElement, deserializedRawValue, xElementValue,
                    xAttributeValue,
                    resultObject, member);
            }
            else if (member.UdtWrapper.HasCustomSerializer &&
                     !Locker.IsLocked(member.UdtWrapper.CustomSerializer!.Type))
            {
                InvokeCustomDeserializer(member.UdtWrapper.CustomSerializer, baseElement, deserializedRawValue,
                    xElementValue, xAttributeValue,
                    resultObject, member);
            }
            else
            {
                RetrieveElementValue(resultObject, member, deserializedRawValue, xElementValue!);
            }

            RemoveRedundantElements(isHelperElementCreated, xElementValue, xAttributeValue);
        }

        return resultObject;
    }

    private void RemoveRedundantElements(bool isHelperElementCreated, XElement? xElementValue,
        XAttribute? xAttributeValue)
    {
        // remove the helper element
        if (isHelperElementCreated)
            xElementValue?.Remove();

        if (RemoveDeserializedXmlNodes)
        {
            xAttributeValue?.Remove();
            xElementValue?.Remove();
        }
    }

    private void InvokeCustomDeserializer(CustomSerializerWrapper customSerializerToUse, XElement baseElement,
        string deserializedValue, XElement? xElementValue,
        XAttribute? xAttributeValue, object resultObject, MemberWrapper member)
    {
        object desObj;
        if (member.IsSerializedAsAttribute)
            desObj = customSerializerToUse.DeserializeFromAttribute(xAttributeValue!,
                new SerializationContext(member, member.UdtWrapper, _serializer));
        else if (member.IsSerializedAsElement)
            desObj = customSerializerToUse.DeserializeFromElement(xElementValue!,
                new SerializationContext(member, member.UdtWrapper, _serializer));
        else if (member.IsSerializedAsValue)
            desObj = customSerializerToUse.DeserializeFromValue(deserializedValue,
                new SerializationContext(member, member.UdtWrapper, _serializer));
        else
            throw new YAXDeserializationException(baseElement, "Missing information how to deserialize the field.");

        try
        {
            member.SetValue(resultObject, desObj);
        }
        catch
        {
            OnExceptionOccurred(
                new YAXPropertyCannotBeAssignedTo(member.Alias.LocalName,
                    GetXmlLineInfo(xAttributeValue, xElementValue, baseElement)),
                _serializer.Options.ExceptionBehavior);
        }
    }

    private bool TrySetDefaultValue(XElement baseElement, object resultObject, XAttribute? xAttributeValue,
        XElement? xElementValue, MemberWrapper member)
    {
        // i.e. if it was NOT resuming deserialization,
        if (_deserializationObject != null)
            return false;

        // set default value, otherwise existing value for the member is kept

        if (!member.MemberType.IsValueType && _serializer.UdtWrapper.IsNotAllowedNullObjectSerialization)
        {
            return TrySetDefaultValue(resultObject, null, member,
                GetXmlLineInfo(xAttributeValue, xElementValue, baseElement));
        }

        if (member.DefaultValue != null)
        {
            return TrySetDefaultValue(resultObject, member.DefaultValue, member,
                GetXmlLineInfo(xAttributeValue, xElementValue, baseElement));
        }

        if (!member.MemberType.IsValueType)
        {
            member.SetValue(resultObject, null);
            return true;
        }

        return false;
    }

    private bool TrySetDefaultValue(object resultObject, object? resultValue, MemberWrapper member,
        IXmlLineInfo? lineInfo)
    {
        try
        {
            member.SetValue(resultObject, resultValue);
        }
        catch
        {
            OnExceptionOccurred(
                new YAXDefaultValueCannotBeAssigned(member.Alias.LocalName, member.DefaultValue ?? "null",
                    lineInfo, _serializer.Options.Culture), _serializer.Options.ExceptionBehavior);
            return false;
        }

        return true;
    }

    private static IXmlLineInfo? GetXmlLineInfo(IXmlLineInfo? attribute, IXmlLineInfo? element,
        IXmlLineInfo? baseElement)
    {
        return attribute ?? element ?? baseElement;
    }

    private bool DeserializeFromXmlElement(XElement baseElement, string serializationLocation, MemberWrapper member,
        object resultObject,
        ref string deserializedValue, ref bool isHelperElementCreated, ref XElement? xElementValue)
    {
        // member is serialized as an xml element

        var canContinue = false;
        var elem = XMLUtils.FindElement(baseElement, serializationLocation,
            member.Alias.OverrideNsIfEmpty(_serializer.TypeNamespace));

        if (elem != null)
        {
            deserializedValue = elem.Value;
            xElementValue = elem;
            return false;
        }

        // no such element was found yet

        if ((member.IsTreatedAsCollection || member.IsTreatedAsDictionary) &&
            member.CollectionAttributeInstance is
                { SerializationType: YAXCollectionSerializationTypes.RecursiveWithNoContainingElement })
        {
            if (AtLeastOneOfCollectionMembersExists(baseElement, member))
            {
                elem = baseElement;
                canContinue = true;
            }
            else
            {
                member.SetValue(resultObject, member.DefaultValue);
                return true;
            }
        }
        else if (!ReflectionUtils.IsBasicType(member.MemberType) && !member.IsTreatedAsCollection &&
                 !member.IsTreatedAsDictionary)
        {
            // try to fix this problem by creating a helper element, maybe all its children are placed somewhere else
            var helperElement = XMLUtils.CreateElement(baseElement, serializationLocation,
                member.Alias.OverrideNsIfEmpty(_serializer.TypeNamespace));
            if (helperElement != null)
            {
                isHelperElementCreated = true;
                if (AtLeastOneOfMembersExists(helperElement, member.MemberType))
                {
                    canContinue = true;
                    elem = helperElement;
                    deserializedValue = elem.Value;
                }
            }
        }
        else if ((_serializer.UdtWrapper.IsNotAllowedNullObjectSerialization && member.DefaultValue is null) ||
                 _serializer.UdtWrapper.IsNotAllowedDefaultValueSerialization)
        {
            // Any missing elements are allowed for deserialization:
            // * Don't set a value - uses default or initial value
            // * Ignore member.TreatErrorsAs
            // * Don't register YAXElementMissingException
            // * Skip Phase 2
            return true;
        }

        if (!canContinue)
            OnExceptionOccurred(new YAXElementMissingException(
                    StringUtils.CombineLocationAndElementName(serializationLocation,
                        member.Alias.OverrideNsIfEmpty(_serializer.TypeNamespace)), baseElement),
                GetExceptionType(member));

        xElementValue = elem;
        return false;
    }

    private string DeserializeFromValue(XElement baseElement, ref XElement xElementValue, string serializationLocation,
        MemberWrapper member)
    {
        var deserializedValue = string.Empty;
        var elem = XMLUtils.FindLocation(baseElement, serializationLocation);
        if (elem == null) // no such element is was found
        {
            OnExceptionOccurred(new YAXElementMissingException(
                    serializationLocation, baseElement),
                GetExceptionType(member));
        }
        else
        {
            var values = elem.Nodes().OfType<XText>().ToArray();
            if (values.Length <= 0)
            {
                // look for an element with the same name AND a yaxlib:realtype attribute
                var innerElement = XMLUtils.FindElement(baseElement, serializationLocation,
                    member.Alias.OverrideNsIfEmpty(_serializer.TypeNamespace));
                if (innerElement != null &&
                    innerElement.Attribute_NamespaceSafe(
                        _serializer.Options.Namespace.Uri + _serializer.Options.AttributeName.RealType,
                        _serializer.DocumentDefaultNamespace) != null)
                {
                    deserializedValue = innerElement.Value;
                    xElementValue = innerElement;
                }
                else
                {
                    OnExceptionOccurred(
                        new YAXElementValueMissingException(serializationLocation,
                            innerElement ?? baseElement),
                        GetExceptionType(member));
                }
            }
            else
            {
                deserializedValue = values[0].Value;
                values[0].Remove();
            }
        }

        return deserializedValue;
    }

    private string DeserializeFromAttribute(XElement baseElement, ref XElement xElementValue,
        ref XAttribute xAttributeValue, string serializationLocation, MemberWrapper member)
    {
        var deserializedValue = string.Empty;

        var attr = XMLUtils.FindAttribute(baseElement, serializationLocation,
            member.Alias.OverrideNsIfEmpty(_serializer.TypeNamespace));
        if (attr == null) // if the attribute does not exist in the element
        {
            // look for an element with the same name AND a yaxlib:realtype attribute
            var elem = XMLUtils.FindElement(baseElement, serializationLocation,
                member.Alias.OverrideNsIfEmpty(_serializer.TypeNamespace));
            if (elem?.Attribute_NamespaceSafe(
                    _serializer.Options.Namespace.Uri + _serializer.Options.AttributeName.RealType,
                    _serializer.DocumentDefaultNamespace) != null)
            {
                deserializedValue = elem.Value;
                xElementValue = elem;
            }
            else
            {
                OnExceptionOccurred(new YAXAttributeMissingException(
                        StringUtils.CombineLocationAndElementName(serializationLocation, member.Alias),
                        elem ?? baseElement),
                    GetExceptionType(member));
            }
        }
        else
        {
            deserializedValue = attr.Value;
            xAttributeValue = attr;
        }

        return deserializedValue;
    }

    private YAXExceptionTypes GetExceptionType(MemberWrapper member)
    {
        return !member.MemberType.IsValueType
               && _serializer.UdtWrapper.IsNotAllowedNullObjectSerialization
            ? YAXExceptionTypes.Ignore
            : member.TreatErrorsAs;
    }

    private bool TryDeserializeAsCollection(XElement baseElement, out object? resultObject)
    {
        resultObject = null;
        if (!_serializer.UdtWrapper.IsTreatedAsCollection || IsCreatedToDeserializeANonCollectionMember) return false;

        resultObject = DeserializeCollectionValue(_serializer.Type, baseElement, _serializer.UdtWrapper.Alias,
            _serializer.UdtWrapper.CollectionAttributeInstance!);

        return true;
    }

    private bool TryDeserializeAsDictionary(XElement baseElement, out object? resultObject)
    {
        resultObject = null;
        if (!_serializer.UdtWrapper.IsTreatedAsDictionary || IsCreatedToDeserializeANonCollectionMember) return false;
        if (_serializer.UdtWrapper.DictionaryAttributeInstance == null) return false;

        resultObject = DeserializeTaggedDictionaryValue(baseElement, _serializer.UdtWrapper.Alias, _serializer.Type,
            _serializer.UdtWrapper.CollectionAttributeInstance!, _serializer.UdtWrapper.DictionaryAttributeInstance);
        return true;
    }

    private void ProcessRealTypeAttribute(XElement baseElement)
    {
        var realTypeAttr = baseElement.Attribute_NamespaceSafe(
            _serializer.Options.Namespace.Uri + _serializer.Options.AttributeName.RealType,
            _serializer.DocumentDefaultNamespace);

        if (realTypeAttr == null) return;

        var theRealType = ReflectionUtils.GetTypeByName(realTypeAttr.Value);
        if (theRealType == null) return;

        _serializer.Type = theRealType;
        _serializer.UdtWrapper = UdtWrapperCache.Instance.GetOrAddItem(_serializer.Type, _serializer.Options);
    }

    /// <summary>
    /// Checks whether at least one of the collection members of
    /// the specified collection exists.
    /// </summary>
    /// <param name="elem">The XML element to check its content.</param>
    /// <param name="member">
    /// The class-member corresponding to the collection for
    /// which we intend to check existence of its members.
    /// </param>
    /// <returns></returns>
    private bool AtLeastOneOfCollectionMembersExists(XElement elem, MemberWrapper member)
    {
        if (!((member.IsTreatedAsCollection || member.IsTreatedAsDictionary) &&
              member.CollectionAttributeInstance != null &&
              member.CollectionAttributeInstance.SerializationType ==
              YAXCollectionSerializationTypes.RecursiveWithNoContainingElement))
            throw new ArgumentException("member should be a collection serialized without containing element");

        XName? eachElementName = null;

        if (member.CollectionAttributeInstance != null)
            eachElementName = StringUtils.RefineSingleElement(member.CollectionAttributeInstance.EachElementName!);

        if (member.DictionaryAttributeInstance != null && member.DictionaryAttributeInstance.EachPairName != null)
            eachElementName = StringUtils.RefineSingleElement(member.DictionaryAttributeInstance.EachPairName);

        if (eachElementName == null)
        {
            var colItemType = ReflectionUtils.GetCollectionItemType(member.MemberType);
            eachElementName = StringUtils.RefineSingleElement(ReflectionUtils.GetTypeFriendlyName(colItemType));
        }

        // return if such an element exists
        return elem.Element(
            eachElementName?
                .OverrideNsIfEmpty(member.Namespace.IfEmptyThen(_serializer.TypeNamespace)
                    .IfEmptyThenNone())!) != null;
    }

    /// <summary>
    /// Checks whether at least one member (property or field) of
    /// the specified element exists in the type.
    /// </summary>
    /// <param name="elem">The XML element to check its content.</param>
    /// <param name="type">
    /// The class type corresponding to the element for
    /// which we intend to check existence of its members.
    /// </param>
    /// <returns><see langword="true" />, if at least one member of the specified element exists in the type.</returns>
    private bool AtLeastOneOfMembersExists(XElement elem, Type type)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(elem);
#else
        if (elem == null) throw new ArgumentNullException(nameof(elem));
#endif
        var udtWrapper = UdtWrapperCache.Instance.GetOrAddItem(type, _serializer.Options);

        foreach (var member in udtWrapper.GetFieldsForDeserialization(false))
        {
            if (CanProcessAttribute(elem, member)) return true;

            // No attribute, so there should be an element

            if (XMLUtils.FindElement(elem, member.SerializationLocation, member.Alias) != null)
                return true;

            if (ReflectionUtils.IsBasicType(member.MemberType) || member.IsTreatedAsCollection ||
                member.IsTreatedAsDictionary || member.MemberType == _serializer.Type) continue;

            // Use an intermediate element
            var helperElement = XMLUtils.CreateElement(elem, member.SerializationLocation, member.Alias);
            var memberExists = AtLeastOneOfMembersExists(helperElement, member.MemberType);
            helperElement.Remove();

            return memberExists;
        }

        return false;
    }

    private bool CanProcessAttribute(XElement xElement, MemberWrapper member)
    {
        if (!member.IsSerializedAsAttribute) return false;

        // find the parent element from its location
        var attr = XMLUtils.FindAttribute(xElement, member.SerializationLocation, member.Alias);
        if (attr != null)
            return true;

        // maybe it has got a realtype attribute and hence have turned into an element
        var elem = XMLUtils.FindElement(xElement, member.SerializationLocation, member.Alias);
        return elem?.Attribute_NamespaceSafe(
            _serializer.Options.Namespace.Uri + _serializer.Options.AttributeName.RealType,
            _serializer.DocumentDefaultNamespace) != null;
    }

    /// <summary>
    /// Retrieves the value of the element from the specified XML element or attribute.
    /// </summary>
    /// <param name="obj">The object to store the retrieved value at.</param>
    /// <param name="member">The member of the specified object whose value we intent to retrieve.</param>
    /// <param name="elemValue">The value of the element stored as string.</param>
    /// <param name="xElementValue">
    /// The XML element value to be retrieved. If the value to be retrieved
    /// has been stored in an XML attribute, this reference is <c>null</c>.
    /// </param>
    private void RetrieveElementValue(object obj, MemberWrapper member, string elemValue, XElement xElementValue)
    {
        var memberType = member.MemberType;

        // when serializing collection with no containing element, then the real type attribute applies to the class
        // containing the collection, not the collection itself. That's because the containing element of collection is not
        // serialized. In this case the flag `isRealTypeAttributeNotRelevant` is set to true.
        var isRealTypeAttributeNotRelevant = member.CollectionAttributeInstance is
            { SerializationType: YAXCollectionSerializationTypes.RecursiveWithNoContainingElement };

        GetRealTypeIfSpecified(xElementValue, isRealTypeAttributeNotRelevant, ref memberType);

        if (TrySetValueForEmptyElement(obj, member, memberType, xElementValue)) return;

        if (TrySetValueForString(obj, elemValue, xElementValue, member, memberType)) return;

        if (TrySetValueForBasicType(obj, elemValue, xElementValue, member, memberType)) return;

        if (member.IsTreatedAsDictionary && member.DictionaryAttributeInstance != null)
        {
            DeserializeTaggedDictionaryMember(obj, member, xElementValue);
            return;
        }

        if (member.IsTreatedAsCollection)
        {
            DeserializeCollectionMember(obj, member, memberType, elemValue, xElementValue);
            return;
        }

        // use the default method for retrieving element values
        _ = TrySetValueDefault(obj, member, memberType, xElementValue);
    }

    /// <summary>
    /// The default method for retrieving element values.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="member"></param>
    /// <param name="memberType"></param>
    /// <param name="xElementValue"></param>
    private bool TrySetValueDefault(object obj, MemberWrapper member, Type memberType, XElement xElementValue)
    {
        var namespaceToOverride = member.Namespace.IfEmptyThen(_serializer.TypeNamespace).IfEmptyThenNone();
        using var serializerPoolObject =
            _serializer.GetChildSerializer(memberType, namespaceToOverride, null, out var serializer);

        serializer.Deserialization.IsCreatedToDeserializeANonCollectionMember =
            !(member.IsTreatedAsDictionary || member.IsTreatedAsCollection);

        if (_deserializationObject != null) // i.e. it is in resuming mode
            serializer.SetDeserializationBaseObject(member.GetValue(obj));

        var convertedObj = serializer.Deserialization.DeserializeBase(xElementValue);
        _serializer.FinalizeChildSerializer(serializer, false);

        try
        {
            member.SetValue(obj, convertedObj);
            return true;
        }
        catch
        {
            OnExceptionOccurred(new YAXPropertyCannotBeAssignedTo(member.Alias.LocalName, xElementValue),
                _serializer.Options.ExceptionBehavior);
        }

        return false;
    }

    private bool TrySetValueForBasicType(object obj, string elemValue, XElement xElementValue,
        MemberWrapper member,
        Type memberType)
    {
        if (!ReflectionUtils.IsBasicType(memberType)) return false;

        try
        {
            object? convertedObj;
            if (ReflectionUtils.IsNullable(memberType) && string.IsNullOrEmpty(elemValue))
                convertedObj = member.DefaultValue;
            else
                convertedObj = ReflectionUtils.ConvertBasicType(elemValue, memberType, _serializer.Options.Culture);

            try
            {
                member.SetValue(obj, convertedObj);
                return true;
            }
            catch
            {
                OnExceptionOccurred(new YAXPropertyCannotBeAssignedTo(member.Alias.LocalName, xElementValue),
                    _serializer.Options.ExceptionBehavior);
            }
        }
        catch (Exception ex)
        {
            if (ex is YAXException) throw;

            OnExceptionOccurred(new YAXBadlyFormedInput(member.Alias.LocalName, elemValue, xElementValue),
                member.TreatErrorsAs);

            try
            {
                member.SetValue(obj, member.DefaultValue);
                return true;
            }
            catch
            {
                OnExceptionOccurred(
                    new YAXDefaultValueCannotBeAssigned(member.Alias.LocalName, member.DefaultValue,
                        xElementValue, _serializer.Options.Culture), _serializer.Options.ExceptionBehavior);
            }
        }

        return false;
    }

    private bool TrySetValueForString(object obj, string? elemValue, XElement? xElementValue, MemberWrapper member,
        Type memberType)
    {
        if (memberType != typeof(string)) return false;
        try
        {
            // There's nothing to do for 'TextEmbedding.CData', it's deserialized transparently
            if (member.TextEmbedding == TextEmbedding.Base64 && !string.IsNullOrEmpty(elemValue))
            {
                member.SetValue(obj, elemValue!.Trim().FromBase64(Encoding.UTF8));
                return true;
            }

            // Take care of empty elements like '<Text />'
            if (string.IsNullOrEmpty(elemValue) && xElementValue != null)
                elemValue = xElementValue.IsEmpty ? null : string.Empty;

            member.SetValue(obj, elemValue);
            return true;
        }
        catch
        {
            OnExceptionOccurred(new YAXBadlyFormedInput(member.Alias.LocalName, elemValue ?? string.Empty),
                _serializer.Options.ExceptionBehavior);
        }

        return false;
    }

    private bool TrySetValueForEmptyElement(object obj, MemberWrapper member, Type memberType, XElement? xElementValue)
    {
        if (xElementValue == null || !XMLUtils.IsElementCompletelyEmpty(xElementValue) ||
            ReflectionUtils.IsBasicType(memberType) || member.IsTreatedAsCollection ||
            member.IsTreatedAsDictionary || AtLeastOneOfMembersExists(xElementValue, memberType)) return false;

        try
        {
            member.SetValue(obj, member.DefaultValue);
            return true;
        }
        catch
        {
            OnExceptionOccurred(
                new YAXDefaultValueCannotBeAssigned(member.Alias.LocalName, member.DefaultValue, xElementValue,
                    _serializer.Options.Culture),
                member.TreatErrorsAs);
        }

        return false;
    }

    private void GetRealTypeIfSpecified(XElement? xElementValue, bool isRealTypeAttributeNotRelevant,
        ref Type memberType)
    {
        // try to retrieve the real-type if specified
        if (xElementValue == null || isRealTypeAttributeNotRelevant) return;

        var realTypeAttribute = xElementValue.Attribute_NamespaceSafe(
            _serializer.Options.Namespace.Uri + _serializer.Options.AttributeName.RealType,
            _serializer.DocumentDefaultNamespace);

        if (realTypeAttribute == null) return;

        var realType = ReflectionUtils.GetTypeByName(realTypeAttribute.Value);
        if (realType != null) memberType = realType;
    }

    private static IList GetCollectionItemList(Type collectionItemType)
    {
        if (collectionItemType == typeof(object))
        {
            return new List<object>();
        }

        var listType = typeof(List<>).MakeGenericType(collectionItemType);
        var result = (IList) Activator.CreateInstance(listType)!;
        return result;
    }

    /// <summary>
    /// Retrieves the collection value.
    /// </summary>
    /// <param name="collType">Type of the collection to be retrieved.</param>
    /// <param name="xElement">The xml element.</param>
    /// <param name="memberAlias">The member's alias, used only in exception titles.</param>
    /// <param name="collAttrInstance">The collection attribute instance.</param>
    /// <returns></returns>
    private object? DeserializeCollectionValue(Type collType, XElement? xElement, XName memberAlias,
        YAXCollectionAttribute? collAttrInstance)
    {
        if (xElement is null) return null;

        // Get the container object from the element (may be null)
        _ = TryGetContainerObject(xElement, collType, memberAlias, out var containerObj);

        var collItemType = ReflectionUtils.GetCollectionItemType(collType);

        var dataItems = GetCollectionItemList(collItemType); // this will hold the actual data items
        var isPrimitive = ReflectionUtils.IsBasicType(collItemType);
        if (isPrimitive && collAttrInstance is
                { SerializationType: YAXCollectionSerializationTypes.Serially })
        {
            // The collection was serialized serially
            GetSerialCollectionItems(xElement, memberAlias, collAttrInstance, collItemType, dataItems);
        }
        else
        {
            // The collection was serialized recursively or has no containing element
            GetRecursiveCollectionItems(xElement, memberAlias, collAttrInstance, collItemType, isPrimitive, dataItems);
        }

        // Now dataItems list is filled and will be processed
        if (TryDataItemListDirect(collType, dataItems, out var directList))
            return directList;

        if (TryGetCollectionAsArray(xElement, collType, collItemType, memberAlias, dataItems, out var array))
            return array;

        if (TryGetCollectionAsDictionary(xElement, collType, collItemType, memberAlias, containerObj, dataItems, out var dictionary))
            return dictionary;

        if (TryGetCollectionAsNonGenericDictionary(xElement, collType, memberAlias, containerObj, dataItems, out var nonGenericDictionary))
            return nonGenericDictionary;

        if (TryGetCollectionAsBitArray(collType, dataItems, out var bitArray))
            return bitArray;

        if (TryGetCollectionAsStack(xElement, collType, memberAlias, containerObj, dataItems, out var stack))
            return stack;

        if (TryGetCollectionAsEnumerable(xElement, collType, memberAlias, containerObj, dataItems, out var enumerable))
            return enumerable;

        return null;
    }

    private static bool TryDataItemListDirect(Type collType, IList dataItems, out object? result)
    {
        if (collType.IsInstanceOfType(dataItems))
        {
            // no copy or transformation needed - e.g. IEnumerable<Item> - we can use the constructed data item list
            result = dataItems;
            return true;
        }
        result = null;
        return false;
    }

    private bool TryGetCollectionAsEnumerable(XElement xElement, Type collType, XName memberAlias, object? containerObj,
        IList dataItems, out object? enumerable)
    {
        enumerable = null;

        if (!ReflectionUtils.IsIEnumerable(collType)) return false;

        if (containerObj == null)
        {
            enumerable = dataItems;
            return true;
        }

        enumerable = containerObj;

        var additionMethodName = "Add";

        if (ReflectionUtils.IsTypeEqualOrInheritedFromType(collType, typeof(Queue)) ||
            ReflectionUtils.IsTypeEqualOrInheritedFromType(collType, typeof(Queue<>)))
            additionMethodName = "Enqueue";
        else if (ReflectionUtils.IsTypeEqualOrInheritedFromType(collType, typeof(LinkedList<>)))
            additionMethodName = "AddLast";

        foreach (var dataItem in dataItems)
            try
            {
                collType.InvokeMember(additionMethodName, BindingFlags.InvokeMethod, null, enumerable,
                    new[] { dataItem });
            }
            catch
            {
                OnExceptionOccurred(
                    new YAXCannotAddObjectToCollection(memberAlias.ToString(), dataItem, xElement),
                    _serializer.Options.ExceptionBehavior);
            }

        return true;
    }

    private bool TryGetCollectionAsStack(XElement xElement, Type collType, XName memberAlias, object? containerObj,
        IList dataItems,
        out object? stack)
    {
        stack = null;

        if (!ReflectionUtils.IsTypeEqualOrInheritedFromType(collType, typeof(Stack)) &&
            !ReflectionUtils.IsTypeEqualOrInheritedFromType(collType, typeof(Stack<>))) return false;

        var st = containerObj;

        const string additionMethodName = "Push";

        for (var i = dataItems.Count - 1; i >= 0; i--) // the loop must be from end to beginning
            try
            {
                collType.InvokeMember(additionMethodName, BindingFlags.InvokeMethod, null, st, new[] { dataItems[i] });
            }
            catch
            {
                OnExceptionOccurred(
                    new YAXCannotAddObjectToCollection(memberAlias.ToString(), dataItems[i], xElement),
                    _serializer.Options.ExceptionBehavior);
            }

        stack = st;
        return true;
    }

    private static bool TryGetCollectionAsBitArray(Type collType, IList dataItems, out object? bitArray)
    {
        bitArray = null;

        if (!ReflectionUtils.IsTypeEqualOrInheritedFromType(collType, typeof(BitArray))) return false;

        var ba = new bool[dataItems.Count];
        for (var i = 0; i < ba.Length; i++)
            try
            {
                ba[i] = (bool) (dataItems[i] ?? false);
            }
            catch
            {
                // Nothing to do, if cast fails
            }

        bitArray = Activator.CreateInstance(collType, ba);

        return true;
    }

    private bool TryGetCollectionAsNonGenericDictionary(XElement xElement, Type collType, XName memberAlias,
        object? containerObj,
        IList dataItems, out object? nonGenericDictionary)
    {
        nonGenericDictionary = containerObj;

        if (!ReflectionUtils.IsNonGenericIDictionary(collType)) return false;

        foreach (var lstItem in dataItems)
        {
            var key = lstItem?.GetType().GetProperty(KeyPropName, BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(lstItem, null);
            var value = lstItem?.GetType().GetProperty(ValuePropName, BindingFlags.Instance | BindingFlags.Public)
                ?.GetValue(lstItem, null);

            try
            {
                collType.InvokeMember("Add", BindingFlags.InvokeMethod, null, nonGenericDictionary,
                    new[] { key, value });
            }
            catch
            {
                OnExceptionOccurred(
                    new YAXCannotAddObjectToCollection(memberAlias.ToString(), lstItem, xElement),
                    _serializer.Options.ExceptionBehavior);
            }
        }

        return true;
    }

    private bool TryGetCollectionAsDictionary(XElement xElement, Type collType, Type collItemType, XName memberAlias,
        object? containerObj, IList dataItems, out object? dictionary)
    {
        dictionary = null;

        if (!ReflectionUtils.IsIDictionary(collType, out _, out _)) return false;

        // The collection is a Dictionary
        var dict = containerObj;

        foreach (var dataItem in dataItems)
        {
            var key = collItemType.GetProperty(KeyPropName)?.GetValue(dataItem, null);
            var value = collItemType.GetProperty(ValuePropName)?.GetValue(dataItem, null);
            try
            {
                collType.InvokeMember("Add", BindingFlags.InvokeMethod, null, dict, new[] { key, value });
            }
            catch
            {
                OnExceptionOccurred(
                    new YAXCannotAddObjectToCollection(memberAlias.ToString(), dataItem, xElement),
                    _serializer.Options.ExceptionBehavior);
            }
        }

        dictionary = dict;
        return true;
    }

    private bool TryGetCollectionAsArray(XElement xElement, Type collType, Type collItemType, XName memberAlias,
        IList dataItems, out object? array)
    {
        array = null;

        if (!ReflectionUtils.IsArray(collType)) return false;

        var dimsAttr = xElement.Attribute_NamespaceSafe(
            _serializer.Options.Namespace.Uri + _serializer.Options.AttributeName.Dimensions,
            _serializer.DocumentDefaultNamespace);
        var dims = Array.Empty<int>();
        if (dimsAttr != null) dims = StringUtils.ParseArrayDimsString(dimsAttr.Value);

        Array arrayInstance;
        if (dims.Length > 0)
        {
            var lowerBounds = new int[dims.Length]; // an array of zeros
            arrayInstance = Array.CreateInstance(collItemType, dims, lowerBounds); // create the array

            var count = Math.Min(arrayInstance.Length, dataItems.Count);
            // now fill the array
            for (var i = 0; i < count; i++)
            {
                var dimIndex = GetArrayDimensionalIndex(i, dims);
                try
                {
                    arrayInstance.SetValue(dataItems[i], dimIndex);
                }
                catch
                {
                    OnExceptionOccurred(
                        new YAXCannotAddObjectToCollection(memberAlias.ToString(), dataItems[i], xElement),
                        _serializer.Options.ExceptionBehavior);
                }
            }
        }
        else
        {
            arrayInstance = Array.CreateInstance(collItemType, dataItems.Count); // create the array

            var count = Math.Min(arrayInstance.Length, dataItems.Count);
            // now fill the array
            for (var i = 0; i < count; i++)
                try
                {
                    arrayInstance.SetValue(dataItems[i], i);
                }
                catch
                {
                    OnExceptionOccurred(
                        new YAXCannotAddObjectToCollection(memberAlias.ToString(), dataItems[i], xElement),
                        _serializer.Options.ExceptionBehavior);
                }
        }

        array = arrayInstance;
        return true;
    }

    /// <summary>
    /// Gets the data items for a collection that was serialized recursively,
    /// or that has no containing element
    /// </summary>
    /// <param name="xElement"></param>
    /// <param name="memberAlias"></param>
    /// <param name="collAttrInstance"></param>
    /// <param name="collItemType"></param>
    /// <param name="isPrimitive"></param>
    /// <param name="dataItems">The list that will be filled.</param>
    private void GetRecursiveCollectionItems(XElement xElement, XName memberAlias,
        YAXCollectionAttribute? collAttrInstance,
        Type collItemType, bool isPrimitive, IList dataItems)
    {
        XName? eachElemName = null;
        if (collAttrInstance is { EachElementName: not null })
        {
            eachElemName = StringUtils.RefineSingleElement(collAttrInstance.EachElementName)!;
            eachElemName =
                eachElemName.OverrideNsIfEmpty(memberAlias.Namespace.IfEmptyThen(_serializer.TypeNamespace)
                    .IfEmptyThenNone());
        }

        var elemsToSearch = eachElemName == null ? xElement.Elements() : xElement.Elements(eachElemName);

        foreach (var childElem in elemsToSearch)
        {
            var curElementType = collItemType;
            var curElementIsPrimitive = isPrimitive;

            var realTypeAttribute = childElem.Attribute_NamespaceSafe(
                _serializer.Options.Namespace.Uri + _serializer.Options.AttributeName.RealType,
                _serializer.DocumentDefaultNamespace);
            if (realTypeAttribute != null)
            {
                var theRealType = ReflectionUtils.GetTypeByName(realTypeAttribute.Value);
                if (theRealType != null)
                {
                    curElementType = theRealType;
                    curElementIsPrimitive = ReflectionUtils.IsBasicType(curElementType);
                }
            }

            // Check if curElementType is derived or is the same is itemType.
            // For speed concerns we perform this check only when eachElemName is null
            if (eachElemName == null && (curElementType == typeof(object) ||
                                         !ReflectionUtils.IsTypeEqualOrInheritedFromType(curElementType,
                                             collItemType)))
                continue;

            if (curElementIsPrimitive)
            {
                try
                {
                    dataItems.Add(ReflectionUtils.ConvertBasicType(childElem.Value, curElementType,
                        _serializer.Options.Culture)!);
                }
                catch
                {
                    OnExceptionOccurred(
                        new YAXBadlyFormedInput(childElem.Name.ToString(), childElem.Value, childElem),
                        _serializer.Options.ExceptionBehavior);
                }
            }
            else
            {
                var namespaceToOverride =
                    memberAlias.Namespace.IfEmptyThen(_serializer.TypeNamespace).IfEmptyThenNone();
                using var serializerPoolObject =
                    _serializer.GetChildSerializer(curElementType, namespaceToOverride, null, out var ser);
                dataItems.Add(ser.Deserialization.DeserializeBase(childElem));
                _serializer.FinalizeChildSerializer(ser, false);
            }
        }
    }

    /// <summary>
    /// Gets the data items for a collection that was serialized serially.
    /// </summary>
    /// <param name="xElement"></param>
    /// <param name="memberAlias"></param>
    /// <param name="collAttrInstance"></param>
    /// <param name="collItemType"></param>
    /// <param name="dataItems">The list that will be filled.</param>
    private void GetSerialCollectionItems(XElement xElement, XName memberAlias,
        YAXCollectionAttribute collAttrInstance,
        Type collItemType,
        IList dataItems)
    {
        var separators = collAttrInstance.SeparateBy.ToCharArray();

        // Should we add white space characters to the separators?
        if (collAttrInstance.IsWhiteSpaceSeparator)
            separators = separators.Union(WhiteSpaceChars).ToArray();

        var elemValue = xElement.Value;
        var items = elemValue.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        foreach (var wordItem in items)
            try
            {
                dataItems.Add(ReflectionUtils.ConvertBasicType(wordItem, collItemType, _serializer.Options.Culture));
            }
            catch
            {
                OnExceptionOccurred(new YAXBadlyFormedInput(memberAlias.ToString(), elemValue, xElement),
                    _serializer.Options.ExceptionBehavior);
            }
    }

    private bool TryGetContainerObject(XElement xElement, Type colType, XName memberAlias, out object? containerObj)
    {
        containerObj = null;

        // The collection type has an empty constructor
        if (!ReflectionUtils.IsInstantiableCollection(colType)) return false;

        var namespaceToOverride = memberAlias.Namespace.IfEmptyThen(_serializer.TypeNamespace).IfEmptyThenNone();
        using var serializerPoolObject =
            _serializer.GetChildSerializer(colType, namespaceToOverride, null, out var containerSer);

        containerSer.Deserialization.IsCreatedToDeserializeANonCollectionMember = true;
        containerSer.Deserialization.RemoveDeserializedXmlNodes = true;

        containerObj = containerSer.Deserialization.DeserializeBase(xElement);
        _serializer.FinalizeChildSerializer(containerSer, false);

        return true;
    }

    /// <summary>
    /// Deserializes the collection member.
    /// </summary>
    /// <param name="o">The object to store the retrieved value at.</param>
    /// <param name="member">The member of the specified object whose value we intent to retreive.</param>
    /// <param name="colType">Type of the collection to be retrieved.</param>
    /// <param name="elemValue">The value of the element stored as string.</param>
    /// <param name="xelemValue">
    /// The XML element value to be retrieved. If the value to be retrieved
    /// has been stored in an XML attribute, this reference is <c>null</c>.
    /// </param>
    private void DeserializeCollectionMember(object o, MemberWrapper member, Type colType, string elemValue,
        XElement xelemValue)
    {
        object? colObject;

        if (member.CollectionAttributeInstance is { SerializationType: YAXCollectionSerializationTypes.Serially }
            && (member.IsSerializedAsAttribute || member.IsSerializedAsValue))
        {
            colObject = DeserializeCollectionValue(colType, new XElement("temp", elemValue), "temp",
                member.CollectionAttributeInstance);
        }
        else
        {
            var memberAlias = member.Alias.OverrideNsIfEmpty(_serializer.TypeNamespace);
            colObject = DeserializeCollectionValue(colType, xelemValue, memberAlias,
                member.CollectionAttributeInstance!);
        }

        try
        {
            member.SetValue(o, colObject);
        }
        catch
        {
            OnExceptionOccurred(new YAXPropertyCannotBeAssignedTo(member.Alias.LocalName, xelemValue),
                _serializer.Options.ExceptionBehavior);
        }
    }

    /// <summary>
    /// Gets the dimensional index for an element of a multi-dimensional array from a
    /// linear index specified.
    /// </summary>
    /// <param name="linearIndex">The linear index.</param>
    /// <param name="dimensions">The dimensions of the array.</param>
    /// <returns></returns>
    private static int[] GetArrayDimensionalIndex(long linearIndex, int[] dimensions)
    {
        var result = new int[dimensions.Length];

        var d = (int) linearIndex;

        for (var n = dimensions.Length - 1; n > 0; n--)
        {
            result[n] = d % dimensions[n];
            d = (d - result[n]) / dimensions[n];
        }

        result[0] = d;
        return result;
    }

    private object? DeserializeTaggedDictionaryValue(XElement xElementValue, XName alias, Type type,
        YAXCollectionAttribute collAttributeInstance, YAXDictionaryAttribute dictAttrInstance)
    {
        if (!ReflectionUtils.IsIDictionary(type, out var keyType, out var valueType))
            throw new ArgumentException("Type must be a Dictionary", nameof(type));

        // deserialize non-collection fields
        var namespaceToOverride = alias.Namespace.IfEmptyThen(_serializer.TypeNamespace).IfEmptyThenNone();
        using var serializerPoolObject =
            _serializer.GetChildSerializer(type, namespaceToOverride, null, out var containerSer);
        containerSer.Deserialization.IsCreatedToDeserializeANonCollectionMember = true;
        containerSer.Deserialization.RemoveDeserializedXmlNodes = true;
        var dic = containerSer.Deserialization.DeserializeBase(xElementValue);
        _serializer.FinalizeChildSerializer(containerSer, false);

        // deserialize collection fields
        ReflectionUtils.IsIEnumerable(type, out var pairType);
        XName? eachElementName = StringUtils.RefineSingleElement(ReflectionUtils.GetTypeFriendlyName(pairType));
        var keyAlias = alias.Namespace.IfEmptyThen(_serializer.TypeNamespace).IfEmptyThenNone() + KeyPropName;
        var valueAlias = alias.Namespace.IfEmptyThen(_serializer.TypeNamespace).IfEmptyThenNone() + ValuePropName;

        if (collAttributeInstance is { EachElementName: { } })
        {
            eachElementName = StringUtils.RefineSingleElement(collAttributeInstance.EachElementName)!;
            eachElementName =
                eachElementName.OverrideNsIfEmpty(alias.Namespace.IfEmptyThen(_serializer.TypeNamespace)
                    .IfEmptyThenNone());
        }

        GetDictionaryAttributeDetails(dictAttrInstance, alias, ref eachElementName, ref keyAlias, ref valueAlias);
        GetDictionaryAttributeFlags(dictAttrInstance, keyType, valueType, out var isKeyAttribute, out var isKeyContent,
            out var isValueAttribute, out var isValueContent);

        foreach (var childElem in xElementValue.Elements(eachElementName))
        {
            object? key = null, value = null;
            YAXSerializer? keySerializer = null, valueSerializer = null;

            var isKeyFound = VerifyDictionaryPairElements(ref keyType, ref isKeyAttribute, ref isKeyContent, keyAlias,
                childElem);
            var isValueFound = VerifyDictionaryPairElements(ref valueType, ref isValueAttribute, ref isValueContent,
                valueAlias, childElem);

            if (!isKeyFound && !isValueFound)
                continue;

            if (isKeyFound)
                key = GetTaggedDictionaryKey(childElem, keyType, keyAlias, isKeyAttribute, isKeyContent,
                    keySerializer);

            if (isValueFound)
                value = GetTaggedDictionaryValue(childElem, valueAlias, valueType, isValueAttribute, isValueContent,
                    valueSerializer);

            try
            {
                type.InvokeMember("Add", BindingFlags.InvokeMethod, null, dic, new[] { key, value });
            }
            catch
            {
                OnExceptionOccurred(
                    new YAXCannotAddObjectToCollection(alias.LocalName,
                        new KeyValuePair<object?, object?>(key, value), childElem),
                    _serializer.Options.ExceptionBehavior);
            }
        }

        return dic;
    }

    private object? GetTaggedDictionaryValue(XElement childElem, XName valueAlias, Type valueType,
        bool isValueAttribute,
        bool isValueContent, YAXSerializer? valueSerializer)
    {
        object? value;
        if (isValueAttribute)
        {
            value = ReflectionUtils.ConvertBasicType(
                childElem.Attribute_NamespaceSafe(valueAlias, _serializer.DocumentDefaultNamespace)!.Value, valueType,
                _serializer.Options.Culture);
        }
        else if (isValueContent)
        {
            value = ReflectionUtils.ConvertBasicType(childElem.GetXmlContent(), valueType, _serializer.Options.Culture);
        }
        else if (ReflectionUtils.IsBasicType(valueType))
        {
            value = ReflectionUtils.ConvertBasicType(childElem.Element(valueAlias)!.Value, valueType,
                _serializer.Options.Culture);
        }
        else
        {
            using var serializerPoolObject =
                _serializer.GetChildSerializer(valueType, valueAlias.Namespace, null, out var serializer);

            valueSerializer ??= serializer;

            value = valueSerializer.Deserialization.DeserializeBase(childElem.Element(valueAlias));
            _serializer.FinalizeChildSerializer(valueSerializer, false);
        }

        return value;
    }

    private object? GetTaggedDictionaryKey(XElement xElement, Type keyType, XName keyAlias, bool isKeyAttribute,
        bool isKeyContent, YAXSerializer? keySerializer)
    {
        object? key;
        if (isKeyAttribute)
        {
            key = ReflectionUtils.ConvertBasicType(
                xElement.Attribute_NamespaceSafe(keyAlias, _serializer.DocumentDefaultNamespace)?.Value!, keyType,
                _serializer.Options.Culture);
        }
        else if (isKeyContent)
        {
            key = ReflectionUtils.ConvertBasicType(xElement.GetXmlContent(), keyType, _serializer.Options.Culture);
        }
        else if (ReflectionUtils.IsBasicType(keyType))
        {
            key = ReflectionUtils.ConvertBasicType(xElement.Element(keyAlias)!.Value, keyType,
                _serializer.Options.Culture);
        }
        else
        {
            using var serializerPoolObject =
                _serializer.GetChildSerializer(keyType, keyAlias.Namespace, null, out var serializer);

            keySerializer ??= serializer;

            key = keySerializer.Deserialization.DeserializeBase(xElement.Element(keyAlias));
            _serializer.FinalizeChildSerializer(keySerializer, false);
        }

        return key;
    }

    private static void GetDictionaryAttributeFlags(YAXDictionaryAttribute? dictAttrInstance, Type keyType,
        Type valueType,
        out bool isKeyAttribute, out bool isKeyContent, out bool isValueAttribute,
        out bool isValueContent)
    {
        // Set defaults
        isKeyAttribute = false;
        isValueAttribute = false;
        isKeyContent = false;
        isValueContent = false;

        if (dictAttrInstance == null) return;

        if (dictAttrInstance.SerializeKeyAs == YAXNodeTypes.Attribute)
            isKeyAttribute = ReflectionUtils.IsBasicType(keyType);
        else if (dictAttrInstance.SerializeKeyAs == YAXNodeTypes.Content)
            isKeyContent = ReflectionUtils.IsBasicType(keyType);

        if (dictAttrInstance.SerializeValueAs == YAXNodeTypes.Attribute)
            isValueAttribute = ReflectionUtils.IsBasicType(valueType);
        else if (dictAttrInstance.SerializeValueAs == YAXNodeTypes.Content)
            isValueContent = ReflectionUtils.IsBasicType(valueType);
    }

    private void GetDictionaryAttributeDetails(YAXDictionaryAttribute? dictAttrInstance, XName alias,
        ref XName? eachElementName, ref XName keyAlias, ref XName valueAlias)
    {
        if (dictAttrInstance == null) return;

        if (dictAttrInstance.EachPairName != null)
        {
            eachElementName = StringUtils.RefineSingleElement(dictAttrInstance.EachPairName);
            eachElementName =
                eachElementName?.OverrideNsIfEmpty(alias.Namespace.IfEmptyThen(_serializer.TypeNamespace)
                    .IfEmptyThenNone());
        }

        if (dictAttrInstance.KeyName != null)
        {
            keyAlias = StringUtils.RefineSingleElement(dictAttrInstance.KeyName)!;
            keyAlias = keyAlias.OverrideNsIfEmpty(alias.Namespace.IfEmptyThen(_serializer.TypeNamespace)
                .IfEmptyThenNone());
        }

        if (dictAttrInstance.ValueName != null)
        {
            valueAlias = StringUtils.RefineSingleElement(dictAttrInstance.ValueName)!;
            valueAlias =
                valueAlias.OverrideNsIfEmpty(alias.Namespace.IfEmptyThen(_serializer.TypeNamespace).IfEmptyThenNone());
        }
    }

    /// <summary>
    /// Deserializes a dictionary member which also benefits from a <see cref="YAXDictionaryAttribute" />.
    /// </summary>
    /// <param name="o">The object to hold the deserialized value.</param>
    /// <param name="member">The member corresponding to the dictionary member.</param>
    /// <param name="xelemValue">The XML element value.</param>
    private void DeserializeTaggedDictionaryMember(object o, MemberWrapper member, XElement xelemValue)
    {
        var dic = DeserializeTaggedDictionaryValue(xelemValue, member.Alias, member.MemberType,
            member.CollectionAttributeInstance!, member.DictionaryAttributeInstance!);

        try
        {
            member.SetValue(o, dic);
        }
        catch
        {
            OnExceptionOccurred(new YAXPropertyCannotBeAssignedTo(member.Alias.LocalName, xelemValue),
                _serializer.Options.ExceptionBehavior);
        }
    }

    /// <summary>
    /// Verifies the existence of dictionary pair <c>Key</c> and <c>Value</c> elements.
    /// </summary>
    /// <param name="type">Type of the key or content.</param>
    /// <param name="isAttribute">
    /// if set to <see langword="true" /> means that key or content have been serialize as an
    /// attribute.
    /// </param>
    /// <param name="isContent">
    /// if set to <see langword="true" /> means that key or content has been serialize as an XML
    /// content.
    /// </param>
    /// <param name="alias">The alias for the key or content.</param>
    /// <param name="childElem">The child XML element to search <c>Key</c> and <c>Value</c> elements in.</param>
    /// <returns><ref langword="true" /> if the elements were found.</returns>
    private bool VerifyDictionaryPairElements(ref Type type, ref bool isAttribute, ref bool isContent,
        XName alias, XElement childElem)
    {
        bool isFound;

        if (isAttribute && childElem.Attribute_NamespaceSafe(alias, _serializer.DocumentDefaultNamespace) != null)
        {
            isFound = true;
        }
        else if (isContent && childElem.GetXmlContent() != string.Empty)
        {
            isFound = true;
        }
        else
        {
            isFound = VerifyDictionaryPairElementsInChild(ref type, ref isAttribute, ref isContent, alias, childElem);
        }

        return isFound;
    }

    /// <summary>
    /// Verifies the existence of a child dictionary pair <c>Key</c> and <c>Value</c> element.
    /// Here we look for an element with the same name.
    /// If it is found, we also check for a yaxlib:realtype attribute to get the real type.
    /// </summary>
    /// <param name="type">Type of the key or content.</param>
    /// <param name="isAttribute">
    /// if set to <see langword="true" /> means that key or content have been serialize as an
    /// attribute.
    /// </param>
    /// <param name="isContent">
    /// if set to <see langword="true" /> means that key or content has been serialize as an XML
    /// content.
    /// </param>
    /// <param name="alias">The alias for the key or content.</param>
    /// <param name="childElem">The child XML element to search <c>Key</c> and <c>Value</c> elements in.</param>
    /// <returns><ref langword="true" /> if the elements were found.</returns>
    private bool VerifyDictionaryPairElementsInChild(ref Type type, ref bool isAttribute, ref bool isContent,
        XName alias,
        XElement childElem)
    {
        var elem = childElem.Element(alias);
        if (elem == null) return false;

        var realTypeAttr = elem.Attribute_NamespaceSafe(
            _serializer.Options.Namespace.Uri + _serializer.Options.AttributeName.RealType,
            _serializer.DocumentDefaultNamespace);

        if (realTypeAttr == null)
            return true; // we found a child element (but without yaxlib:realtype attribute)

        var theRealType = ReflectionUtils.GetTypeByName(realTypeAttr.Value);
        if (theRealType != null)
        {
            isAttribute = false;
            isContent = false;
            type = theRealType;
        }

        return true; // we found a child element (but without finding the real type)
    }

    /// <summary>
    /// Deserializes the XML representation of a key-value pair, as specified, and returns
    /// a <c>KeyValuePair</c> instance containing the deserialized data.
    /// </summary>
    /// <param name="baseElement">The element containing the XML representation of a key-value pair.</param>
    /// <returns>a <c>KeyValuePair</c> instance containing the deserialized data</returns>
    private object? DeserializeKeyValuePair(XElement baseElement)
    {
        var genArgs = _serializer.Type.GetGenericArguments();
        var keyType = genArgs[0];
        var valueType = genArgs[1];

        var xNameKey = _serializer.TypeNamespace.IfEmptyThenNone() + KeyPropName;
        var xNameValue = _serializer.TypeNamespace.IfEmptyThenNone() + ValuePropName;

        object? keyValue, valueValue;
        if (ReflectionUtils.IsBasicType(keyType))
        {
            try
            {
                keyValue = ReflectionUtils.ConvertBasicType(
                    baseElement.Element(xNameKey)?.Value!, keyType, _serializer.Options.Culture);
            }
            catch (NullReferenceException)
            {
                keyValue = null;
            }
        }
        else if (ReflectionUtils.IsStringConvertibleIFormattable(keyType))
        {
            keyValue = Activator.CreateInstance(keyType, baseElement.Element(xNameKey)?.Value);
        }
        else if (ReflectionUtils.IsCollectionType(keyType))
        {
            keyValue = DeserializeCollectionValue(keyType,
                baseElement.Element(xNameKey), xNameKey, null);
        }
        else
        {
            using var serializerPoolObject =
                _serializer.GetChildSerializer(keyType, xNameKey.Namespace.IfEmptyThenNone(), null, out var ser);

            keyValue = ser.Deserialization.DeserializeBase(baseElement.Element(xNameKey));
            _serializer.FinalizeChildSerializer(ser, false);
        }

        if (ReflectionUtils.IsBasicType(valueType))
        {
            try
            {
                valueValue = ReflectionUtils.ConvertBasicType(baseElement.Element(xNameValue)?.Value!, valueType,
                    _serializer.Options.Culture);
            }
            catch (NullReferenceException)
            {
                valueValue = null;
            }
        }
        else if (ReflectionUtils.IsStringConvertibleIFormattable(valueType))
        {
            valueValue = Activator.CreateInstance(valueType, baseElement.Element(xNameValue)?.Value);
        }
        else if (ReflectionUtils.IsCollectionType(valueType))
        {
            valueValue = DeserializeCollectionValue(valueType,
                baseElement.Element(xNameValue), xNameValue, null);
        }
        else
        {
            using var serializerPoolObject =
                _serializer.GetChildSerializer(valueType, xNameValue.Namespace.IfEmptyThenNone(), null, out var ser);

            valueValue = ser.Deserialization.DeserializeBase(baseElement.Element(xNameValue));
            _serializer.FinalizeChildSerializer(ser, false);
        }

        var pair = Activator.CreateInstance(_serializer.Type, keyValue, valueValue);
        return pair;
    }

    /// <summary>
    /// Generates XDocument LoadOptions from SerializationOption
    /// </summary>
    internal LoadOptions GetXmlLoadOptions()
    {
        var options = LoadOptions.None;
        if (_serializer.Options.SerializationOptions.HasFlag(YAXSerializationOptions.DisplayLineInfoInExceptions))
            options |= LoadOptions.SetLineInfo;
        return options;
    }
}
