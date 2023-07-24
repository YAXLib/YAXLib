// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using YAXLib.Attributes;
using YAXLib.Caching;
using YAXLib.Customization;
using YAXLib.Enums;
using YAXLib.Exceptions;
using YAXLib.Pooling.SpecializedPools;

namespace YAXLib;

internal class Serialization
{
    private readonly YAXSerializer _serializer;

    /// <summary>
    /// A reference to the base xml element used during serialization.
    /// Will always be set by <see cref="SetBaseElement(XName)" /> when serialization starts.
    /// </summary>
    private XElement? _baseElement;

    /// <summary>
    /// XML document object which will hold the resulting serialization
    /// </summary>
    private XDocument? _mainDocument;

    private bool _stripInvalidXmlChars;

    /// <summary>
    /// This instance will be (re-) initialized it a way
    /// that it has the same virgin state like an instance that
    /// was created with one of the CTORs.
    /// </summary>
    internal void Initialize()
    {
        _baseElement = null;
        _mainDocument = null;
        _stripInvalidXmlChars =
            _serializer.Options.SerializationOptions.HasFlag(YAXSerializationOptions.StripInvalidXmlChars);
    }

    public Serialization(YAXSerializer serializer)
    {
        _serializer = serializer;
    }

    /// <summary>
    /// Serializes the object into an <c>XDocument</c> object.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns></returns>
    public XDocument SerializeXDocument(object? obj)
    {
        // This method must be called by any public Serialize method
        _serializer.IsSerializing = true;
        _mainDocument = new XDocument();
        _mainDocument.Add(SerializeBase(obj));
        return _mainDocument;
    }

    /// <summary>
    /// The base method that performs the whole job of serialization.
    /// Other serialization methods call this method to have their job done.
    /// </summary>
    /// <param name="obj">The object to be serialized</param>
    /// <param name="className">Name of the element that contains the serialized object.</param>
    /// <returns>
    /// an instance of <c>XElement</c> which contains the result of
    /// serialization of the specified object
    /// </returns>
    private XElement SerializeBase(object obj, XName className)
    {
        // this is set once again here since internal serializers
        // must not call public Serialize methods
        _serializer.IsSerializing = true;

        SetBaseElement(className);

        if (((IRecursionCounter) _serializer).RecursionCount >= _serializer.Options.MaxRecursion - 1)
        {
            PushObjectToSerializationStack(obj);
            return _baseElement!;
        }

        if (!_serializer.Type.IsValueType)
        {
            var alreadySerializedObject = _serializer.SerializedStack.FirstOrDefault(x => ReferenceEquals(x, obj));
            if (alreadySerializedObject != null)
            {
                if (!_serializer.UdtWrapper.ThrowUponSerializingCyclingReferences)
                {
                    // although we are not going to serialize anything, push the object to be picked up
                    // by the pop statement right after serialization
                    PushObjectToSerializationStack(obj);
                    return _baseElement!;
                }

                throw new YAXCannotSerializeSelfReferentialTypes(_serializer.Type);
            }

            PushObjectToSerializationStack(obj);
        }

        if (_serializer.UdtWrapper.HasComment && _baseElement!.Parent == null && _mainDocument != null)
            foreach (var comment in _serializer.UdtWrapper.Comment!)
                _mainDocument.Add(new XComment(comment));

        // if the containing element is set to preserve spaces, then emit the
        // required attribute
        if (_serializer.UdtWrapper.PreservesWhitespace)
            XMLUtils.AddPreserveSpaceAttribute(_baseElement!, _serializer.Options.Culture);

        // check if the main class/type has defined custom serializers
        if (_serializer.UdtWrapper.HasCustomSerializer &&
            !Locker.IsLocked(_serializer.UdtWrapper.CustomSerializer!.Type))
        {
            _serializer.UdtWrapper.CustomSerializer!.SerializeToElement(obj, _baseElement!,
                new SerializationContext(null, _serializer.UdtWrapper, _serializer));
        }
        else if (_serializer.UdtWrapper.IsKnownType && !Locker.IsLocked(_serializer.UdtWrapper.KnownType!.Type))
        {
            using var _ = new Locker(_serializer.UdtWrapper.KnownType!.Type);
            _serializer.UdtWrapper.KnownType!.Serialize(obj, _baseElement!, _serializer.TypeNamespace,
                new SerializationContext(null, _serializer.UdtWrapper, _serializer));
        }
        else // no custom serializers or known type
        {
            SerializeFields(obj);
        }

        if (_baseElement!.Parent == null)
            _serializer.XmlNamespaceManager.AddNamespacesToElement(_baseElement,
                _serializer.DocumentDefaultNamespace, _serializer.Options, _serializer.UdtWrapper);

        return _baseElement;
    }

    /// <summary>
    /// One of the base methods that perform the whole job of serialization.
    /// </summary>
    /// <param name="obj">The object to be serialized</param>
    /// <returns>
    /// an instance of <c>XElement</c> which contains the result of
    /// serialization of the specified object
    /// </returns>
    private XElement SerializeBase(object? obj)
    {
        if (obj == null)
            return new XElement(_serializer.UdtWrapper.Alias);

        if (!_serializer.Type.IsInstanceOfType(obj))
            throw new YAXObjectTypeMismatch(_serializer.Type, obj.GetType());

        _serializer.DocumentDefaultNamespace = _serializer.UdtWrapper.FindDocumentDefaultNamespace();

        if (TrySerializeAsDictionary(obj, out var xElement)) return xElement!;

        if (TrySerializeAsCollection(obj, out xElement)) return xElement!;

        if (TrySerializeUnderlyingTypeAsBasicType(obj, out xElement)) return xElement!;

        if (TrySerializeUnderlyingTypeIfNotEqualOrNullableOfObjectType(obj, out xElement)) return xElement!;

        // SerializeBase will add the object to the stack
        var elem = SerializeBase(obj, _serializer.UdtWrapper.Alias);
        if (!_serializer.Type.IsValueType) _serializer.SerializedStack.Pop();
        Debug.Assert(_serializer.SerializedStack.Count == 0,
            "Serialization stack is not empty at the end of serialization");

        return elem;
    }

