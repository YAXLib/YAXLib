// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using YAXLib.Attributes;
using YAXLib.Enums;
using YAXLib.Exceptions;

namespace YAXLib
{
    /// <summary>
    ///     A wrapper class for members which only can be properties or member variables
    /// </summary>
    internal class MemberWrapper
    {
        /// <summary>
        ///     <c>true</c> if this instance corresponds to a property, <c>false</c>
        ///     if it corresponds to a field (i.e., a member variable)
        /// </summary>
        private readonly bool _isProperty;

        /// <summary>
        ///     the member type of the underlying member
        /// </summary>
        private readonly Type _memberType;

        private readonly List<YAXCollectionItemTypeAttribute> _possibleCollectionItemRealTypes = new();

        private readonly List<YAXTypeAttribute> _possibleRealTypes = new();

        /// <summary>
        ///     The alias specified by the user
        /// </summary>
        private XName? _alias;

        /// <summary>
        ///     specifies whether this member is going to be serialized as an attribute
        /// </summary>
        private bool _isSerializedAsAttribute;

        /// <summary>
        ///     specifies whether this member is going to be serialized as a value for another element
        /// </summary>
        private bool _isSerializedAsValue;

        /// <summary>
        ///     The xml-namespace this member is going to be serialized under.
        /// </summary>
        private XNamespace _namespace = XNamespace.None;

        /// <summary>
        ///     The location of the serialization
        /// </summary>
        private string _serializationLocation = "";

        /// <summary>
        ///     Initializes a new instance of the <see cref="MemberWrapper" /> class.
        /// </summary>
        /// <param name="memberInfo">The member-info to build this instance from.</param>
        /// <param name="callerSerializer">The caller serializer.</param>
        public MemberWrapper(MemberInfo memberInfo, YAXSerializer? callerSerializer)
        {
            Order = int.MaxValue;

            if (!(memberInfo.MemberType == MemberTypes.Property || memberInfo.MemberType == MemberTypes.Field))
                throw new Exception("Member must be either property or field");

            MemberInfo = memberInfo;
            _isProperty = memberInfo.MemberType == MemberTypes.Property;

            Alias = StringUtils.RefineSingleElement(MemberInfo.Name);
            if (_isProperty)
            {
                PropertyInfo = (PropertyInfo) memberInfo;
                _memberType = PropertyInfo.PropertyType;
            }
            else
            {
                FieldInfo = (FieldInfo) memberInfo;
                _memberType = FieldInfo.FieldType;
            }

            MemberTypeWrapper = TypeWrappersPool.Pool.GetTypeWrapper(MemberType, callerSerializer);
            if (MemberTypeWrapper.HasNamespace)
            {
                Namespace = MemberTypeWrapper.Namespace;
                NamespacePrefix = MemberTypeWrapper.NamespacePrefix;
            }

            InitInstance();
            
            TreatErrorsAs = callerSerializer?.Options.ExceptionBehavior ?? YAXExceptionTypes.Error;

            // discover YAXCustomSerializerAttributes earlier, because some other attributes depend on it
            var attrsToProcessEarlier = new HashSet<Type>
                {typeof(YAXCustomSerializerAttribute), typeof(YAXCollectionAttribute)};
            
            foreach (var attrType in attrsToProcessEarlier)
            {
                var customSerAttrs = Attribute.GetCustomAttributes(MemberInfo, attrType, true);
                foreach (var attr in customSerAttrs)
                {
                    if (attr is IYaxMemberLevelAttribute memberAttr)
                        memberAttr.Setup(this);
                }
            }

            foreach (var attr in Attribute.GetCustomAttributes(MemberInfo, true))
            {
                // no need to process, it has been processed earlier
                if (attrsToProcessEarlier.Contains(attr.GetType()))
                    continue;

                if (attr is IYaxMemberLevelAttribute memberAttr)
                    memberAttr.Setup(this);
            }

            // now override some values from member-type-wrapper into member-wrapper
            // if member-type has collection attributes while the member itself does not have them, 
            // then use those of the member-type
            if (CollectionAttributeInstance == null && MemberTypeWrapper.CollectionAttributeInstance != null)
                CollectionAttributeInstance = MemberTypeWrapper.CollectionAttributeInstance;
            
            if (DictionaryAttributeInstance == null && MemberTypeWrapper.DictionaryAttributeInstance != null)
                DictionaryAttributeInstance = MemberTypeWrapper.DictionaryAttributeInstance;
        }

