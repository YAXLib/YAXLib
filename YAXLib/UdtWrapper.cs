// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using YAXLib.Attributes;
using YAXLib.Caching;
using YAXLib.Customization;
using YAXLib.Enums;
using YAXLib.KnownTypes;
using YAXLib.Options;

namespace YAXLib;

/// <summary>
/// A wrapper class around user-defined types, for quick access to their YAXLib related attributes
/// </summary>
internal class UdtWrapper
{
    /// <summary>
    /// The <see cref="SerializerOptions" /> to use for this instance
    /// and <see cref="MemberWrapper" />s created by this instance.
    /// </summary>
    private readonly SerializerOptions _serializerOptions;

    /// <summary>
    /// the underlying type for this instance of <c>TypeWrapper</c>
    /// </summary>
    private readonly Type _udtType;

    /// <summary>
    /// Alias for the type
    /// </summary>
    private XName _alias;

    /// <summary>
    /// reference to an instance of <c>EnumWrapper</c> in case that the current instance is an enum.
    /// </summary>
    private EnumWrapper? _enumWrapper;

    /// <summary>
    /// the namespace associated with this element
    /// </summary>
    private XNamespace _namespace = XNamespace.None;

    /// <summary>
    /// supports customization of type properties and name
    /// </summary>
    private readonly ITypeInspector _typeInspector;

    /// <summary>
    /// Initializes a new instance of the <see cref="UdtWrapper" /> class.
    /// </summary>
    /// <param name="udtType">
    /// The underlying type to create the wrapper around.
    /// If the the type is <see cref="Nullable" />, the underlying type of the <see cref="Nullable" /> is used.
    /// </param>
    /// <param name="serializerOptions">The <see cref="SerializerOptions" /> to use.</param>
    public UdtWrapper(Type udtType, SerializerOptions serializerOptions)
    {
        _serializerOptions = serializerOptions;
        _udtType = ReflectionUtils.IsNullable(udtType, out var nullableUnderlyingType)
            ? nullableUnderlyingType!
            : udtType;
        IsCollectionType = ReflectionUtils.IsCollectionType(_udtType);
        IsDictionaryType = ReflectionUtils.IsIDictionary(_udtType);

        _ = WellKnownTypes.TryGetKnownType(_udtType, out var knownType);
        KnownType = knownType;
        _typeInspector = serializerOptions.TypeInspector;

        _alias = Alias = StringUtils.RefineSingleElement(_typeInspector.GetTypeName(_udtType, serializerOptions))!;

        Comment = null;
        FieldsToSerialize = YAXSerializationFields.PublicPropertiesOnly;
        IsAttributedAsNotCollection = false;

        SerializationOptions = serializerOptions.SerializationOptions;

        foreach (var attr in _udtType.GetCustomAttributes(true))
            if (attr is IYaxTypeLevelAttribute typeLevelAttribute)
                typeLevelAttribute.Setup(this);
    }

    /// <summary>
    /// Gets the alias of the type.
    /// </summary>
    /// <value>The alias of the type.</value>
    public XName Alias
    {
        get
        {
            return _alias;
        }

        internal set
        {
            if (Namespace.IsEmpty())
            {
                _alias = Namespace + value.LocalName;
            }
            else
            {
                _alias = value;
                if (_alias.Namespace.IsEmpty())
                    _namespace = _alias.Namespace;
            }
        }
    }

    /// <summary>
    /// Gets an array of comments for the underlying type.
    /// </summary>
    /// <value>The array of comments for the underlying type.</value>
    public string[]? Comment { get; internal set; }

    /// <summary>
    /// Gets the fields to be serialized.
    /// </summary>
    /// <value>The fields to be serialized.</value>
    public YAXSerializationFields FieldsToSerialize { get; internal set; }

    /// <inheritdoc cref="YAXSerializableTypeAttribute.IncludePrivateMembersFromBaseTypes" />
    /// >
    public bool IncludePrivateMembersFromBaseTypes { get; internal set; }