    private bool TrySerializeUnderlyingTypeAsBasicType(object obj, out XElement? xElement)
    {
        xElement = null;

        if (!ReflectionUtils.IsBasicType(_serializer.UdtWrapper.UnderlyingType)) return false;

        var elemResult = MakeBaseElement(null, _serializer.UdtWrapper.Alias, obj, out _);
        if (_serializer.UdtWrapper.PreservesWhitespace)
            XMLUtils.AddPreserveSpaceAttribute(elemResult, _serializer.Options.Culture);
        if (elemResult.Parent == null)
            _serializer.XmlNamespaceManager.AddNamespacesToElement(elemResult, _serializer.DocumentDefaultNamespace,
                _serializer.Options, _serializer.UdtWrapper);

        xElement = elemResult;
        return true;
    }

    private bool TrySerializeAsCollection(object obj, out XElement? xElement)
    {
        xElement = null;

        if (!_serializer.UdtWrapper.IsTreatedAsCollection) return false;

        var elemResult = MakeCollectionElement(null, _serializer.UdtWrapper.Alias, obj, null, null);
        if (_serializer.UdtWrapper.PreservesWhitespace)
            XMLUtils.AddPreserveSpaceAttribute(elemResult, _serializer.Options.Culture);
        if (elemResult.Parent == null)
            _serializer.XmlNamespaceManager.AddNamespacesToElement(elemResult, _serializer.DocumentDefaultNamespace,
                _serializer.Options, _serializer.UdtWrapper);

        xElement = elemResult;
        return true;
    }

    private bool TrySerializeAsDictionary(object obj, out XElement? xElement)
    {
        xElement = null;

        // to serialize stand-alone collection or dictionary objects
        if (!_serializer.UdtWrapper.IsTreatedAsDictionary) return false;

        var elemResult = MakeDictionaryElement(null, _serializer.UdtWrapper.Alias, obj,
            _serializer.UdtWrapper.DictionaryAttributeInstance, _serializer.UdtWrapper.CollectionAttributeInstance,
            _serializer.UdtWrapper.IsNotAllowedNullObjectSerialization);
        if (_serializer.UdtWrapper.PreservesWhitespace)
            XMLUtils.AddPreserveSpaceAttribute(elemResult, _serializer.Options.Culture);
        if (elemResult.Parent == null)
            _serializer.XmlNamespaceManager.AddNamespacesToElement(elemResult, _serializer.DocumentDefaultNamespace,
                _serializer.Options, _serializer.UdtWrapper);

        xElement = elemResult;
        return true;
    }

    private bool TrySerializeUnderlyingTypeIfNotEqualOrNullableOfObjectType(object obj, out XElement? xElement)
    {
        xElement = null;

        if (_serializer.UdtWrapper.UnderlyingType.EqualsOrIsNullableOf(obj.GetType())) return false;

        // this block of code runs if the serializer is instantiated with a
        // another base value such as System.Object but is provided with an
        // object of its child
        using var serializerPoolObject =
            _serializer.GetChildSerializer(obj.GetType(), _serializer.TypeNamespace, null, out var ser);
        var xDocument = ser.SerializeToXDocument(obj);
        xElement = xDocument.Root!;

        // do not pop from stack because the new internal serializer was sufficient for the whole serialization
        // and this instance of serializer did not do anything extra
        _serializer.FinalizeChildSerializer(ser, true, false);
        xElement.Name = _serializer.UdtWrapper.Alias;

        AddMetadataAttribute(xElement,
            _serializer.Options.Namespace.Uri + _serializer.Options.AttributeName.RealType,
            obj.GetType().FullName!, _serializer.DocumentDefaultNamespace);
        _serializer.XmlNamespaceManager.AddNamespacesToElement(xElement, _serializer.DocumentDefaultNamespace,
            _serializer.Options, _serializer.UdtWrapper);

        return true;
    }

    private void PushObjectToSerializationStack(object obj)
    {
        if (!obj.GetType().IsValueType) _serializer.SerializedStack.Push(obj);
    }

    private void SerializeFields(object obj)
    {
        var isAnythingFoundToSerialize = false;

        foreach (var member in _serializer.UdtWrapper.GetFieldsForSerialization())
        {
            var elementValue = member.GetValue(obj);
            if (!ShouldWriteMember(member, elementValue)) continue;

            isAnythingFoundToSerialize = true;

            if (member.HasNamespace)
                _serializer.XmlNamespaceManager.RegisterNamespace(member.Namespace, member.NamespacePrefix);

            var areOfSameType = AreOfSameType(obj, member, elementValue);

            var hasCustomSerializer =
                member.HasCustomSerializer || member.UdtWrapper.HasCustomSerializer;
            var isCollectionSerially = member.CollectionAttributeInstance is
                { SerializationType: YAXCollectionSerializationTypes.Serially };
            var isKnownType = member.IsKnownType;

            var serializationLocation = member.SerializationLocation;
            var canSerializeAsAttributeOrValue =
                areOfSameType || hasCustomSerializer || isCollectionSerially || isKnownType;

            // it gets true only for basic data types
            if (member.IsSerializedAsAttribute && canSerializeAsAttributeOrValue)
            {
                SerializeAsAttribute(member, elementValue, isCollectionSerially);
            }
            else if (member.IsSerializedAsValue && canSerializeAsAttributeOrValue)
            {
                SerializeAsValue(member, elementValue, serializationLocation, isCollectionSerially);
            }
            else
            {
                SerializeAsElement(member, elementValue, serializationLocation, areOfSameType);
            }
        }

        // This is an important step
        RemoveElementIfNecessary(isAnythingFoundToSerialize);
    }

    /// <summary>
    /// It checks, if all the members of an element have been serialized
    /// somewhere else, leaving the containing member empty.
    /// <para>
    /// In this case, remove that element by itself, <b>unless</b> the element is empty, because the
    /// corresponding object did not have any fields to serialize (e.g., DBNull, Random).
    /// Then keep the element.
    /// </para>
    /// </summary>
    /// <param name="isAnythingFoundToSerialize">
    /// A flag that indicates whether the object has any fields to be serialized.
    /// If it is <see langword="false" />, then we will not remove
    /// the containing element from the resulting XML.
    /// </param>
    private void RemoveElementIfNecessary(bool isAnythingFoundToSerialize)
    {
        if (_baseElement!.Parent != null &&
            XMLUtils.IsElementCompletelyEmpty(_baseElement) &&
            isAnythingFoundToSerialize)
            _baseElement.Remove();
    }

    private bool ShouldWriteMember(MemberWrapper member, object? elementValue)
    {
        return !IsNullButDoNotSerializeNull(member, elementValue) &&
               !IsDefaultButDoNotSerializeDefault(member, elementValue);
    }

