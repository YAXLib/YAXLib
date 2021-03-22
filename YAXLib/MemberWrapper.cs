// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace YAXLib
{
    /// <summary>
    ///     A wrapper class for members which only can be properties or member variables
    /// </summary>
    internal class MemberWrapper
    {
        /// <summary>
        ///     the <c>FieldInfo</c> instance, if this member corresponds to a field, <c>null</c> otherwise
        /// </summary>
        private readonly FieldInfo _fieldInfoInstance;

        /// <summary>
        ///     <c>true</c> if this instance corresponds to a property, <c>false</c>
        ///     if it corresponds to a field (i.e., a member variable)
        /// </summary>
        private readonly bool _isProperty;

        /// <summary>
        ///     reference to the underlying <c>MemberInfo</c> from which this instance is built
        /// </summary>
        private readonly MemberInfo _memberInfo;

        /// <summary>
        ///     the member type of the underlying member
        /// </summary>
        private readonly Type _memberType;

        /// <summary>
        ///     a type wrapper around the underlying member type
        /// </summary>
        private readonly UdtWrapper _memberTypeWrapper;

        private readonly List<YAXCollectionItemTypeAttribute> _possibleCollectionItemRealTypes =
            new List<YAXCollectionItemTypeAttribute>();

        private readonly List<YAXTypeAttribute> _possibleRealTypes = new List<YAXTypeAttribute>();

        /// <summary>
        ///     the <c>PropertyInfo</c> instance, if this member corresponds to a property, <c>null</c> otherwise
        /// </summary>
        private readonly PropertyInfo _propertyInfoInstance;

        /// <summary>
        ///     The alias specified by the user
        /// </summary>
        private XName _alias;

        /// <summary>
        ///     The collection attribute instance
        /// </summary>
        private YAXCollectionAttribute _collectionAttributeInstance;

        /// <summary>
        ///     the dictionary attribute instance
        /// </summary>
        private YAXDictionaryAttribute _dictionaryAttributeInstance;

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
        public MemberWrapper(MemberInfo memberInfo, YAXSerializer callerSerializer)
        {
            Order = int.MaxValue;

            if (!(memberInfo.MemberType == MemberTypes.Property || memberInfo.MemberType == MemberTypes.Field))
                throw new Exception("Member must be either property or field");

            _memberInfo = memberInfo;
            _isProperty = memberInfo.MemberType == MemberTypes.Property;

            Alias = StringUtils.RefineSingleElement(_memberInfo.Name);

            if (_isProperty)
                _propertyInfoInstance = (PropertyInfo) memberInfo;
            else
                _fieldInfoInstance = (FieldInfo) memberInfo;

            _memberType = _isProperty ? _propertyInfoInstance.PropertyType : _fieldInfoInstance.FieldType;

            _memberTypeWrapper = TypeWrappersPool.Pool.GetTypeWrapper(MemberType, callerSerializer);
            if (_memberTypeWrapper.HasNamespace)
            {
                Namespace = _memberTypeWrapper.Namespace;
                NamespacePrefix = _memberTypeWrapper.NamespacePrefix;
            }

            InitInstance();

            TreatErrorsAs = callerSerializer != null ? callerSerializer.DefaultExceptionType : YAXExceptionTypes.Error;

            // discover YAXCustomSerializerAttributes earlier, because some other attributes depend on it
            var attrsToProcessEarlier = new HashSet<Type>
                {typeof(YAXCustomSerializerAttribute), typeof(YAXCollectionAttribute)};
            foreach (var attrType in attrsToProcessEarlier)
            {
                var customSerAttrs = Attribute.GetCustomAttributes(_memberInfo, attrType, true);
                foreach (var attr in customSerAttrs) ProcessYaxAttribute(attr);
            }

            foreach (var attr in Attribute.GetCustomAttributes(_memberInfo, true))
            {
                // no need to process, it has been processed earlier
                if (attrsToProcessEarlier.Contains(attr.GetType()))
                    continue;

                if (attr is YAXBaseAttribute)
                    ProcessYaxAttribute(attr);
            }

            // now override some values from member-type-wrapper into member-wrapper
            // if member-type has collection attributes while the member itself does not have them, 
            // then use those of the member-type
            if (_collectionAttributeInstance == null && _memberTypeWrapper.CollectionAttributeInstance != null)
                _collectionAttributeInstance = _memberTypeWrapper.CollectionAttributeInstance;
            _memberInfo.GetCustomAttributes(true);

            if (_dictionaryAttributeInstance == null && _memberTypeWrapper.DictionaryAttributeInstance != null)
                _dictionaryAttributeInstance = _memberTypeWrapper.DictionaryAttributeInstance;
        }

        /// <summary>
        ///     Gets the alias specified for this member.
        /// </summary>
        /// <value>The alias specified for this member.</value>
        public XName Alias
        {
            get { return _alias; }

            private set
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
        ///     Gets a value indicating whether the member corresponding to this instance can be read from.
        /// </summary>
        /// <value><c>true</c> if the member corresponding to this instance can be read from; otherwise, <c>false</c>.</value>
        public bool CanRead
        {
            get
            {
                if (_isProperty)
                    return _propertyInfoInstance.CanRead;
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
                    return _propertyInfoInstance.CanWrite;
                return true;
            }
        }

        /// <summary>
        ///     Gets an array of comment lines.
        /// </summary>
        /// <value>The comment lines.</value>
        public string[] Comment { get; private set; }

        /// <summary>
        ///     Gets the default value for this instance.
        /// </summary>
        /// <value>The default value for this instance.</value>
        public object DefaultValue { get; private set; }

        /// <summary>
        ///     Gets the format specified for this value; <c>null</c> if no format is specified.
        /// </summary>
        /// <value>the format specified for this value; <c>null</c> if no format is specified.</value>
        public string Format { get; private set; }

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
        public bool IsAttributedAsDontSerialize { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is attributed as not-collection.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is attributed as not-collection; otherwise, <c>false</c>.
        /// </value>
        public bool IsAttributedAsNotCollection { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is attributed as serializable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is attributed as serializable; otherwise, <c>false</c>.
        /// </value>
        public bool IsAttributedAsSerializable { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is attributed as dont serialize when null.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is attributed as dont serialize when null; otherwise, <c>false</c>.
        /// </value>
        public bool IsAttributedAsDontSerializeIfNull { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is serialized as an XML attribute.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is serialized as an XML attribute; otherwise, <c>false</c>.
        /// </value>
        public bool IsSerializedAsAttribute
        {
            get { return _isSerializedAsAttribute; }

            private set
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

            private set
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

            private set
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
        ///     Gets the type wrapper instance corresponding to the member-type of this instance.
        /// </summary>
        /// <value>The type wrapper instance corresponding to the member-type of this instance.</value>
        public UdtWrapper MemberTypeWrapper => _memberTypeWrapper;

        /// <summary>
        ///     Gets a value indicating whether the underlying type is a known-type
        /// </summary>
        public bool IsKnownType => _memberTypeWrapper.IsKnownType;

        /// <summary>
        ///     Gets the original of this member (as opposed to its alias).
        /// </summary>
        /// <value>The original of this member .</value>
        public string OriginalName => _memberInfo.Name;

        /// <summary>
        ///     Gets the serialization location.
        /// </summary>
        /// <value>The serialization location.</value>
        public string SerializationLocation
        {
            get { return _serializationLocation; }

            private set { _serializationLocation = StringUtils.RefineLocationString(value); }
        }

        /// <summary>
        ///     Gets the exception type for this instance in case of encountering missing values
        /// </summary>
        /// <value>The exception type for this instance in case of encountering missing values</value>
        public YAXExceptionTypes TreatErrorsAs { get; private set; }

        /// <summary>
        ///     Gets the collection attribute instance.
        /// </summary>
        /// <value>The collection attribute instance.</value>
        public YAXCollectionAttribute CollectionAttributeInstance => _collectionAttributeInstance;

        /// <summary>
        ///     Gets the dictionary attribute instance.
        /// </summary>
        /// <value>The dictionary attribute instance.</value>
        public YAXDictionaryAttribute DictionaryAttributeInstance => _dictionaryAttributeInstance;

        /// <summary>
        ///     Gets a value indicating whether this instance is treated as a collection.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is treated as a collection; otherwise, <c>false</c>.
        /// </value>
        public bool IsTreatedAsCollection => !IsAttributedAsNotCollection && _memberTypeWrapper.IsTreatedAsCollection;

        /// <summary>
        ///     Gets a value indicating whether this instance is treated as a dictionary.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is treated as a dictionary; otherwise, <c>false</c>.
        /// </value>
        public bool IsTreatedAsDictionary => !IsAttributedAsNotCollection && _memberTypeWrapper.IsTreatedAsDictionary;

        /// <summary>
        ///     Gets or sets the type of the custom serializer.
        /// </summary>
        /// <value>The type of the custom serializer.</value>
        public Type CustomSerializerType { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this instance has custom serializer.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has custom serializer; otherwise, <c>false</c>.
        /// </value>
        public bool HasCustomSerializer => CustomSerializerType != null;

        public bool PreservesWhitespace { get; private set; }

        public int Order { get; private set; }

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

            private set
            {
                _namespace = value;
                // explicit namespace definition overrides namespace definitions in SerializeAs attributes.
                _alias = _namespace + _alias.LocalName;
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
        public string NamespacePrefix { get; private set; }

        public bool IsRealTypeDefined(Type type)
        {
            return GetRealTypeDefinition(type) != null;
        }


        // TODO: move to public methods section
        public YAXTypeAttribute GetRealTypeDefinition(Type type)
        {
            return _possibleRealTypes.FirstOrDefault(x => ReferenceEquals(x.Type, type));
        }


        // Public Methods

        /// <summary>
        ///     Gets the original value of this member in the specified object
        /// </summary>
        /// <param name="obj">The object whose value corresponding to this instance, must be retreived.</param>
        /// <param name="index">The array of indeces (usually <c>null</c>).</param>
        /// <returns>the original value of this member in the specified object</returns>
        public object GetOriginalValue(object obj, object[] index)
        {
            if (_isProperty)
                return _propertyInfoInstance.GetValue(obj, index);
            return _fieldInfoInstance.GetValue(obj);
        }

        /// <summary>
        ///     Gets the processed value of this member in the specified object
        /// </summary>
        /// <param name="obj">The object whose value corresponding to this instance, must be retreived.</param>
        /// <returns>the processed value of this member in the specified object</returns>
        public object GetValue(object obj)
        {
            var elementValue = GetOriginalValue(obj, null);

            if (elementValue == null)
                return null;

            if (_memberTypeWrapper.IsEnum) return _memberTypeWrapper.EnumWrapper.GetAlias(elementValue);

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
        public void SetValue(object obj, object value)
        {
            if (_isProperty)
                _propertyInfoInstance.SetValue(obj, value, null);
            else
                _fieldInfoInstance.SetValue(obj, value);
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
            if (dontSerializePropertiesWithNoSetter && _isProperty && !_propertyInfoInstance.CanWrite)
                return false;

            if (serializationFields == YAXSerializationFields.AllFields)
                return !IsAttributedAsDontSerialize;
            if (serializationFields == YAXSerializationFields.AttributedFieldsOnly)
                return !IsAttributedAsDontSerialize && IsAttributedAsSerializable;
            if (serializationFields == YAXSerializationFields.PublicPropertiesOnly)
                return !IsAttributedAsDontSerialize && _isProperty &&
                       ReflectionUtils.IsPublicProperty(_propertyInfoInstance);
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
            return _memberInfo.ToString();
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
            if (MemberType.IsValueType)
                DefaultValue = Activator.CreateInstance(MemberType, new object[0]);
            //DefaultValue = MemberType.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[0]);
            else
                DefaultValue = null;
        }

        /// <summary>
        ///     Processes the specified attribute which is an instance of <c>YAXAttribute</c>.
        /// </summary>
        /// <param name="attr">The attribute to process.</param>
        private void ProcessYaxAttribute(object attr)
        {
            if (attr is YAXCommentAttribute)
            {
                var comment = (attr as YAXCommentAttribute).Comment;
                if (!string.IsNullOrEmpty(comment))
                {
                    var comments = comment.Split(new[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
                    for (var i = 0; i < comments.Length; i++) comments[i] = string.Format(" {0} ", comments[i].Trim());

                    Comment = comments;
                }
            }
            else if (attr is YAXSerializableFieldAttribute)
            {
                IsAttributedAsSerializable = true;
            }
            else if (attr is YAXAttributeForClassAttribute)
            {
                // it is required that YAXCustomSerializerAttribute is processed earlier
                if (ReflectionUtils.IsBasicType(MemberType) || CustomSerializerType != null ||
                    _collectionAttributeInstance != null && _collectionAttributeInstance.SerializationType ==
                    YAXCollectionSerializationTypes.Serially)
                {
                    IsSerializedAsAttribute = true;
                    SerializationLocation = ".";
                }
            }
            else if (attr is YAXValueForClassAttribute)
            {
                // it is required that YAXCustomSerializerAttribute is processed earlier
                if (ReflectionUtils.IsBasicType(MemberType) || CustomSerializerType != null ||
                    _collectionAttributeInstance != null && _collectionAttributeInstance.SerializationType ==
                    YAXCollectionSerializationTypes.Serially)
                {
                    IsSerializedAsValue = true;
                    SerializationLocation = ".";
                }
            }
            else if (attr is YAXAttributeForAttribute)
            {
                // it is required that YAXCustomSerializerAttribute is processed earlier
                if (ReflectionUtils.IsBasicType(MemberType) || CustomSerializerType != null ||
                    _collectionAttributeInstance != null && _collectionAttributeInstance.SerializationType ==
                    YAXCollectionSerializationTypes.Serially)
                {
                    IsSerializedAsAttribute = true;
                    string path, alias;
                    StringUtils.ExttractPathAndAliasFromLocationString((attr as YAXAttributeForAttribute).Parent,
                        out path, out alias);

                    SerializationLocation = path;
                    if (!string.IsNullOrEmpty(alias))
                        Alias = StringUtils.RefineSingleElement(alias);
                }
            }
            else if (attr is YAXElementForAttribute)
            {
                IsSerializedAsElement = true;

                string path, alias;
                StringUtils.ExttractPathAndAliasFromLocationString((attr as YAXElementForAttribute).Parent, out path,
                    out alias);

                SerializationLocation = path;
                if (!string.IsNullOrEmpty(alias))
                    Alias = StringUtils.RefineSingleElement(alias);
            }
            else if (attr is YAXValueForAttribute)
            {
                // it is required that YAXCustomSerializerAttribute is processed earlier
                if (ReflectionUtils.IsBasicType(MemberType) || CustomSerializerType != null ||
                    _collectionAttributeInstance != null && _collectionAttributeInstance.SerializationType ==
                    YAXCollectionSerializationTypes.Serially)
                {
                    IsSerializedAsValue = true;

                    string path, alias;
                    StringUtils.ExttractPathAndAliasFromLocationString((attr as YAXValueForAttribute).Parent, out path,
                        out alias);

                    SerializationLocation = path;
                    if (!string.IsNullOrEmpty(alias))
                        Alias = StringUtils.RefineSingleElement(alias);
                }
            }
            else if (attr is YAXDontSerializeAttribute)
            {
                IsAttributedAsDontSerialize = true;
            }
            else if (attr is YAXSerializeAsAttribute)
            {
                Alias = StringUtils.RefineSingleElement((attr as YAXSerializeAsAttribute).SerializeAs);
            }
            else if (attr is YAXCollectionAttribute)
            {
                _collectionAttributeInstance = attr as YAXCollectionAttribute;
            }
            else if (attr is YAXDictionaryAttribute)
            {
                _dictionaryAttributeInstance = attr as YAXDictionaryAttribute;
            }
            else if (attr is YAXErrorIfMissedAttribute)
            {
                var temp = attr as YAXErrorIfMissedAttribute;
                DefaultValue = temp.DefaultValue;
                TreatErrorsAs = temp.TreatAs;
            }
            else if (attr is YAXFormatAttribute)
            {
                Format = (attr as YAXFormatAttribute).Format;
            }
            else if (attr is YAXNotCollectionAttribute)
            {
                // arrays are always treated as collections
                if (!ReflectionUtils.IsArray(MemberType))
                    IsAttributedAsNotCollection = true;
            }
            else if (attr is YAXCustomSerializerAttribute)
            {
                var serType = (attr as YAXCustomSerializerAttribute).CustomSerializerType;

                Type genTypeArg;
                var isDesiredInterface =
                    ReflectionUtils.IsDerivedFromGenericInterfaceType(serType, typeof(ICustomSerializer<>),
                        out genTypeArg);

                if (!isDesiredInterface)
                    throw new YAXException(
                        "The provided custom serialization type is not derived from the proper interface");
                if (genTypeArg != MemberType)
                    throw new YAXException("The generic argument of the class and the member type do not match");
                CustomSerializerType = serType;
            }
            else if (attr is YAXPreserveWhitespaceAttribute)
            {
                PreservesWhitespace = true;
            }
            else if (attr is YAXSerializableTypeAttribute)
            {
                // this should not happen
                throw new Exception("This attribute is not applicable to fields and properties!");
            }
            else if (attr is YAXNamespaceAttribute)
            {
                var nsAttrib = attr as YAXNamespaceAttribute;
                Namespace = nsAttrib.Namespace;
                NamespacePrefix = nsAttrib.Prefix;
            }
            else if (attr is YAXTypeAttribute)
            {
                var yaxTypeAttr = attr as YAXTypeAttribute;
                var alias = yaxTypeAttr.Alias;
                if (alias != null)
                {
                    alias = alias.Trim();
                    if (alias.Length == 0)
                        alias = null;
                }

                if (_possibleRealTypes.Any(x => x.Type == yaxTypeAttr.Type))
                    throw new YAXPolymorphicException(string.Format(
                        "The type \"{0}\" for field/property \"{1}\" has already been defined through another attribute.",
                        yaxTypeAttr.Type.Name, _memberInfo));

                if (alias != null && _possibleRealTypes.Any(x => alias.Equals(x.Alias, StringComparison.Ordinal)))
                    throw new YAXPolymorphicException(string.Format(
                        "The alias \"{0}\" given to type \"{1}\" for field/property \"{2}\" has already been given to another type through another attribute.",
                        alias, yaxTypeAttr.Type.Name, _memberInfo));

                _possibleRealTypes.Add(yaxTypeAttr);
            }
            else if (attr is YAXCollectionItemTypeAttribute)
            {
                var yaxColletionItemTypeAttr = attr as YAXCollectionItemTypeAttribute;
                var alias = yaxColletionItemTypeAttr.Alias;
                if (alias != null)
                {
                    alias = alias.Trim();
                    if (alias.Length == 0)
                        alias = null;
                }

                if (_possibleCollectionItemRealTypes.Any(x => x.Type == yaxColletionItemTypeAttr.Type))
                    throw new YAXPolymorphicException(string.Format(
                        "The collection-item type \"{0}\" for collection \"{1}\" has already been defined through another attribute.",
                        yaxColletionItemTypeAttr.Type.Name, _memberInfo));

                if (alias != null &&
                    _possibleCollectionItemRealTypes.Any(x => alias.Equals(x.Alias, StringComparison.Ordinal)))
                    throw new YAXPolymorphicException(string.Format(
                        "The alias \"{0}\" given to collection-item type \"{1}\" for field/property \"{2}\" has already been given to another type through another attribute.",
                        alias, yaxColletionItemTypeAttr.Type.Name, _memberInfo));

                _possibleCollectionItemRealTypes.Add(yaxColletionItemTypeAttr);
            }
            else if (attr is YAXDontSerializeIfNullAttribute)
            {
                IsAttributedAsDontSerializeIfNull = true;
            }
            else if (attr is YAXElementOrder)
            {
                Order = (attr as YAXElementOrder).Order;
            }
            else
            {
                throw new Exception("Added new attribute type to the library but not yet processed!");
            }
        }
    }
}