    /// <summary>
    /// Gets the serialization options.
    /// </summary>
    /// <value>The serialization options.</value>
    public YAXSerializationOptions SerializationOptions { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance is attributed as not collection.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is attributed as not collection; otherwise, <c>false</c>.
    /// </value>
    public bool IsAttributedAsNotCollection { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether this instance has comment.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance has comment; otherwise, <c>false</c>.
    /// </value>
    public bool HasComment => Comment is { Length: > 0 };

    /// <summary>
    /// Gets the underlying type corresponding to this wrapper.
    /// </summary>
    /// <value>The underlying type corresponding to this wrapper.</value>
    public Type UnderlyingType => _udtType;

    /// <summary>
    /// Gets a value indicating whether the underlying type is a known-type
    /// </summary>
    public bool IsKnownType => KnownType != null;

    /// <summary>
    /// Gets the <see cref="IKnownType" />.
    /// </summary>
    public IKnownType? KnownType { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this instance wraps around an enum.
    /// </summary>
    /// <value><c>true</c> if this instance wraps around an enum; otherwise, <c>false</c>.</value>
    public bool IsEnum => _udtType.IsEnum;

    /// <summary>
    /// Gets the enum wrapper, provided that this instance wraps around an enum.
    /// </summary>
    /// <value>The enum wrapper, provided that this instance wraps around an enum.</value>
    public EnumWrapper? EnumWrapper
    {
        get
        {
            if (IsEnum)
            {
                return _enumWrapper ??= new EnumWrapper(_udtType);
            }

            return null;
        }
    }

    /// <summary>
    /// Determines whether serialization of null objects is not allowed.
    /// </summary>
    /// <returns>
    /// <c>true</c> if serialization of null objects is not allowed; otherwise, <c>false</c>.
    /// </returns>
    public bool IsNotAllowedNullObjectSerialization =>
        (SerializationOptions & YAXSerializationOptions.DontSerializeNullObjects) ==
        YAXSerializationOptions.DontSerializeNullObjects;

    /// <summary>
    /// Determines whether serialization of default values is not allowed.
    /// </summary>
    /// <returns>
    /// <c>true</c> if serialization of default values is not allowed; otherwise, <c>false</c>.
    /// </returns>
    public bool IsNotAllowedDefaultValueSerialization =>
        (SerializationOptions & YAXSerializationOptions.DoNotSerializeDefaultValues) ==
        YAXSerializationOptions.DoNotSerializeDefaultValues;

    /// <summary>
    /// Determines whether cycling references must be ignored, or an exception needs to be thrown
    /// </summary>
    public bool ThrowUponSerializingCyclingReferences =>
        (SerializationOptions & YAXSerializationOptions.ThrowUponSerializingCyclingReferences) ==
        YAXSerializationOptions.ThrowUponSerializingCyclingReferences;

    /// <summary>
    /// Determines whether properties with no setters should be serialized
    /// </summary>
    public bool DoNotSerializePropertiesWithNoSetter =>
        (SerializationOptions & YAXSerializationOptions.DontSerializePropertiesWithNoSetter) ==
        YAXSerializationOptions.DontSerializePropertiesWithNoSetter;

    /// <summary>
    /// Never add YAXLib metadata attributes (e.g., 'yaxlib:realtype') to the serialized XML (even when they would be
    /// required for deserialization.)
    /// Useful when generating XML intended for another system's consumption.
    /// </summary>
    public bool SuppressMetadataAttributes =>
        (SerializationOptions & YAXSerializationOptions.SuppressMetadataAttributes) ==
        YAXSerializationOptions.SuppressMetadataAttributes;

    /// <summary>
    /// Gets a value indicating whether this instance wraps around a collection type.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance wraps around a collection type; otherwise, <c>false</c>.
    /// </value>
    public bool IsCollectionType { get; }

    /// <summary>
    /// Gets a value indicating whether this instance wraps around a dictionary type.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance wraps around a dictionary type; otherwise, <c>false</c>.
    /// </value>
    public bool IsDictionaryType { get; }

    /// <summary>
    /// Gets a value indicating whether this instance is treated as collection.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is treated as collection; otherwise, <c>false</c>.
    /// </value>
    public bool IsTreatedAsCollection => !IsAttributedAsNotCollection && IsCollectionType;

    /// <summary>
    /// Gets a value indicating whether this instance is treated as dictionary.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is treated as dictionary; otherwise, <c>false</c>.
    /// </value>
    public bool IsTreatedAsDictionary => !IsAttributedAsNotCollection && IsDictionaryType;

    /// <summary>
    /// Gets the collection attribute instance.
    /// </summary>
    /// <value>The collection attribute instance.</value>
    public YAXCollectionAttribute? CollectionAttributeInstance { get; internal set; }

    /// <summary>
    /// Gets the dictionary attribute instance.
    /// </summary>
    /// <value>The dictionary attribute instance.</value>
    public YAXDictionaryAttribute? DictionaryAttributeInstance { get; internal set; }

    /// <summary>
    /// Gets or sets the wrapper for an <see cref="ICustomSerializer{T}" /> instance.
    /// </summary>
    /// <value>The wrapper for an <see cref="ICustomSerializer{T}" /> instance.</value>
    public CustomSerializerWrapper? CustomSerializer { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether this instance has custom serializer.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance has custom serializer; otherwise, <c>false</c>.
    /// </value>
    public bool HasCustomSerializer => CustomSerializer != null;

    /// <summary>
    /// If <see langword="true" />, the 'xml:space="preserve"' attribute will be added to the specified element.
    /// </summary>
    public bool PreservesWhitespace { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether this instance has a custom namespace
    /// defined for it through the <see cref="YAXNamespaceAttribute" /> attribute.
    /// </summary>
    public bool HasNamespace => Namespace.IsEmpty();

    /// <summary>
    /// Gets the namespace associated with this element.
    /// </summary>
    /// <remarks>
    /// If <see cref="HasNamespace" /> is <c>false</c> then this should
    /// be inherited from any parent elements.
    /// </remarks>
    public XNamespace Namespace
    {
        get { return _namespace; }

        internal set
        {
            _namespace = value;
            // explicit namespace definition overrides namespace definitions in SerializeAs attributes.
            _alias = _namespace + _alias.LocalName;
        }
    }

    /// <summary>
    /// Gets the namespace prefix associated with this element
    /// </summary>
    /// <remarks>
    /// If <see cref="HasNamespace" /> is <c>false</c> then this should
    /// be inherited from any parent elements.
    /// If this is <c>null</c>, then it should be assumed that the specified
    /// <see cref="Namespace" /> (if it is present) is the default namespace.
    /// It should also be noted that if a namespace is not provided for the
    /// entire document (default namespace) and yet a default namespace is
    /// provided for one element that an exception should be thrown (since
    /// setting a default namespace for that element would make it apply to
    /// the whole document).
    /// </remarks>
    public string NamespacePrefix { get; internal set; } = string.Empty;

    /// <inheritdoc />
    public override string ToString()
    {
        return _udtType.ToString();
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is UdtWrapper udtWrapper)
        {
            return _udtType == udtWrapper.UnderlyingType;
        }

        return false;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return _udtType.GetHashCode();
    }

    /// <summary>
    /// Used by attributes when setting <see cref="YAXSerializationOptions" />.
    /// </summary>
    /// <param name="serializationOptions"></param>
    internal void SetSerializationOptionsFromAttribute(YAXSerializationOptions serializationOptions)
    {
        SerializationOptions = serializationOptions;
    }

    /// <summary>
    /// Gets the sequence of fields to be serialized to XML for the specified type.
    /// This sequence is retrieved according to the specified field types.
    /// <para>See also: <seealso cref="GetFieldsForDeserialization" /></para>
    /// </summary>
    /// <returns>The sequence of fields to be serialized for the specified type</returns>
    internal IEnumerable<MemberWrapper> GetFieldsForSerialization()
    {
        return GetFieldsToBeSerialized().Where(m => m.CanRead);
    }

    /// <summary>
    /// Gets the sequence of fields to be deserialized from XML for the specified type.
    /// This sequence is retrieved according to the specified field types.
    /// <para>See also: <seealso cref="GetFieldsForSerialization" /></para>
    /// </summary>
    /// <param name="sorted">
    /// If <see langword="true" /> (default), the returned fields will be sorted by
    /// <see cref="MemberWrapper.Order" />.
    /// </param>
    /// <returns>The sequence of fields to be deserialized for the specified type</returns>
    internal IEnumerable<MemberWrapper> GetFieldsForDeserialization(bool sorted = true)
    {
        var fieldsForDeserialization = GetFieldsToBeSerialized(sorted).Where(m => m.CanWrite);
        
        return fieldsForDeserialization;
    }

    /// <summary>
    /// Gets the sequence of fields to be serialized or to be deserialized for the specified type.
    /// This sequence is retrieved according to the field types specified by the user.
    /// </summary>
    /// <param name="sorted">
    /// If <see langword="true" /> (default), the returned fields will be sorted by
    /// <see cref="MemberWrapper.Order" />.
    /// </param>
    /// <returns>The sequence of fields to be de/serialized for the specified type</returns>
    private IEnumerable<MemberWrapper> GetFieldsToBeSerialized(bool sorted = true)
    {
        var members = GetMembers();

        return sorted ? members.OrderBy(mr => mr.Order) : members;
    }

    private IEnumerable<MemberWrapper> GetMembers()
    {
        if (MemberWrapperCache.Instance.TryGetItem((UnderlyingType, _serializerOptions), out var memberWrappers))
            return memberWrappers;

        var members = _typeInspector.GetMembers(UnderlyingType, _serializerOptions, IncludePrivateMembersFromBaseTypes);

        foreach (var member in members)
        {
            memberWrappers.Add(new MemberWrapper(member, _serializerOptions));
        }

        // Filter the members that are actually subject to be serialized
        // according to settings and attributes.
        // IsAllowedToBeSerialized evaluates only booleans, no reflection.
        memberWrappers = memberWrappers
            .Where(mr => mr.IsAllowedToBeSerialized(FieldsToSerialize, DoNotSerializePropertiesWithNoSetter) &&
                         !mr.IsAttributedAsDontSerialize)
            .ToList();

        _ = MemberWrapperCache.Instance.TryAdd((UnderlyingType, _serializerOptions), memberWrappers);

        return memberWrappers;
    }

    /// <summary>
    /// Gets the default <see cref="XNamespace" /> for the document, it is defined,
    /// or an empty <see cref="XNamespace" />.
    /// </summary>
    /// <returns>
    /// The default <see cref="XNamespace" /> for the document, it is defined,
    /// or an empty <see cref="XNamespace" />.
    /// </returns>
    internal XNamespace FindDocumentDefaultNamespace()
    {
        if (HasNamespace && string.IsNullOrEmpty(NamespacePrefix))
            // it has a default namespace defined (one without a prefix)
            return Namespace;

        return XNamespace.Get(string.Empty);
    }
}