    /// <summary>
    /// Checks whether the <paramref name="elementValue" /> is <see langword="null" />,
    /// and <see langword="null" /> shall not be serialized.
    /// </summary>
    /// <param name="elementValue"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    private bool IsNullButDoNotSerializeNull(MemberWrapper member, object? elementValue)
    {
        // ignore this member if it is null and we are not about to serialize null objects
        if (elementValue == null && _serializer.UdtWrapper.IsNotAllowedNullObjectSerialization)
            return true;

        if (elementValue == null &&
            member.IsAttributedAsDontSerializeIfNull)
            return true;


        return false;
    }

    /// <summary>
    /// Checks whether the <paramref name="elementValue" /> is <see langword="default" />,
    /// and <see langword="default" /> shall not be serialized.
    /// </summary>
    /// <param name="elementValue"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    private bool IsDefaultButDoNotSerializeDefault(MemberWrapper member, object? elementValue)
    {
        if (!_serializer.UdtWrapper.IsNotAllowedDefaultValueSerialization)
            return false;

        if (elementValue == null || ValueEquals(elementValue, ReflectionUtils.GetDefaultValue(member.MemberType)))
        {
           return true;
        }

        return false;
    }

    public static bool ValueEquals(object? objA, object? objB)
    {
        if (objA == objB)
        {
            return true;
        }
        if (objA == null || objB == null)
        {
            return false;
        }

        return objA.Equals(objB);
    }

    /// <summary>
    /// Creates a new <see cref="XElement" /> and adds it to the base element.
    /// </summary>
    /// <param name="className"></param>
    private void SetBaseElement(XName className)
    {
        if (_baseElement == null)
        {
            _baseElement = CreateElementWithNamespace(_serializer.UdtWrapper, className);
        }
        else
        {
            var baseElem = new XElement(className, default(object?));
            _baseElement.Add(baseElem);
            _baseElement = baseElem;
        }
    }

    /// <summary>
    /// Sets the base XML element. This method is used when an <see cref="YAXSerializer" />
    /// instantiates another <see cref="YAXSerializer" /> to serialize nested objects.
    /// Through this method the child objects have access to the already serialized elements of
    /// their parent.
    /// </summary>
    /// <param name="baseElement">The base XML element.</param>
    public void SetBaseElement(XElement baseElement)
    {
        _baseElement = baseElement;
    }

    private void SerializeAsElement(MemberWrapper member,
        object? elementValue, string serializationLocation, bool areOfSameType)
    {
        // Throw, if no parent element can be found
        var parentElem = GetParentElement(serializationLocation);

        AddComments(parentElem, member);

        if (member.HasCustomSerializer && !Locker.IsLocked(member.CustomSerializer!.Type))
        {
            InvokeCustomSerializer(member.CustomSerializer, parentElem, elementValue, member);
        }
        else if (member.UdtWrapper.HasCustomSerializer &&
                 !Locker.IsLocked(member.UdtWrapper.CustomSerializer!.Type))
        {
            InvokeCustomSerializer(member.UdtWrapper.CustomSerializer, parentElem, elementValue, member);
        }
        else if (member.IsKnownType && !Locker.IsLocked(member.KnownType!.Type))
        {
            using var _ = new Locker(member.KnownType!.Type);
            AddKnownTypeElement(parentElem, elementValue, member);
        }
        else
        {
            AddElement(parentElem, elementValue, member, areOfSameType);
        }
    }

    /// <summary>
    /// Adds a 'regular' <see cref="XElement" /> to the parent element.
    /// </summary>
    /// <param name="parElem"></param>
    /// <param name="elementValue"></param>
    /// <param name="member"></param>
    /// <param name="areOfSameType"></param>
    private void AddElement(XElement parElem, object? elementValue, MemberWrapper member, bool areOfSameType)
    {
        // make an element with the provided data
        var elemToAdd = MakeElement(parElem, member, elementValue, out var moveDescOnly,
            out var alreadyAdded);
        if (!areOfSameType)
        {
            var realType = elementValue?.GetType();

            var realTypeDefinition = member.GetRealTypeDefinition(realType);
            if (realTypeDefinition != null)
            {
                var alias = realTypeDefinition.Alias;
                if (string.IsNullOrEmpty(alias))
                {
                    var udtWrapper = UdtWrapperCache.Instance.GetOrAddItem(realType!, _serializer.Options);
                    alias = udtWrapper.Alias.LocalName;
                }

                elemToAdd.Name = XName.Get(alias, elemToAdd.Name.Namespace.NamespaceName);
            }
            else if (realType != null)
            {
                AddMetadataAttribute(elemToAdd,
                    _serializer.Options.Namespace.Uri + _serializer.Options.AttributeName.RealType,
                    realType.FullName!, _serializer.DocumentDefaultNamespace);
            }
        }

        if (moveDescOnly) // if only the descendants of the resulting element are going to be added ...
        {
            XMLUtils.MoveDescendants(elemToAdd, parElem);
            if (elemToAdd.Parent == parElem)
                elemToAdd.Remove();
        }
        else if (!alreadyAdded)
        {
            // see if such element already exists
            var existingElem = parElem.Element(member.Alias.OverrideNsIfEmpty(_serializer.TypeNamespace));
            if (existingElem == null)
            {
                // if not add the new element gracefully
                parElem.Add(elemToAdd);
            }
            else // if an element with our desired name already exists
            {
                if (ReflectionUtils.IsBasicType(member.MemberType))
                    existingElem.SetValue(elementValue ?? string.Empty);
                else
                    XMLUtils.MoveDescendants(elemToAdd, existingElem);
            }
        }
    }

    private void AddKnownTypeElement(XElement parElem, object? elementValue, MemberWrapper member)
    {
        var elemToFill = new XElement(member.Alias.OverrideNsIfEmpty(_serializer.TypeNamespace));
        parElem.Add(elemToFill);
        member.KnownType!.Serialize(elementValue, elemToFill,
            member.Namespace.IfEmptyThen(_serializer.TypeNamespace).IfEmptyThen(XNamespace.None),
            new SerializationContext(member, member.UdtWrapper, _serializer));
        if (member.PreservesWhitespace)
            XMLUtils.AddPreserveSpaceAttribute(elemToFill, _serializer.Options.Culture);
    }