        /// <summary>
        ///     Gets the alias specified for this member.
        /// </summary>
        /// <value>The alias specified for this member.</value>
        public XName? Alias
        {
            get { return _alias; }

            internal set
            {
                if (Namespace.IsEmpty())
                {
                    _alias = Namespace + value?.LocalName;
                }
                else
                {
                    _alias = value;
                    if (_alias != null && _alias.Namespace.IsEmpty())
                        _namespace = _alias.Namespace;
                }
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the member corresponding to this instance can be read from.
        /// </summary>
        /// <value><c>true</c> if the member corresponding to this instance can be read from; otherwise, <c>false</c>.</value>
        public bool CanRead
        {
            get
            {
                if (_isProperty)
                    return PropertyInfo != null && PropertyInfo.CanRead;
                return true;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the member corresponding to this instance can be written to.
        /// </summary>
        /// <value><c>true</c> if the member corresponding to this instance can be written to; otherwise, <c>false</c>.</value>
        public bool CanWrite
        {
            get
            {
                if (_isProperty)
                    return PropertyInfo != null && PropertyInfo.CanWrite;
                return true;
            }
        }

        /// <summary>
        ///     Gets an array of comment lines.
        /// </summary>
        /// <value>The comment lines.</value>
        public string[]? Comment { get; internal set; }

        /// <summary>
        ///     Gets the default value for this instance.
        /// </summary>
        /// <value>The default value for this instance.</value>
        public object? DefaultValue { get; internal set; }

        /// <summary>
        ///     Gets the format specified for this value; <c>null</c> if no format is specified.
        /// </summary>
        /// <value>the format specified for this value; <c>null</c> if no format is specified.</value>
        public string? Format { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether this instance has comments.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has comments; otherwise, <c>false</c>.
        /// </value>
        public bool HasComment => Comment != null && Comment.Length > 0;

        /// <summary>
        ///     Gets a value indicating whether this instance has format values specified
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has format values specified; otherwise, <c>false</c>.
        /// </value>
        public bool HasFormat =>
            // empty string may be considered as a valid format
            Format != null;

        /// <summary>
        ///     Gets a value indicating whether this instance is attributed as dont serialize.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is attributed as dont serialize; otherwise, <c>false</c>.
        /// </value>
        public bool IsAttributedAsDontSerialize { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is attributed as not-collection.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is attributed as not-collection; otherwise, <c>false</c>.
        /// </value>
        public bool IsAttributedAsNotCollection { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is attributed as serializable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is attributed as serializable; otherwise, <c>false</c>.
        /// </value>
        public bool IsAttributedAsSerializable { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is attributed as dont serialize when null.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is attributed as don't serialize when null; otherwise, <c>false</c>.
        /// </value>
        public bool IsAttributedAsDontSerializeIfNull { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is serialized as an XML attribute.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is serialized as an XML attribute; otherwise, <c>false</c>.
        /// </value>
        public bool IsSerializedAsAttribute
        {
            get { return _isSerializedAsAttribute; }

            internal set
            {
                _isSerializedAsAttribute = value;
                if (value)
                    // a field cannot be both serialized as an attribute and a value
                    _isSerializedAsValue = false;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is serialized as a value for an element.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is serialized as a value for an element; otherwise, <c>false</c>.
        /// </value>
        public bool IsSerializedAsValue
        {
            get { return _isSerializedAsValue; }

            internal set
            {
                _isSerializedAsValue = value;
                if (value)
                    // a field cannot be both serialized as an attribute and a value
                    _isSerializedAsAttribute = false;
            }
        }


        /// <summary>
        ///     Gets a value indicating whether this instance is serialized as an XML element.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is serialized as an XML element; otherwise, <c>false</c>.
        /// </value>
        public bool IsSerializedAsElement
        {
            get { return !IsSerializedAsAttribute && !IsSerializedAsValue; }

            internal set
            {
                if (value)
                {
                    _isSerializedAsAttribute = false;
                    _isSerializedAsValue = false;
                }
            }
        }

        /// <summary>
        ///     Gets the type of the member.
        /// </summary>
        /// <value>The type of the member.</value>
        public Type MemberType => _memberType;

        /// <summary>
        ///     Gets the <see cref="FieldInfo"/> of a field, if the member is a field.
        /// </summary>
        public FieldInfo? FieldInfo { get; }

        /// <summary>
        ///     Gets the <see cref="MemberInfo"/>.
        /// </summary>
        public MemberInfo MemberInfo { get; }

        /// <summary>
        ///     Gets the <see cref="PropertyInfo"/> of a field, if the member is a property.
        /// </summary>
        public PropertyInfo? PropertyInfo { get; }

        /// <summary>
        ///     Gets the type wrapper instance corresponding to the member-type of this instance.
        /// </summary>
        /// <value>The type wrapper instance corresponding to the member-type of this instance.</value>
        public UdtWrapper MemberTypeWrapper { get; }

        /// <summary>
        ///     Gets a value indicating whether the underlying type is a known-type
        /// </summary>
        public bool IsKnownType => MemberTypeWrapper.IsKnownType;

        /// <summary>
        ///     Gets the original of this member (as opposed to its alias).
        /// </summary>
        /// <value>The original of this member .</value>
        public string OriginalName => MemberInfo.Name;

        /// <summary>
        ///     Gets the serialization location.
        /// </summary>
        /// <value>The serialization location.</value>
        public string SerializationLocation
        {
            get => _serializationLocation;

            internal set => _serializationLocation = StringUtils.RefineLocationString(value);
        }

        /// <summary>
        ///     Gets the exception type for this instance in case of encountering missing values
        /// </summary>
        /// <value>The exception type for this instance in case of encountering missing values</value>
        public YAXExceptionTypes TreatErrorsAs { get; internal set; }

        /// <summary>
        ///     Gets the collection attribute instance.
        /// </summary>
        /// <value>The collection attribute instance.</value>
        public YAXCollectionAttribute? CollectionAttributeInstance { get; internal set; }

        /// <summary>
        ///     Gets the dictionary attribute instance.
        /// </summary>
        /// <value>The dictionary attribute instance.</value>
        public YAXDictionaryAttribute? DictionaryAttributeInstance { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is treated as a collection.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is treated as a collection; otherwise, <c>false</c>.
        /// </value>
        public bool IsTreatedAsCollection => !IsAttributedAsNotCollection && MemberTypeWrapper.IsTreatedAsCollection;

        /// <summary>
        ///     Gets a value indicating whether this instance is treated as a dictionary.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is treated as a dictionary; otherwise, <c>false</c>.
        /// </value>
        public bool IsTreatedAsDictionary => !IsAttributedAsNotCollection && MemberTypeWrapper.IsTreatedAsDictionary;

        /// <summary>
        ///     Gets or sets the type of the custom serializer.
        /// </summary>
        /// <value>The type of the custom serializer.</value>
        public Type? CustomSerializerType { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether this instance has custom serializer.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has custom serializer; otherwise, <c>false</c>.
        /// </value>
        public bool HasCustomSerializer => CustomSerializerType != null;

        public bool PreservesWhitespace { get; internal set; }

        public int Order { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether this instance has a custom namespace
        ///     defined for it through the <see cref="YAXNamespaceAttribute" /> attribute.
        /// </summary>
        public bool HasNamespace => Namespace.IsEmpty();

        /// <summary>
        ///     Gets the namespace associated with this element.
        /// </summary>
        /// <remarks>
        ///     If <see cref="HasNamespace" /> is <c>false</c> then this should
        ///     be inherited from any parent elements.
        /// </remarks>
        public XNamespace Namespace
        {
            get { return _namespace; }

            internal set
            {
                _namespace = value;
                // explicit namespace definition overrides namespace definitions in SerializeAs attributes.
                _alias = _namespace + _alias?.LocalName;
            }
        }

        /// <summary>
        ///     Gets the namespace prefix associated with this element
        /// </summary>
        /// <remarks>
        ///     If <see cref="HasNamespace" /> is <c>false</c> then this should
        ///     be inherited from any parent elements.
        ///     If this is <c>null</c>, then it should be assumed that the specified
        ///     <see cref="Namespace" /> (if it is present) is the default namespace.
        ///     It should also be noted that if a namespace is not provided for the
        ///     entire document (default namespace) and yet a default namespace is
        ///     provided for one element that an exception should be thrown (since
        ///     setting a default namespace for that element would make it apply to
        ///     the whole document).
        /// </remarks>
        public string? NamespacePrefix { get; internal set; }

        public bool IsRealTypeDefined(Type type)
        {
            return GetRealTypeDefinition(type) != null;
        }

        // Public Methods

        public YAXTypeAttribute? GetRealTypeDefinition(Type type)
        {
            return _possibleRealTypes.FirstOrDefault(x => ReferenceEquals(x.Type, type));
        }

        /// <summary>
        ///     Gets the original value of this member in the specified object
        /// </summary>
        /// <param name="obj">The object whose value corresponding to this instance, must be retreived.</param>
        /// <param name="index">The array of indices (usually <c>null</c>).</param>
        /// <returns>the original value of this member in the specified object</returns>
        public object? GetOriginalValue(object? obj, object[]? index)
        {
            if (_isProperty)
                return PropertyInfo?.GetValue(obj, index);
            return FieldInfo?.GetValue(obj);
        }

        /// <summary>
        ///     Gets the processed value of this member in the specified object
        /// </summary>
        /// <param name="obj">The object whose value corresponding to this instance, must be retreived.</param>
        /// <returns>the processed value of this member in the specified object</returns>
        public object? GetValue(object obj)
        {
            var elementValue = GetOriginalValue(obj, null);

            if (elementValue == null)
                return null;

            if (MemberTypeWrapper.IsEnum) return MemberTypeWrapper.EnumWrapper.GetAlias(elementValue);

            // trying to build the element value
            if (HasFormat && !IsTreatedAsCollection)
                // do the formatting. If formatting succeeds the type of 
                // the elementValue will become 'System.String'
                elementValue = ReflectionUtils.TryFormatObject(elementValue, Format);

            return elementValue;
        }

        /// <summary>
        ///     Sets the value of this member in the specified object
        /// </summary>
        /// <param name="obj">The object whose member corresponding to this instance, must be given value.</param>
        /// <param name="value">The value.</param>
        public void SetValue(object obj, object? value)
        {
            if (_isProperty)
                PropertyInfo?.SetValue(obj, value, null);
            else
                FieldInfo?.SetValue(obj, value);
        }

        /// <summary>
        ///     Determines whether this instance of <c>MemberWrapper</c> can be serialized.
        /// </summary>
        /// <param name="serializationFields">The serialization fields.</param>
        /// <param name="dontSerializePropertiesWithNoSetter">Skip serialization of fields which doesn't have a setter.</param>
        /// <returns>
        ///     <c>true</c> if this instance of <c>MemberWrapper</c> can be serialized; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAllowedToBeSerialized(YAXSerializationFields serializationFields,
            bool dontSerializePropertiesWithNoSetter)
        {
            if (PropertyInfo != null && dontSerializePropertiesWithNoSetter && _isProperty && !PropertyInfo.CanWrite)
                return false;

            if (serializationFields == YAXSerializationFields.AllFields)
                return !IsAttributedAsDontSerialize;
            if (serializationFields == YAXSerializationFields.AttributedFieldsOnly)
                return !IsAttributedAsDontSerialize && IsAttributedAsSerializable;
            if (serializationFields == YAXSerializationFields.PublicPropertiesOnly)
                return !IsAttributedAsDontSerialize && _isProperty &&
                       ReflectionUtils.IsPublicProperty(PropertyInfo);
            throw new Exception("Unknown serialization field option");
        }

        /// <summary>
        ///     Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            return MemberInfo.ToString();
        }

        // Private Methods 

        /// <summary>
        ///     Initializes this instance of <c>MemberWrapper</c>.
        /// </summary>
        private void InitInstance()
        {
            Comment = null;
            IsAttributedAsSerializable = false;
            IsAttributedAsDontSerialize = false;
            IsAttributedAsNotCollection = false;
            IsSerializedAsAttribute = false;
            IsSerializedAsValue = false;
            SerializationLocation = ".";
            Format = null;
            InitDefaultValue();
        }

        /// <summary>
        ///     Initializes the default value for this instance of <c>MemberWrapper</c>.
        /// </summary>
        private void InitDefaultValue()
        {
            DefaultValue = MemberType.IsValueType 
                ? Activator.CreateInstance(MemberType, Array.Empty<object>()) 
                : null;
        }

        /// <summary>
        /// Called by the following attributes implementing <see cref="IYaxMemberLevelAttribute.Setup"/>:
        /// <see cref="YAXAttributeForClassAttribute"/>, <see cref="YAXValueForClassAttribute"/>, <see cref="YAXAttributeForAttribute"/>
        /// <see cref="YAXValueForAttribute"/>.
        /// </summary>
        /// <returns><see langword="true"/>, if processing is allowed.</returns>
        /// <remarks>MemberWrapper processes YAXCustomSerializerAttribute and YAXCollectionAttribute first.</remarks>
        internal bool IsAllowedToProcess()
        {
            return ReflectionUtils.IsBasicType(MemberType) || HasCustomSerializer || MemberTypeWrapper.HasCustomSerializer ||
                   CollectionAttributeInstance is { SerializationType: YAXCollectionSerializationTypes.Serially };
        }

        /// <summary>
        /// Adds the <paramref name="yaxTypeAttribute"/> to the list of possible real types.
        /// </summary>
        /// <param name="yaxTypeAttribute"></param>
        /// <exception cref="YAXPolymorphicException"></exception>
        internal void AddAttributeToListOfRealTypes(YAXTypeAttribute yaxTypeAttribute)
        {
            var alias = yaxTypeAttribute.Alias;
            if (alias != null)
            {
                alias = alias.Trim();
                if (alias.Length == 0)
                    alias = null;
            }

            if (_possibleRealTypes.Any(x => x.Type == yaxTypeAttribute.Type))
                throw new YAXPolymorphicException(
                    $"The type \"{yaxTypeAttribute.Type.Name}\" for field/property \"{MemberInfo}\" has already been defined through another attribute.");

            if (alias != null && _possibleRealTypes.Any(x => alias.Equals(x.Alias, StringComparison.Ordinal)))
                throw new YAXPolymorphicException(
                    $"The alias \"{alias}\" given to type \"{yaxTypeAttribute.Type.Name}\" for field/property \"{MemberInfo}\" has already been given to another type through another attribute.");

            _possibleRealTypes.Add(yaxTypeAttribute);
        }

        /// <summary>
        /// Adds the <paramref name="yaxCollectionItemTypeAttr"/> to the list of collection item real types.
        /// </summary>
        /// <param name="yaxCollectionItemTypeAttr"></param>
        /// <exception cref="YAXPolymorphicException"></exception>
        internal void AddAttributeToCollectionItemRealTypes(YAXCollectionItemTypeAttribute yaxCollectionItemTypeAttr)
        {
            var alias = yaxCollectionItemTypeAttr.Alias;
            if (alias != null)
            {
                alias = alias.Trim();
                if (alias.Length == 0)
                    alias = null;
            }

            if (_possibleCollectionItemRealTypes.Any(x => x.Type == yaxCollectionItemTypeAttr.Type))
                throw new YAXPolymorphicException(string.Format(
                    "The collection-item type \"{0}\" for collection \"{1}\" has already been defined through another attribute.",
                    yaxCollectionItemTypeAttr.Type.Name, MemberInfo));

            if (alias != null &&
                _possibleCollectionItemRealTypes.Any(x => alias.Equals(x.Alias, StringComparison.Ordinal)))
                throw new YAXPolymorphicException(string.Format(
                    "The alias \"{0}\" given to collection-item type \"{1}\" for field/property \"{2}\" has already been given to another type through another attribute.",
                    alias, yaxCollectionItemTypeAttr.Type.Name, MemberInfo));

            _possibleCollectionItemRealTypes.Add(yaxCollectionItemTypeAttr);
        }
    }
}