    private XElement GetElementToFill(XElement parElem, MemberWrapper member)
    {
        var elemToFill = new XElement(member.Alias.OverrideNsIfEmpty(_serializer.TypeNamespace));
        parElem.Add(elemToFill);

        return elemToFill;
    }

    private void PreserveWhiteSpace(XElement elemToFill, MemberWrapper member)
    {
        if (member.PreservesWhitespace)
            XMLUtils.AddPreserveSpaceAttribute(elemToFill, _serializer.Options.Culture);
    }

    private void InvokeCustomSerializer(CustomSerializerWrapper customSerializerToUse, XElement parElem,
        object? elementValue, MemberWrapper member)
    {
        var elemToFill = GetElementToFill(parElem, member);
        customSerializerToUse.SerializeToElement(elementValue, elemToFill,
            new SerializationContext(member, member.UdtWrapper, _serializer));
        PreserveWhiteSpace(elemToFill, member);
    }

    private static void AddComments(XElement parElem, MemberWrapper member)
    {
        if (!member.HasComment) return;

        foreach (var comment in member.Comment!)
            parElem.Add(new XComment(comment));
    }

    /// <summary>
    /// Gets the parent <see cref="XElement" />.
    /// </summary>
    /// <param name="serializationLocation"></param>
    /// <returns></returns>
    /// <exception cref="YAXBadLocationException"></exception>
    private XElement GetParentElement(string serializationLocation)
    {
        // find the parent element from its location
        var parElem = XMLUtils.FindLocation(_baseElement!, serializationLocation);
        if (parElem != null) return parElem;

        // see if the location can be created
        if (!XMLUtils.CanCreateLocation(_baseElement!, serializationLocation))
            throw new YAXBadLocationException(serializationLocation);

        // try to create the location
        parElem = XMLUtils.CreateLocation(_baseElement!, serializationLocation);
        if (parElem == null)
            throw new YAXBadLocationException(serializationLocation);

        return parElem;
    }

    private void SerializeAsValue(MemberWrapper member, object? elementValue,
        string serializationLocation,
        bool isCollectionSerially)
    {
        // find the parent element from its location
        var parElem = GetParentElement(serializationLocation);

        // if control is moved here, it means that the parent
        // element has been found/created successfully

        string valueToSet;
        if (member.HasCustomSerializer && !Locker.IsLocked(member.CustomSerializer!.Type))
        {
            valueToSet = member.CustomSerializer!.SerializeToValue(elementValue,
                new SerializationContext(member, member.UdtWrapper, _serializer));
        }
        else if (member.UdtWrapper.HasCustomSerializer &&
                 !Locker.IsLocked(member.UdtWrapper.CustomSerializer!.Type))
        {
            valueToSet = member.UdtWrapper.CustomSerializer!.SerializeToValue(elementValue,
                new SerializationContext(member, member.UdtWrapper, _serializer));
        }
        else if (member.IsKnownType && !Locker.IsLocked(member.KnownType!.Type))
        {
            using var _ = new Locker(member.KnownType!.Type);
            var tempLoc = new XElement("temp");
            member.KnownType!.Serialize(elementValue, tempLoc, string.Empty,
                new SerializationContext(member, member.UdtWrapper, _serializer));
            valueToSet = tempLoc.Value;
        }
        else if (isCollectionSerially)
        {
            var tempLoc = new XElement("temp");
            var added = MakeCollectionElement(tempLoc, "name", elementValue,
                member.CollectionAttributeInstance, member.Format);
            valueToSet = added.Value;
        }
        else
        {
            valueToSet = elementValue.ToXmlValue(_serializer.Options.Culture);
        }

        parElem.Add(new XText(valueToSet.StripInvalidXmlChars(_stripInvalidXmlChars)));
        if (member.PreservesWhitespace)
            XMLUtils.AddPreserveSpaceAttribute(parElem, _serializer.Options.Culture);
    }

    private void SerializeAsAttribute(MemberWrapper member, object? elementValue,
        bool isCollectionSerially)
    {
        var hasCustomSerializer =
            member.HasCustomSerializer || member.UdtWrapper.HasCustomSerializer;
        var useElementValue = !(hasCustomSerializer || isCollectionSerially || member.IsKnownType);
        // Fill the base element with an attribute
        var attributeToUse = CreateAttribute(member, elementValue, useElementValue);
        if (member.HasCustomSerializer && !Locker.IsLocked(member.CustomSerializer!.Type))
        {
            member.CustomSerializer!.SerializeToAttribute(elementValue, attributeToUse,
                new SerializationContext(member, member.UdtWrapper, _serializer));
        }
        else if (member.UdtWrapper.HasCustomSerializer &&
                 !Locker.IsLocked(member.UdtWrapper.CustomSerializer!.Type))
        {
            member.UdtWrapper.CustomSerializer!.SerializeToAttribute(elementValue, attributeToUse,
                new SerializationContext(member, member.UdtWrapper, _serializer));
        }
        else if (member.IsKnownType)
        {
            // Serializing KnownTypes as attribute is not implemented
        }
        else if (isCollectionSerially)
        {
            var tempLoc = new XElement("temp");
            var added = MakeCollectionElement(tempLoc, "name", elementValue,
                member.CollectionAttributeInstance, member.Format);
            attributeToUse.Value = added.Value;
        }

        attributeToUse.Value = attributeToUse.Value.StripInvalidXmlChars(_stripInvalidXmlChars);

        // if member does not have any type wrappers
        // then it has been already populated with the CreateAttribute method
    }

    private XAttribute CreateAttribute(MemberWrapper member, object? elementValue, bool useElementValue)
    {
        var serializationLocation = member.SerializationLocation;

        if (XMLUtils.AttributeExists(_baseElement!, serializationLocation,
                member.Alias.OverrideNsIfEmpty(_serializer.TypeNamespace)))
        {
            throw new YAXAttributeAlreadyExistsException(member.Alias.LocalName);
        }

        var attribute = XMLUtils.CreateAttribute(_baseElement!,
            serializationLocation, member.Alias.OverrideNsIfEmpty(_serializer.TypeNamespace),
            useElementValue ? elementValue : string.Empty, _serializer.DocumentDefaultNamespace,
            _serializer.Options.Culture);

        _serializer.XmlNamespaceManager.RegisterNamespace(
            member.Alias.OverrideNsIfEmpty(_serializer.TypeNamespace).Namespace, null);

        if (attribute == null) throw new YAXBadLocationException(serializationLocation);

        return attribute;
    }

    /// <summary>
    /// Adds the namespace applying to the object type specified in <paramref name="wrapper" />
    /// to the <paramref name="className" />
    /// </summary>
    /// <param name="wrapper">The wrapper around the object who's namespace should be added</param>
    /// <param name="className">The root node of the document to which the namespace should be written</param>
    private XElement CreateElementWithNamespace(UdtWrapper wrapper, XName className)
    {
        var elemName = className.OverrideNsIfEmpty(wrapper.Namespace);
        if (elemName.Namespace == wrapper.Namespace)
            _serializer.XmlNamespaceManager.RegisterNamespace(elemName.Namespace, wrapper.NamespacePrefix);
        else
            _serializer.XmlNamespaceManager.RegisterNamespace(elemName.Namespace, string.Empty);

        return new XElement(elemName, default(object?));
    }

    /// <summary>
    /// Makes the element corresponding to the member specified.
    /// </summary>
    /// <param name="insertionLocation">The insertion location.</param>
    /// <param name="member">The member to serialize.</param>
    /// <param name="elementValue">The element value.</param>
    /// <param name="moveDescOnly">
    /// if set to <see langword="true" /> specifies that only the descendants of the resulting element should be
    /// added to the parent.
    /// </param>
    /// <param name="alreadyAdded">
    /// If set to <see langword="true" /> specifies the element returned is
    /// already added to the parent element and should not be added once more.
    /// </param>
    /// <returns></returns>
    private XElement MakeElement(XElement insertionLocation, MemberWrapper member, object? elementValue,
        out bool moveDescOnly, out bool alreadyAdded)
    {
        moveDescOnly = false;
        alreadyAdded = false;

        _serializer.XmlNamespaceManager.RegisterNamespace(member.Namespace, member.NamespacePrefix);

        XElement elemToAdd;
        if (member.IsTreatedAsDictionary)
        {
            elemToAdd = MakeDictionaryElement(insertionLocation,
                member.Alias.OverrideNsIfEmpty(_serializer.TypeNamespace),
                elementValue, member.DictionaryAttributeInstance, member.CollectionAttributeInstance,
                member.IsAttributedAsDontSerializeIfNull);

            (alreadyAdded, moveDescOnly) = HandleRecursiveCollection(insertionLocation, member, elemToAdd);
        }
        else if (member.IsTreatedAsCollection)
        {
            elemToAdd = MakeCollectionElement(insertionLocation,
                member.Alias.OverrideNsIfEmpty(_serializer.TypeNamespace),
                elementValue, member.CollectionAttributeInstance, member.Format);

            (alreadyAdded, moveDescOnly) = HandleRecursiveCollection(insertionLocation, member, elemToAdd);
        }
        else if (member.TextEmbedding != TextEmbedding.None && elementValue is string elementStringValue)
        {
            elemToAdd = MakeBaseElement(member.Alias.OverrideNsIfEmpty(_serializer.TypeNamespace),
                member.TextEmbedding, elementStringValue);
        }
        else
        {
            elemToAdd = MakeBaseElement(insertionLocation,
                member.Alias.OverrideNsIfEmpty(_serializer.TypeNamespace),
                elementValue, out alreadyAdded);
        }

        if (member.PreservesWhitespace)
            XMLUtils.AddPreserveSpaceAttribute(elemToAdd, _serializer.Options.Culture);

        return elemToAdd;
    }

    private static (bool alreadyAdded, bool moveDescOnly) HandleRecursiveCollection(XElement insertionLocation,
        MemberWrapper member, XElement elemToAdd)
    {
        var moveDescOnly = member.CollectionAttributeInstance is {
            SerializationType: YAXCollectionSerializationTypes.RecursiveWithNoContainingElement
        } && !elemToAdd.HasAttributes;

        var alreadyAdded = elemToAdd.Parent == insertionLocation;

        return (alreadyAdded, moveDescOnly);
    }

    /// <summary>
    /// Creates a dictionary element according to the specified options, as described
    /// by the attribute instances.
    /// </summary>
    /// <param name="insertionLocation">The insertion location.</param>
    /// <param name="elementName">Name of the element.</param>
    /// <param name="elementValue">The element value, corresponding to a dictionary object.</param>
    /// <param name="dicAttrInst">reference to the dictionary attribute instance.</param>
    /// <param name="collectionAttrInst">reference to collection attribute instance.</param>
    /// <param name="dontSerializeNull"><see langword="true" /> to not serialize <see langword="null" /></param>
    /// <returns>
    /// an instance of <see cref="XElement" /> which contains the dictionary object
    /// serialized properly
    /// </returns>
    private XElement MakeDictionaryElement(XElement? insertionLocation, XName elementName, object? elementValue,
        YAXDictionaryAttribute? dicAttrInst, YAXCollectionAttribute? collectionAttrInst, bool dontSerializeNull)
    {
        if (elementValue == null) return new XElement(elementName);

        if (!ReflectionUtils.IsIDictionary(elementValue.GetType(), out var keyType, out var valueType))
            throw new ArgumentException($"{nameof(elementValue)} must be a Dictionary", nameof(elementValue));

        // Serialize non-collection members
        var elemToAdd = SerializeUsingInternalSerializer(insertionLocation, elementName, elementValue);

        // now iterate through collection members

        // An IDictionary implements IEnumerable, so we can cast
        var dicInst = (IEnumerable) elementValue;

        (Type keyType, Type valueType, bool isKeyAttrib, bool isValueAttrib, bool isKeyContent, bool isValueContent,
            string? keyFormat, string? valueFormat, XName keyAlias, XName valueAlias) details =
                (keyType, valueType, false, false, false, false, null, null,
                    elementName.Namespace.IfEmptyThen(_serializer.TypeNamespace).IfEmptyThenNone() + "Key",
                    elementName.Namespace.IfEmptyThen(_serializer.TypeNamespace).IfEmptyThenNone() + "Value");

        _ = GetElementName(collectionAttrInst, elementName, out var eachElementName);

        if (dicAttrInst != null)
        {
            _ = GetElementName(dicAttrInst, elementName, out eachElementName);

            details = GetDictionaryAttributeDetails(elementName, dicAttrInst, details);
        }

        foreach (var obj in dicInst)
        {
            var elemChild = CreateDictionaryElement(elementName, eachElementName, obj, dontSerializeNull, details);

            elemToAdd.Add(elemChild);
        }

        return elemToAdd;
    }

    private XElement SerializeUsingInternalSerializer(XElement? insertionLocation, XName elementName,
        object elementValue)
    {
        using var serializerPoolObject =
            _serializer.GetChildSerializer(elementValue.GetType(), elementName.Namespace, insertionLocation,
                out var ser);

        var elem = ser.Serialization.SerializeBase(elementValue, elementName);
        _serializer.FinalizeChildSerializer(ser, true);

        return elem;
    }

    private XElement CreateDictionaryElement(XName elementName, XName? eachElementName, object obj,
        bool dontSerializeNull,
        (Type keyType, Type valueType, bool isKeyAttrib, bool isValueAttrib, bool isKeyContent, bool isValueContent,
            string? keyFormat, string? valueFormat, XName keyAlias, XName valueAlias) details)
    {
        var keyObj = obj.GetType().GetProperty("Key")!.GetValue(obj, null);
        var valueObj = obj.GetType().GetProperty("Value")!.GetValue(obj, null);

        var areKeyOfSameType = keyObj == null || keyObj.GetType().EqualsOrIsNullableOf(details.keyType);
        var areValueOfSameType = valueObj == null || valueObj.GetType().EqualsOrIsNullableOf(details.valueType);

        keyObj = ReflectionUtils.TryFormatObject(keyObj, details.keyFormat);
        valueObj = ReflectionUtils.TryFormatObject(valueObj, details.valueFormat);

        if (eachElementName == null)
        {
            eachElementName =
                StringUtils.RefineSingleElement(ReflectionUtils.GetTypeFriendlyName(obj.GetType()))!;
            eachElementName =
                eachElementName.OverrideNsIfEmpty(elementName.Namespace.IfEmptyThen(_serializer.TypeNamespace)
                    .IfEmptyThenNone());
        }

        var elemChild = new XElement(eachElementName, default(object?));

        AddDictionaryKey(keyObj, elemChild, areKeyOfSameType, details);

        AddDictionaryValue(valueObj, elemChild, areValueOfSameType, dontSerializeNull, details);

        return elemChild;
    }

    private void AddDictionaryValue(object? valueObj, XElement elemChild, bool areValueOfSameType,
        bool dontSerializeNull,
        (Type keyType, Type valueType, bool isKeyAttrib, bool isValueAttrib, bool isKeyContent, bool isValueContent,
            string? keyFormat, string? valueFormat, XName keyAlias, XName valueAlias) details)
    {
        if (details.isValueAttrib && areValueOfSameType)
        {
            elemChild.AddAttributeNamespaceSafe(details.valueAlias, valueObj, _serializer.DocumentDefaultNamespace,
                _serializer.Options.Culture);
        }
        else if (details.isValueContent && areValueOfSameType)
        {
            elemChild.AddXmlContent(valueObj, _serializer.Options.Culture);
        }
        else if (!(valueObj == null && dontSerializeNull))
        {
            var addedElem = AddObjectToElement(elemChild, details.valueAlias, valueObj);
            if (!areValueOfSameType)
            {
                if (addedElem.Parent == null)
                    // sometimes empty elements are removed because its members are serialized in
                    // other elements, therefore we need to make sure to re-add the element.
                    elemChild.Add(addedElem);

                AddMetadataAttribute(addedElem,
                    _serializer.Options.Namespace.Uri + _serializer.Options.AttributeName.RealType,
                    valueObj!.GetType().FullName!, _serializer.DocumentDefaultNamespace);
            }
        }
    }

    private void AddDictionaryKey(object? keyObj, XElement elemChild, bool areKeyOfSameType,
        (Type keyType, Type valueType, bool isKeyAttrib, bool isValueAttrib, bool isKeyContent, bool isValueContent,
            string? keyFormat, string? valueFormat, XName keyAlias, XName valueAlias) details)
    {
        if (details.isKeyAttrib && areKeyOfSameType)
        {
            elemChild.AddAttributeNamespaceSafe(details.keyAlias, keyObj, _serializer.DocumentDefaultNamespace,
                _serializer.Options.Culture);
        }
        else if (details.isKeyContent && areKeyOfSameType)
        {
            elemChild.AddXmlContent(keyObj, _serializer.Options.Culture);
        }
        else
        {
            var addedElem = AddObjectToElement(elemChild, details.keyAlias, keyObj);
            if (!areKeyOfSameType)
            {
                if (addedElem.Parent == null)
                    // sometimes empty elements are removed because its members are serialized in
                    // other elements, therefore we need to make sure to re-add the element.
                    elemChild.Add((object) addedElem);

                // keyObj can't be null, if areKeyOfSameType is false
                AddMetadataAttribute(addedElem,
                    _serializer.Options.Namespace.Uri + _serializer.Options.AttributeName.RealType,
                    keyObj!.GetType().FullName!, _serializer.DocumentDefaultNamespace);
            }
        }
    }

    private (Type keyType, Type valueType, bool isKeyAttrib, bool isValueAttrib, bool isKeyContent, bool
        isValueContent,
        string? keyFormat, string? valueFormat, XName keyAlias, XName valueAlias)
        GetDictionaryAttributeDetails(XName elementName,
            YAXDictionaryAttribute dicAttrInst,
            (Type keyType, Type valueType, bool isKeyAttrib, bool isValueAttrib, bool isKeyContent, bool
                isValueContent, string? keyFormat, string? valueFormat, XName keyAlias, XName valueAlias) details)
    {
        // Process key
        if (dicAttrInst.SerializeKeyAs == YAXNodeTypes.Attribute)
            details.isKeyAttrib = ReflectionUtils.IsBasicType(details.keyType);
        else if (dicAttrInst.SerializeKeyAs == YAXNodeTypes.Content)
            details.isKeyContent = ReflectionUtils.IsBasicType(details.keyType);

        // Process value
        if (dicAttrInst.SerializeValueAs == YAXNodeTypes.Attribute)
            details.isValueAttrib = ReflectionUtils.IsBasicType(details.valueType);
        else if (dicAttrInst.SerializeValueAs == YAXNodeTypes.Content)
            details.isValueContent = ReflectionUtils.IsBasicType(details.valueType);

        details.keyFormat = dicAttrInst.KeyFormatString;
        details.valueFormat = dicAttrInst.ValueFormatString;

        details.keyAlias = StringUtils.RefineSingleElement(dicAttrInst.KeyName ?? "Key")!;
        if (details.keyAlias.Namespace.IsEmpty())
            _serializer.XmlNamespaceManager.RegisterNamespace(details.keyAlias.Namespace, null);
        details.keyAlias = details.keyAlias.OverrideNsIfEmpty(
            elementName.Namespace.IfEmptyThen(_serializer.TypeNamespace).IfEmptyThenNone());

        details.valueAlias = StringUtils.RefineSingleElement(dicAttrInst.ValueName ?? "Value")!;
        if (details.valueAlias.Namespace.IsEmpty())
            _serializer.XmlNamespaceManager.RegisterNamespace(details.valueAlias.Namespace, null);
        details.valueAlias =
            details.valueAlias.OverrideNsIfEmpty(elementName.Namespace.IfEmptyThen(_serializer.TypeNamespace)
                .IfEmptyThenNone());

        return details;
    }

    private bool GetElementName(YAXCollectionAttribute? collectionAttrInst, XName elementName,
        out XName? eachElementName)
    {
        eachElementName = null;

        if (collectionAttrInst == null || string.IsNullOrEmpty(collectionAttrInst.EachElementName)) return false;

        eachElementName = StringUtils.RefineSingleElement(collectionAttrInst.EachElementName)!;
        if (eachElementName.Namespace.IsEmpty())
            _serializer.XmlNamespaceManager.RegisterNamespace(eachElementName.Namespace, string.Empty);
        eachElementName =
            eachElementName.OverrideNsIfEmpty(
                elementName.Namespace.IfEmptyThen(_serializer.TypeNamespace).IfEmptyThenNone());
        return true;
    }

    private bool GetElementName(YAXDictionaryAttribute? dicAttrInst, XName elementName,
        out XName? eachElementName)
    {
        eachElementName = null;
        if (dicAttrInst == null || dicAttrInst.EachPairName == null) return false;

        eachElementName = StringUtils.RefineSingleElement(dicAttrInst.EachPairName)!;
        if (eachElementName.Namespace.IsEmpty())
            _serializer.XmlNamespaceManager.RegisterNamespace(eachElementName.Namespace, string.Empty);
        eachElementName =
            eachElementName.OverrideNsIfEmpty(elementName.Namespace.IfEmptyThen(_serializer.TypeNamespace)
                .IfEmptyThenNone());
        return true;
    }

    /// <summary>
    /// Adds an element containing data related to the specified object, to an existing xml element.
    /// </summary>
    /// <param name="elem">The parent element.</param>
    /// <param name="alias">The name for the element to be added.</param>
    /// <param name="obj">
    /// The object corresponding to which an element is going to be added to
    /// an existing parent element.
    /// </param>
    /// <returns>the enclosing XML element.</returns>
    private XElement AddObjectToElement(XElement elem, XName alias, object? obj)
    {
        UdtWrapper? udt = null;
        if (obj != null)
            udt = UdtWrapperCache.Instance.GetOrAddItem(obj.GetType(), _serializer.Options);

        if (alias.LocalName == "" && udt != null)
            alias = udt.Alias.OverrideNsIfEmpty(_serializer.TypeNamespace);

        XElement elemToAdd;

        switch (udt)
        {
            case { IsTreatedAsDictionary: true }:
            {
                elemToAdd = MakeDictionaryElement(elem, alias, obj, null, null,
                    udt.IsNotAllowedNullObjectSerialization);
                if (elemToAdd.Parent != elem)
                    elem.Add(elemToAdd);
                break;
            }
            case { IsTreatedAsCollection: true }:
            {
                elemToAdd = MakeCollectionElement(elem, alias, obj, null, null);
                if (elemToAdd.Parent != elem)
                    elem.Add(elemToAdd);
                break;
            }
            case { IsEnum: true }:
            {
                elemToAdd = MakeBaseElement(elem, alias, udt.EnumWrapper!.GetAlias(obj!), out var alreadyAdded);
                if (!alreadyAdded)
                    elem.Add(elemToAdd);
                break;
            }
            default: // udt is null or none of the cases
            {
                elemToAdd = MakeBaseElement(elem, alias, obj, out var alreadyAdded);
                if (!alreadyAdded)
                    elem.Add(elemToAdd);
                break;
            }
        }

        return elemToAdd;
    }

    /// <summary>
    /// Serializes a collection object.
    /// </summary>
    /// <param name="insertionLocation">The insertion location.</param>
    /// <param name="elementName">Name of the element.</param>
    /// <param name="elementValue">The object to be serialized.</param>
    /// <param name="collectionAttrInst">The collection attribute instance.</param>
    /// <param name="format">formatting string, which is going to be applied to all members of the collection.</param>
    /// <returns>
    /// an instance of <see cref="XElement" /> which contains the collection
    /// serialized properly
    /// </returns>
    private XElement MakeCollectionElement(
        XElement? insertionLocation, XName elementName, object? elementValue,
        YAXCollectionAttribute? collectionAttrInst, string? format)
    {
        if (elementValue == null)
            return new XElement(elementName);

        if (elementValue is not IEnumerable collectionInst)
            throw new ArgumentException($"{nameof(elementName)} must be an IEnumerable", nameof(elementValue));

        // Serialize non-collection members
        var elemToAdd = SerializeUsingInternalSerializer(insertionLocation, elementName, collectionInst);

        // now iterate through collection members

        var serType = YAXCollectionSerializationTypes.Recursive;
        var separator = string.Empty;
        XName? eachElementName = null;

        if (collectionAttrInst != null)
        {
            serType = collectionAttrInst.SerializationType;
            separator = collectionAttrInst.SeparateBy;
            _ = GetElementName(collectionAttrInst, elementName, out eachElementName);
        }

        var colItemType = ReflectionUtils.GetCollectionItemType(collectionInst.GetType());
        var colItemsUdt = UdtWrapperCache.Instance.GetOrAddItem(colItemType, _serializer.Options);

        if (serType == YAXCollectionSerializationTypes.Serially && !ReflectionUtils.IsBasicType(colItemType))
            serType = YAXCollectionSerializationTypes.Recursive;

        if (serType == YAXCollectionSerializationTypes.Serially && elemToAdd.IsEmpty)
        {
            elemToAdd = MakeSerialTypeCollectionElement(insertionLocation, elementName, format, collectionInst,
                colItemsUdt, separator)!;
        }
        else
        {
            AddCollectionItems(elemToAdd, elementName, eachElementName!, collectionInst, colItemsUdt, format,
                colItemType);
        }

        var arrayDims = ReflectionUtils.GetArrayDimensions(collectionInst);
        if (arrayDims is { Length: > 1 })
            AddMetadataAttribute(elemToAdd,
                _serializer.Options.Namespace.Uri + _serializer.Options.AttributeName.Dimensions,
                StringUtils.GetArrayDimsString(arrayDims), _serializer.DocumentDefaultNamespace);

        return elemToAdd;
    }

    private void AddCollectionItems(XElement elemToAdd, XName elementName, XName? eachElementName,
        IEnumerable collectionInst, UdtWrapper colItemsUdt, string? format, Type colItemType)
    {
        foreach (var obj in collectionInst)
        {
            var objToAdd = ReflectionUtils.TryFormatObject(obj, format);
            var curElemName = eachElementName ?? colItemsUdt.Alias;
            var itemElem = AddObjectToElement(elemToAdd, curElemName.OverrideNsIfEmpty(elementName.Namespace),
                objToAdd);

            if (AreOfSameType(obj, colItemType)) continue;

            // i.e., it has been removed, e.g., because all its members have been serialized outside the element
            if (itemElem.Parent == null)
                elemToAdd.Add(itemElem); // return it back

            AddMetadataAttribute(itemElem,
                _serializer.Options.Namespace.Uri + _serializer.Options.AttributeName.RealType,
                obj.GetType().FullName!, _serializer.DocumentDefaultNamespace);
        }
    }

    /// <summary>
    /// Are element value and the member declared type the same?
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="member"></param>
    /// <param name="elementValue"></param>
    /// <returns></returns>
    private static bool AreOfSameType(object? obj, MemberWrapper member, object? elementValue)
    {
        var originalValue = member.GetOriginalValue(obj, null);
        return (elementValue == null && originalValue == null)
               || (originalValue != null && member.MemberType.EqualsOrIsNullableOf(originalValue.GetType()));
    }

    /// <summary>
    /// Are <paramref name="obj" />object type and type <paramref name="toCompare" /> the same?
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="toCompare"></param>
    /// <returns></returns>
    private static bool AreOfSameType(object? obj, Type toCompare)
    {
        return obj == null || obj.GetType().EqualsOrIsNullableOf(toCompare);
    }

    private XElement? MakeSerialTypeCollectionElement(XElement? insertionLocation, XName elementName,
        string? format,
        IEnumerable collectionInst, UdtWrapper colItemsUdt, string separator)
    {
        using var pooledObject = StringBuilderPool.Instance.Get(out var sb);

        var isFirst = true;
        foreach (var obj in collectionInst)
        {
            object? objToAdd;
            if (colItemsUdt.IsEnum)
                objToAdd = colItemsUdt.EnumWrapper!.GetAlias(obj);
            else if (format != null)
                objToAdd = ReflectionUtils.TryFormatObject(obj, format);
            else
                objToAdd = obj;

            if (isFirst)
            {
                sb.Append(objToAdd.ToXmlValue(_serializer.Options.Culture));
                isFirst = false;
            }
            else
            {
                sb.AppendFormat(_serializer.Options.Culture, "{0}{1}", separator, objToAdd);
            }
        }

        var elemToAdd = MakeBaseElement(insertionLocation, elementName, sb.ToString(), out var alreadyAdded);
        return !alreadyAdded ? elemToAdd : null;
    }

    /// <summary>
    /// Makes an XML element with the specified name, corresponding to the object specified.
    /// </summary>
    /// <param name="insertionLocation">The insertion location.</param>
    /// <param name="name">The name of the element.</param>
    /// <param name="value">The object to be serialized in an XML element.</param>
    /// <param name="alreadyAdded">
    /// If set to <see langword="true" /> specifies the element returned is
    /// already added to the parent element and should not be added once more.
    /// </param>
    /// <returns>
    /// An instance of <see cref="XElement" /> which will contain the serialized object.
    /// </returns>
    private XElement MakeBaseElement(XElement? insertionLocation, XName name, object? value, out bool alreadyAdded)
    {
        alreadyAdded = false;
        if (value == null || ReflectionUtils.IsBasicType(value.GetType()))
        {
            value = value?.ToXmlValue(_serializer.Options.Culture).StripInvalidXmlChars(_stripInvalidXmlChars);

            return new XElement(name, value);
        }

        if (ReflectionUtils.IsStringConvertibleIFormattable(value.GetType()))
        {
            var elementValue = (string) value.GetType().InvokeMember("ToString", BindingFlags.InvokeMethod, null,
                value, Array.Empty<object>())!;
            return new XElement(name, elementValue.StripInvalidXmlChars(_stripInvalidXmlChars));
        }

        var elem = SerializeUsingInternalSerializer(insertionLocation, name, value);
        alreadyAdded = true;
        return elem;
    }

    /// <summary>
    /// The equivalent to the IsBasicType block of <see cref="MakeBaseElement(XElement,XName,object,out bool)" />,
    /// but specialized for a string field/property flagged with the <see cref="YAXTextEmbeddingAttribute" />.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="embedding"></param>
    /// <param name="value"></param>
    /// <returns>
    /// An instance of <see cref="XElement" /> which will contain the serialized object.
    /// </returns>
    private XElement MakeBaseElement(XName name, TextEmbedding embedding, string value)
    {
        var elem = new XElement(name);
        switch (embedding)
        {
            case TextEmbedding.CData:
                elem.Add(new XCData(value.StripInvalidXmlChars(_stripInvalidXmlChars)));
                break;
            case TextEmbedding.Base64:
                elem.Add(new XText(value.ToBase64(System.Text.Encoding.UTF8)!));
                break;
            /*
                TextEmbedding.None and null values uses standard element serialization,
                which is not handled in this method.
            */
        }

        return elem;
    }

    private void AddMetadataAttribute(XElement parent, XName attrName, object attrValue,
        XNamespace documentDefaultNamespace)
    {
        if (!_serializer.UdtWrapper.SuppressMetadataAttributes)
        {
            parent.AddAttributeNamespaceSafe(attrName, attrValue, documentDefaultNamespace,
                _serializer.Options.Culture);
            _serializer.XmlNamespaceManager.RegisterNamespace(_serializer.Options.Namespace.Uri,
                _serializer.Options.Namespace.Prefix);
        }
    }
}
