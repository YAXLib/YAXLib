// Copyright 2009 - 2010 Sina Iravanian - <sina@sinairv.com>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace YAXLib
{
    /// <summary>
    /// An XML serialization class which lets developers design the XML file structure and select the exception handling policy.
    /// This class also supports serializing most of the collection classes such as the Dictionary generic class.
    /// </summary>
    public class YAXSerializer
    {
        #region Fields

        /// <summary>
        /// The class or structure that is to be serialized/deserialized.
        /// </summary>
        private Type m_type;

        /// <summary>
        /// The handling policy enumeration to be used by the YAX library.
        /// </summary>
        private YAXExceptionHandlingPolicies m_exceptionPolicy = YAXExceptionHandlingPolicies.ThrowErrorsOnly;

        /// <summary>
        /// The list of all errors that have occured.
        /// </summary>
        private YAXParsingErrors m_parsingErrors = new YAXParsingErrors();

        /// <summary>
        /// The exception error behaviour enumeration to be used by the YAX library.
        /// </summary>
        private YAXExceptionTypes m_defaultExceptionType = YAXExceptionTypes.Warning;

        /// <summary>
        /// Specifies whether an exception is occurred during the deserialization of the current member
        /// </summary>
        private bool m_exceptionOccurredDuringMemberDeserialization = false;

        /// <summary>
        /// The serialization option enumeration which can be set during initialization.
        /// </summary>
        private YAXSerializationOptions m_serializationOption = YAXSerializationOptions.SerializeNullObjects;

        /// <summary>
        /// The type wrapper for the underlying type used in the serializer
        /// </summary>
        private UdtWrapper m_udtWrapper = null;

        /// <summary>
        /// XML document object which will hold the resulting serialization
        /// </summary>
        private XDocument m_mainDocument = null;

        /// <summary>
        /// a reference to the base xml element used during serialization.
        /// </summary>
        private XElement m_baseElement = null;

        /// <summary>
        /// reference to a pre assigned deserialization base object
        /// </summary>
        private object m_desObject = null;

        /// <summary>
        /// if <c>true</c> an xmlns:yaxlib attribute will be added to the top level serialized element.
        /// </summary>
        private bool m_needsNamespaceAddition = false;

        /// <summary>
        /// The URI address which holds the xmlns:yaxlib definition.
        /// </summary>
        private static readonly XNamespace s_namespaceURI = XNamespace.Get("http://www.sinairv.com/yaxlib/");

        /// <summary>
        /// The initials used for the xml namespace
        /// </summary>
        private const string s_namespaceInits = "yaxlib";

        /// <summary>
        /// the attribute name used to deserialize meta-data for multi-dimensional arrays.
        /// </summary>
        private const string s_dimsAttrName = "dims";

        /// <summary>
        /// the attribute name used to deserialize meta-data for real types of objects serialized through
        /// a reference to their base class or interface.
        /// </summary>
        private const string s_trueTypeAttrName = "realtype";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="YAXSerializer"/> class.
        /// </summary>
        /// <param name="type">The type of the object being serialized/deserialized.</param>
        public YAXSerializer(Type type)
            : this(type, YAXExceptionHandlingPolicies.ThrowWarningsAndErrors, YAXExceptionTypes.Error, YAXSerializationOptions.SerializeNullObjects)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YAXSerializer"/> class.
        /// </summary>
        /// <param name="type">The type of the object being serialized/deserialized.</param>
        /// <param name="exceptionPolicy">The exception handling policy.</param>
        public YAXSerializer(Type type, YAXExceptionHandlingPolicies exceptionPolicy)
            : this(type, exceptionPolicy, YAXExceptionTypes.Error, YAXSerializationOptions.SerializeNullObjects)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YAXSerializer"/> class.
        /// </summary>
        /// <param name="type">The type of the object being serialized/deserialized.</param>
        /// <param name="exceptionPolicy">The exception handling policy.</param>
        /// <param name="defaultExType">The exceptions are treated as the value specified, unless otherwise specified.</param>
        public YAXSerializer(Type type, YAXExceptionHandlingPolicies exceptionPolicy, YAXExceptionTypes defaultExType)
            : this(type, exceptionPolicy, defaultExType, YAXSerializationOptions.SerializeNullObjects)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="YAXSerializer"/> class.
        /// </summary>
        /// <param name="t">The type of the object being serialized/deserialized.</param>
        /// <param name="exceptionPolicy">The exception handling policy.</param>
        /// <param name="defaultExType">The exceptions are treated as the value specified, unless otherwise specified.</param>
        /// <param name="option">The serialization option.</param>
        public YAXSerializer(Type t, YAXExceptionHandlingPolicies exceptionPolicy, YAXExceptionTypes defaultExType, YAXSerializationOptions option)
        {
            this.m_type = t;
            this.m_exceptionPolicy = exceptionPolicy;
            this.m_defaultExceptionType = defaultExType;
            this.m_serializationOption = option;
            // this must be the last call
            this.m_udtWrapper = TypeWrappersPool.Pool.GetTypeWrapper(m_type, this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the default type of the exception.
        /// </summary>
        /// <value>The default type of the exception.</value>
        public YAXExceptionTypes DefaultExceptionType
        {
            get
            {
                return this.m_defaultExceptionType;
            }
        }

        /// <summary>
        /// Gets the serialization option.
        /// </summary>
        /// <value>The serialization option.</value>
        public YAXSerializationOptions SerializationOption
        {
            get
            {
                return this.m_serializationOption;
            }
        }

        /// <summary>
        /// Gets the exception handling policy.
        /// </summary>
        /// <value>The exception handling policy.</value>
        public YAXExceptionHandlingPolicies ExceptionHandlingPolicy
        {
            get
            {
                return this.m_exceptionPolicy;
            }
        }

        /// <summary>
        /// Gets the parsing errors.
        /// </summary>
        /// <value>The parsing errors.</value>
        public YAXParsingErrors ParsingErrors
        {
            get
            {
                return this.m_parsingErrors;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is craeted to deserialize a non collection member of another object.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is craeted to deserialize a non collection member of another object; otherwise, <c>false</c>.
        /// </value>
        private bool IsCraetedToDeserializeANonCollectionMember { get; set; }

        #endregion

        #region Public Methods

        #region Serialization

        /// <summary>
        /// Serializes the specified object and returns a string containing the XML.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>A <code>System.String</code> containing the XML</returns>
        public string Serialize(object obj)
        {
            return SerializeXDocument(obj).ToString();
        }

        /// <summary>
        /// Serializes the specified object into a <c>TextWriter</c> instance.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="textWriter">The <c>TextWriter</c> instance.</param>
        public void Serialize(object obj, TextWriter textWriter)
        {
            textWriter.Write(this.Serialize(obj));
        }

        /// <summary>
        /// Serializes the specified object into a <c>XmlWriter</c> instance.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="xmlWriter">The <c>XmlWriter</c> instance.</param>
        public void Serialize(object obj, XmlWriter xmlWriter)
        {
            this.SerializeXDocument(obj).WriteTo(xmlWriter);
        }

        /// <summary>
        /// Serializes the specified object to file.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="fileName">Path to the file.</param>
        public void SerializeToFile(object obj, string fileName)
        {
            string ser = String.Format(
                CultureInfo.CurrentCulture,
                "{0}{1}{2}",
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>",
                Environment.NewLine,
                this.Serialize(obj));
            File.WriteAllText(fileName, ser, Encoding.UTF8);
        }

        #endregion

        #region Deserialization

        /// <summary>
        /// Deserializes the specified string containing the XML serialization and returns an object.
        /// </summary>
        /// <param name="input">The input string containing the XML serialization.</param>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(string input)
        {
            try
            {
                using (TextReader tr = new StringReader(input))
                {
                    XDocument xdoc = XDocument.Load(tr);
                    XElement baseElement = xdoc.Root;
                    return this.DeserializeBase(baseElement);
                }
            }
            catch (XmlException ex)
            {
                this.OnExceptionOccurred(new YAXBadlyFormedXML(ex), this.m_defaultExceptionType);
                return null;
            }
        }

        /// <summary>
        /// Deserializes an object while reading input from an instance of <c>XmlReader</c>.
        /// </summary>
        /// <param name="xmlReader">The <c>XmlReader</c> instance to read input from.</param>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(XmlReader xmlReader)
        {
            try
            {
                XDocument xdoc = XDocument.Load(xmlReader);
                XElement baseElement = xdoc.Root;
                return this.DeserializeBase(baseElement);
            }
            catch (XmlException ex)
            {
                this.OnExceptionOccurred(new YAXBadlyFormedXML(ex), this.m_defaultExceptionType);
                return null;
            }
        }

        /// <summary>
        /// Deserializes an object while reading input from an instance of <c>TextReader</c>.
        /// </summary>
        /// <param name="textReader">The <c>TextReader</c> instance to read input from.</param>
        /// <returns>The deserialized object.</returns>
        public object Deserialize(TextReader textReader)
        {
            try
            {
                XDocument xdoc = XDocument.Load(textReader);
                XElement baseElement = xdoc.Root;
                return this.DeserializeBase(baseElement);
            }
            catch (XmlException ex)
            {
                this.OnExceptionOccurred(new YAXBadlyFormedXML(ex), this.m_defaultExceptionType);
                return null;
            }
        }

        /// <summary>
        /// Deserializes an object from the specified file which contains the XML serialization of the object.
        /// </summary>
        /// <param name="fileName">Path to the file.</param>
        /// <returns>The deserialized object.</returns>
        public object DeserializeFromFile(string fileName)
        {
            try
            {
                return this.Deserialize(File.ReadAllText(fileName));
            }
            catch (XmlException ex)
            {
                this.OnExceptionOccurred(new YAXBadlyFormedXML(ex), this.m_defaultExceptionType);
                return null;
            }
        }

        /// <summary>
        /// Sets the object used as the base object in the next stage of deserialization.
        /// This method enables multi-stage deserialization for YAXLib.
        /// </summary>
        /// <param name="obj">The object used as the base object in the next stage of deserialization.</param>
        public void SetDeserializationBaseObject(object obj)
        {
            if (obj != null && !this.m_type.IsInstanceOfType(obj))
            {
                throw new YAXObjectTypeMismatch(this.m_type, obj.GetType());
            }

            m_desObject = obj;
        }

        #endregion

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Cleans up auxiliary memory used by YAXLib during different sessions of serialization.
        /// </summary>
        public static void CleanUpAuxiliaryMemory()
        {
            TypeWrappersPool.CleanUp();
        }

        #endregion

        #region Private Methods
        // Private methods

        #region Serialization

        /// <summary>
        /// Serializes the object into an <c>XDocument</c> object.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns></returns>
        private XDocument SerializeXDocument(object obj)
        {
            m_mainDocument = new XDocument();
            m_mainDocument.Add(this.SerializeBase(obj));
            return m_mainDocument;
        }

        /// <summary>
        /// One of the base methods that perform the whole job of serialization.
        /// </summary>
        /// <param name="obj">The object to be serialized</param>
        /// <returns>an instance of <c>XElement</c> which contains the result of 
        /// serialization of the specified object</returns>
        private XElement SerializeBase(object obj)
        {
            if (!this.m_type.IsInstanceOfType(obj))
            {
                throw new YAXObjectTypeMismatch(this.m_type, obj.GetType());
            }

            // to serialize stand-alone collection or dictionary objects
            if (m_udtWrapper.IsTreatedAsCollection || m_udtWrapper.IsTreatedAsDictionary)
            {
                if (m_udtWrapper.IsTreatedAsDictionary)
                {
                    return this.MakeDictionaryElement(null, m_udtWrapper.Alias, obj, null, null);
                }
                else if (m_udtWrapper.IsTreatedAsCollection)
                {
                    return this.MakeCollectionElement(null, m_udtWrapper.Alias, obj, null, null);
                }
                else
                {
                    throw new Exception("This should not happen!");
                }
            }
            else
            {
                return this.SerializeBase(obj, m_udtWrapper.Alias);
            }
        }

        /// <summary>
        /// Sets the base XML element. This method is used when an <c>XMLSerializer</c>
        /// instantiates another <c>XMLSerializer</c> to serialize nested objects.
        /// Through this method the child objects have access to the already serialized elements of 
        /// their parent.
        /// </summary>
        /// <param name="baseElement">The base XML element.</param>
        private void SetBaseElement(XElement baseElement)
        {
            m_baseElement = baseElement;
        }

        /// <summary>
        /// The base method that performs the whole job of serialization. 
        /// Other serialization methods call this method to have their job done.
        /// </summary>
        /// <param name="obj">The object to be serialized</param>
        /// <param name="className">Name of the element that contains the serialized object.</param>
        /// <returns>an instance of <c>XElement</c> which contains the result of 
        /// serialization of the specified object</returns>
        private XElement SerializeBase(object obj, string className)
        {
            if (m_baseElement == null)
                m_baseElement = new XElement(className, null);
            else
            {
                XElement baseElem = new XElement(className, null);
                m_baseElement.Add(baseElem);
                m_baseElement = baseElem;
            }

            if (m_udtWrapper.HasComment && m_baseElement.Parent == null && m_mainDocument != null)
            {
                foreach (string comment in m_udtWrapper.Comment)
                    m_mainDocument.Add(new XComment(comment));
            }

            // iterate through public properties
            foreach (var member in GetFieldsToBeSerialized())
            {
                object elementValue = null;

                if (!member.CanRead)
                    continue;

                if (member.MemberType == this.m_type)
                {
                    throw new YAXCannotSerializeSelfReferentialTypes(this.m_type);
                }

                // ignore this member if it is attributed as dont serialize
                if (member.IsAttributedAsDontSerialize)
                    continue;

                elementValue = member.GetValue(obj);

                // ignore this member if it is null and we are not about to serialize null objects
                if (elementValue == null &&
                    m_udtWrapper.IsNotAllowdNullObjectSerialization)
                {
                    continue;
                }

                bool areOfSameType = true; // are element value and the member declared type the same?
                object originalValue = member.GetOriginalValue(obj, null);
                if (elementValue != null && member.MemberType != originalValue.GetType())
                {
                    areOfSameType = false;
                }

                // it gets true only for basic data types
                if (member.IsSerializedAsAttribute && areOfSameType)
                {
                    if (!XMLUtils.AttributeExists(m_baseElement, member.SerializationLocation, member.Alias))
                    {
                        if (XMLUtils.CreateAttribute(m_baseElement, member.SerializationLocation, member.Alias, elementValue) == null)
                            throw new YAXBadLocationException(member.SerializationLocation);
                    }
                    else
                    {
                        throw new YAXAttributeAlreadyExistsException(member.Alias);
                    }
                }
                else // if the data is going to be serialized as an element
                {
                    // find the parent element from its location
                    XElement parElem = XMLUtils.FindLocation(m_baseElement, member.SerializationLocation);
                    if (parElem == null) // if the parent element does not exist
                    {
                        // see if the location can be created
                        if (!XMLUtils.CanCreateLocation(m_baseElement, member.SerializationLocation))
                            throw new YAXBadLocationException(member.SerializationLocation);
                        // try to create the location
                        parElem = XMLUtils.CreateLocation(m_baseElement, member.SerializationLocation);
                        if (parElem == null)
                            throw new YAXBadLocationException(member.SerializationLocation);
                    }

                    // if control is moved here, it means that the parent 
                    // element has been found/created successfully

                    if (member.HasComment)
                    {
                        foreach (string comment in member.Comment)
                            parElem.Add(new XComment(comment));
                    }

                    // make an element with the provided data
                    bool moveDescOnly = false;
                    bool alreadyAdded = false;
                    XElement elemToAdd = MakeElement(parElem, member, elementValue, out moveDescOnly, out alreadyAdded);
                    if (!areOfSameType)
                    {
                        elemToAdd.Add(new XAttribute(s_namespaceURI + s_trueTypeAttrName, elementValue.GetType().FullName));
                        this.m_needsNamespaceAddition = true;
                    }

                    if (!alreadyAdded)
                    {
                        if (moveDescOnly) // if only the descendants of the resulting element are going to be added ...
                        {
                            XMLUtils.MoveDescendants(elemToAdd, parElem);
                        }
                        else
                        {
                            // see if such element already exists
                            XElement existingElem = parElem.Element(member.Alias);
                            if (existingElem == null)
                            {
                                // if not add the new element gracefully
                                parElem.Add(elemToAdd);
                            }
                            else // if an element with our desired name already exists
                            {
                                if (ReflectionUtils.IsBasicType(member.MemberType))
                                {
                                    existingElem.SetValue(elementValue);
                                }
                                else
                                {
                                    XMLUtils.MoveDescendants(elemToAdd, existingElem);
                                }
                            }
                        }
                    }
                }
            }

            if (m_baseElement.Parent == null && m_needsNamespaceAddition)
            {
                m_baseElement.Add(new XAttribute(XNamespace.Xmlns + s_namespaceInits, s_namespaceURI));
            }

            if (m_baseElement.Parent != null && XMLUtils.IsElementCompletelyEmpty(m_baseElement))
                m_baseElement.Remove();

            return m_baseElement;
        }

        /// <summary>
        /// Makes the element corresponding to the member specified.
        /// </summary>
        /// <param name="insertionLocation">The insertion location.</param>
        /// <param name="member">The member to serialize.</param>
        /// <param name="elementValue">The element value.</param>
        /// <param name="moveDescOnly">if set to <c>true</c> specifies that only the descendants of the resulting element should be added to the parent.</param>
        /// <param name="alreadyAdded">if set to <c>true</c> specifies the element returned is 
        /// already added to the parent element and should not be added once more.</param>
        /// <returns></returns>
        private XElement MakeElement(XElement insertionLocation, MemberWrapper member, object elementValue, out bool moveDescOnly, out bool alreadyAdded)
        {
            alreadyAdded = false;
            moveDescOnly = false;

            XElement elemToAdd;
            if (member.IsTreatedAsDictionary)
            {
                elemToAdd = this.MakeDictionaryElement(insertionLocation, member.Alias, elementValue, member.DictionaryAttributeInstance, member.CollectionAttributeInstance);
                if (member.CollectionAttributeInstance != null &&
                    member.CollectionAttributeInstance.SerializationType == YAXCollectionSerializationTypes.RecursiveWithNoContainingElement &&
                    !elemToAdd.HasAttributes)
                    moveDescOnly = true;
            }
            else if (member.IsTreatedAsCollection)
            {
                elemToAdd = this.MakeCollectionElement(insertionLocation, member.Alias, elementValue, member.CollectionAttributeInstance, member.Format);

                if (member.CollectionAttributeInstance != null &&
                    member.CollectionAttributeInstance.SerializationType == YAXCollectionSerializationTypes.RecursiveWithNoContainingElement &&
                    !elemToAdd.HasAttributes)
                    moveDescOnly = true;
            }
            else
            {
                elemToAdd = this.MakeBaseElement(insertionLocation, member.Alias, elementValue, out alreadyAdded);
            }

            return elemToAdd;
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
        /// <returns>
        /// an instance of <c>XElement</c> which contains the dictionary object
        /// serialized properly
        /// </returns>
        private XElement MakeDictionaryElement(
            XElement insertionLocation,
            string elementName,
            object elementValue,
            YAXDictionaryAttribute dicAttrInst,
            YAXCollectionAttribute collectionAttrInst)
        {
            if (elementValue == null)
            {
                return new XElement(elementName, elementValue);
            }

            Type keyType, valueType;
            if (!ReflectionUtils.IsIDictionary(elementValue.GetType(), out keyType, out valueType))
            {
                throw new ArgumentException("elementValue must be a Dictionary");
            }

            IEnumerable dicInst = elementValue as IEnumerable;
            string eachElementName;
            bool isKeyAttrib = false;
            bool isValueAttrib = false;
            string keyFormat = null;
            string valueFormat = null;
            string keyAlias = "Key";
            string valueAlias = "Value";

            if (collectionAttrInst != null)
            {
                eachElementName = collectionAttrInst.EachElementName;
            }
            else
            {
                eachElementName = null;
            }

            if (dicAttrInst != null)
            {
                eachElementName = dicAttrInst.EachPairName ?? eachElementName;

                if (dicAttrInst.SerializeKeyAs == YAXNodeTypes.Attribute)
                {
                    isKeyAttrib = ReflectionUtils.IsBasicType(keyType);
                }

                if (dicAttrInst.SerializeValueAs == YAXNodeTypes.Attribute)
                {
                    isValueAttrib = ReflectionUtils.IsBasicType(valueType);
                }

                keyFormat = dicAttrInst.KeyFormatString;
                valueFormat = dicAttrInst.ValueFormatString;

                keyAlias = dicAttrInst.KeyName ?? "Key";
                valueAlias = dicAttrInst.ValueName ?? "Value";
            }

            XElement elem = new XElement(elementName, null);

            foreach (object obj in dicInst)
            {
                object keyObj = obj.GetType().GetProperty("Key").GetValue(obj, null);
                object valueObj = obj.GetType().GetProperty("Value").GetValue(obj, null);

                bool areKeyOfSameType = true;
                bool areValueOfSameType = true;

                if (keyObj != null && keyObj.GetType() != keyType)
                    areKeyOfSameType = false;

                if (valueObj != null && valueObj.GetType() != valueType)
                    areValueOfSameType = false;

                if (keyFormat != null)
                {
                    keyObj = ReflectionUtils.TryFormatObject(keyObj, keyFormat);
                }

                if (valueFormat != null)
                {
                    valueObj = ReflectionUtils.TryFormatObject(valueObj, valueFormat);
                }

                XElement elemChild = new XElement(eachElementName ?? ReflectionUtils.GetTypeFriendlyName(obj.GetType()), null);

                if (isKeyAttrib && areKeyOfSameType)
                {
                    elemChild.Add(new XAttribute(keyAlias, keyObj ?? string.Empty));
                }
                else
                {
                    XElement addedElem = AddObjectToElement(elemChild, keyAlias, keyObj);
                    if (!areKeyOfSameType)
                    {
                        addedElem.Add(new XAttribute(s_namespaceURI + s_trueTypeAttrName, keyObj.GetType().FullName));
                        m_needsNamespaceAddition = true;
                    }
                }

                if (isValueAttrib && areValueOfSameType)
                {
                    elemChild.Add(new XAttribute(valueAlias, valueObj ?? string.Empty));
                }
                else
                {
                    XElement addedElem = AddObjectToElement(elemChild, valueAlias, valueObj);
                    if (!areValueOfSameType)
                    {
                        addedElem.Add(new XAttribute(s_namespaceURI + s_trueTypeAttrName, valueObj.GetType().FullName));
                        m_needsNamespaceAddition = true;
                    }
                }

                elem.Add(elemChild);
            }

            return elem;
        }

        /// <summary>
        /// Adds an element contatining data related to the specified object, to an existing xml element.
        /// </summary>
        /// <param name="elem">The parent element.</param>
        /// <param name="alias">The name for the element to be added.</param>
        /// <param name="obj">The object corresponding to which an element is going to be added to
        /// an existing parent element.</param>
        /// <returns>the enclosing XML element.</returns>
        private XElement AddObjectToElement(XElement elem, string alias, object obj)
        {
            UdtWrapper udt = TypeWrappersPool.Pool.GetTypeWrapper(obj.GetType(), this);
            XElement elemToAdd = null;

            if (udt.IsTreatedAsDictionary)
            {
                elemToAdd = this.MakeDictionaryElement(elem, alias, obj, null, null);
                elem.Add(elemToAdd);
            }
            else if (udt.IsTreatedAsCollection)
            {
                elemToAdd = this.MakeCollectionElement(elem, alias, obj, null, null);
                elem.Add(elemToAdd);
            }
            else if (udt.IsEnum)
            {
                bool alreadyAdded = false;
                elemToAdd = this.MakeBaseElement(elem, alias, udt.EnumWrapper.GetAlias(obj), out alreadyAdded);
                if (!alreadyAdded)
                    elem.Add(elemToAdd);
            }
            else
            {
                bool alreadyAdded = false;
                elemToAdd = this.MakeBaseElement(elem, alias, obj, out alreadyAdded);
                if (!alreadyAdded)
                    elem.Add(elemToAdd);
            }

            return elemToAdd;
        }

        /// <summary>
        /// Serializes a collection object.
        /// </summary>
        /// <param name="insertionLocation">The insertion location.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="elementValue">The object to be serailized.</param>
        /// <param name="collectionAttrInst">The collection attribute instance.</param>
        /// <param name="format">formatting string, which is going to be applied to all members of the collection.</param>
        /// <returns>
        /// an instance of <c>XElement</c> which will contain the serailized collection
        /// </returns>
        private XElement MakeCollectionElement(
            XElement insertionLocation,
            string elementName,
            object elementValue,
            YAXCollectionAttribute collectionAttrInst,
            string format)
        {
            if (elementValue == null)
            {
                return new XElement(elementName, elementValue);
            }

            if (!(elementValue is IEnumerable))
            {
                throw new ArgumentException("elementValue must be an IEnumerable");
            }

            IEnumerable collectionInst = elementValue as IEnumerable;
            YAXCollectionSerializationTypes serType = YAXCollectionSerializationTypes.Recursive;
            string seperator = string.Empty;
            string eachElementName = null;

            if (collectionAttrInst != null)
            {
                serType = collectionAttrInst.SerializationType;
                seperator = collectionAttrInst.SeparateBy;
                eachElementName = collectionAttrInst.EachElementName;
            }

            Type colItemType = ReflectionUtils.GetCollectionItemType(elementValue.GetType());
            if (eachElementName == null)
            {
                eachElementName = ReflectionUtils.GetTypeFriendlyName(colItemType);
            }

            if (serType == YAXCollectionSerializationTypes.Serially && !ReflectionUtils.IsBasicType(colItemType))
                serType = YAXCollectionSerializationTypes.Recursive;

            UdtWrapper colItemsUdt = TypeWrappersPool.Pool.GetTypeWrapper(colItemType, this);

            XElement elemToAdd = null; // will hold the resulting element
            if (serType == YAXCollectionSerializationTypes.Serially)
            {
                StringBuilder sb = new StringBuilder();

                bool isFirst = true;
                object objToAdd = null;
                foreach (object obj in collectionInst)
                {
                    if (colItemsUdt.IsEnum)
                        objToAdd = colItemsUdt.EnumWrapper.GetAlias(obj);
                    else if (format != null)
                        objToAdd = ReflectionUtils.TryFormatObject(obj, format);
                    else
                        objToAdd = obj;

                    if (isFirst)
                    {
                        sb.Append(objToAdd.ToString());
                        isFirst = false;
                    }
                    else
                    {
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", seperator, objToAdd.ToString());
                    }
                }

                bool alreadyAdded = false;
                elemToAdd = MakeBaseElement(insertionLocation, elementName, sb.ToString(), out alreadyAdded);
                if (alreadyAdded)
                    elemToAdd = null;
            }
            else
            {
                XElement elem = new XElement(elementName, null);
                object objToAdd = null;

                foreach (object obj in collectionInst)
                {
                    objToAdd = (format == null) ? obj : ReflectionUtils.TryFormatObject(obj, format);
                    XElement itemElem = this.AddObjectToElement(elem, eachElementName, objToAdd);
                    if (obj.GetType() != colItemType)
                    {
                        itemElem.Add(new XAttribute(s_namespaceURI + s_trueTypeAttrName, obj.GetType().FullName));
                        if (itemElem.Parent == null) // i.e. it has been removed, e.g. because all its members have been serialized outside the element
                            elem.Add(itemElem); // return it back, or undelete this item

                        m_needsNamespaceAddition = true;
                    }
                }

                elemToAdd = elem;
            }

            int[] arrayDims = ReflectionUtils.GetArrayDimensions(elementValue);
            if (arrayDims != null && arrayDims.Length > 1)
            {
                elemToAdd.Add(new XAttribute(s_namespaceURI + s_dimsAttrName, StringUtils.GetArrayDimsString(arrayDims)));
                this.m_needsNamespaceAddition = true;
            }

            return elemToAdd;
        }

        /// <summary>
        /// Makes an XML element with the specified name, corresponding to the object specified.
        /// </summary>
        /// <param name="insertionLocation">The insertion location.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="value">The object to be serialized in an XML element.</param>
        /// <param name="alreadyAdded">if set to <c>true</c> specifies the element returned is 
        /// already added to the parent element and should not be added once more.</param>
        /// <returns>
        /// an instance of <c>XElement</c> which will contain the serialized object,
        /// or <c>null</c> if the serialized object is already added to the base element
        /// </returns>
        private XElement MakeBaseElement(XElement insertionLocation, string name, object value, out bool alreadyAdded)
        {
            alreadyAdded = false;
            if (value == null || ReflectionUtils.IsBasicType(value.GetType()))
            {
                return new XElement(name, value);
            }
            else if (ReflectionUtils.IsStringConvertibleIFormattable(value.GetType()))
            {
                object elementValue = value.GetType().InvokeMember("ToString", BindingFlags.InvokeMethod, null, value, new object[0]);
                return new XElement(name, elementValue);
            }
            else
            {
                YAXSerializer ser = new YAXSerializer(value.GetType(), this.m_exceptionPolicy, this.m_defaultExceptionType, this.m_serializationOption);
                ser.SetBaseElement(insertionLocation);
                XElement elem = ser.SerializeBase(value, name);

                if (ser.m_needsNamespaceAddition)
                    this.m_needsNamespaceAddition = true;

                this.m_parsingErrors.AddRange(ser.ParsingErrors);
                alreadyAdded = true;
                return elem;
            }
        }

        #endregion

        #region Deserialization

        /// <summary>
        /// The basic method which performs the whole job of deserialization.
        /// </summary>
        /// <param name="baseElement">The element to be deserialized.</param>
        /// <returns>object containing the deserialized data</returns>
        private object DeserializeBase(XElement baseElement)
        {
            if (baseElement == null)
            {
                return m_desObject;
            }

            XAttribute realTypeAttr = baseElement.Attribute(s_namespaceURI + s_trueTypeAttrName);
            if (realTypeAttr != null)
            {
                Type theRealType = ReflectionUtils.GetTypeByName(realTypeAttr.Value);
                if (theRealType != null)
                {
                    m_type = theRealType;
                    m_udtWrapper = TypeWrappersPool.Pool.GetTypeWrapper(m_type, this);
                }
            }

            if (this.m_type.IsGenericType && this.m_type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                return this.DeserializeKeyValuePair(baseElement);
            }

            if (IsKnownDotNetBuiltInType(this.m_type))
            {
                return DeserializeKnownDotNetBuiltInType(baseElement, this.m_type);
            }

            if ((m_udtWrapper.IsTreatedAsCollection || m_udtWrapper.IsTreatedAsDictionary) && !IsCraetedToDeserializeANonCollectionMember)
            {
                return DeserializeCollectionValue(this.m_type, baseElement, ReflectionUtils.GetTypeFriendlyName(m_type), null);
            }

            if (ReflectionUtils.IsBasicType(m_type))
            {
                return ReflectionUtils.ConvertBasicType(baseElement.Value, m_type);
            }

            object o = null;
            if (m_desObject != null)
                o = m_desObject;
            else
                o = this.m_type.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, new object[0]);

            bool foundAnyOfMembers = false;
            foreach (var member in GetFieldsToBeSerialized())
            {
                if (!member.CanWrite)
                    continue;

                if (member.IsAttributedAsDontSerialize)
                    continue;

                // reset handled exceptions status
                m_exceptionOccurredDuringMemberDeserialization = false;

                string elemValue = string.Empty;
                XElement xelemValue = null;

                // first evaluate elemValue
                bool createdFakeElement = false;
                if (member.IsSerializedAsAttribute)
                {
                    // find the parent element from its location
                    XAttribute attr = XMLUtils.FindAttribute(baseElement, member.SerializationLocation, member.Alias);
                    if (attr == null) // if the parent element does not exist
                    {
                        // loook for an element with the same name AND a yaxlib:realtype attribute
                        XElement elem = XMLUtils.FindElement(baseElement, member.SerializationLocation, member.Alias);
                        if (elem != null && elem.Attribute(s_namespaceURI + s_trueTypeAttrName) != null)
                        {
                            elemValue = elem.Value;
                            xelemValue = elem;
                        }
                        else
                        {
                            this.OnExceptionOccurred(new YAXAttributeMissingException(
                                StringUtils.CombineLocationAndElementName(member.SerializationLocation, member.Alias)),
                                (!member.MemberType.IsValueType && m_udtWrapper.IsNotAllowdNullObjectSerialization) ? YAXExceptionTypes.Ignore : member.TreatErrorsAs);
                        }
                    }
                    else
                    {
                        foundAnyOfMembers = true;
                        elemValue = attr.Value;
                    }
                }
                else
                {
                    bool canContinue = false;
                    XElement elem = XMLUtils.FindElement(baseElement, member.SerializationLocation, member.Alias);
                    if (elem == null) // such element is not found
                    {
                        if ((member.IsTreatedAsCollection || member.IsTreatedAsDictionary) && member.CollectionAttributeInstance != null &&
                            member.CollectionAttributeInstance.SerializationType == YAXCollectionSerializationTypes.RecursiveWithNoContainingElement)
                        {
                            if (AtLeastOneOfCollectionMembersExists(baseElement, member))
                            {
                                elem = baseElement;
                                canContinue = true;
                                foundAnyOfMembers = true;
                            }
                            else
                            {
                                member.SetValue(o, member.DefaultValue);
                                continue;
                            }
                        }
                        else if (!ReflectionUtils.IsBasicType(member.MemberType) && !member.IsTreatedAsCollection && !member.IsTreatedAsDictionary)
                        {
                            // try to fix this problem by creating a fake element, maybe all its children are placed somewhere else
                            XElement fakeElem = XMLUtils.CreateElement(baseElement, member.SerializationLocation, member.Alias);
                            if (fakeElem != null)
                            {
                                createdFakeElement = true;
                                if (AtLeastOneOfMembersExists(fakeElem, member.MemberType))
                                {
                                    canContinue = true;
                                    foundAnyOfMembers = true;
                                    elem = fakeElem;
                                    elemValue = elem.Value;
                                }
                            }
                        }

                        if (!canContinue)
                        {
                            this.OnExceptionOccurred(new YAXElementMissingException(
                                StringUtils.CombineLocationAndElementName(member.SerializationLocation, member.Alias)),
                                (!member.MemberType.IsValueType && m_udtWrapper.IsNotAllowdNullObjectSerialization) ? YAXExceptionTypes.Ignore : member.TreatErrorsAs);
                        }
                    }
                    else
                    {
                        foundAnyOfMembers = true;
                        elemValue = elem.Value;
                    }

                    xelemValue = elem;
                }

                // Now try to retrieve elemValue's value.
                if (m_exceptionOccurredDuringMemberDeserialization)
                {
                    if (m_desObject == null) // i.e. if it was NOT resuming deserialization, set default value, otherwise existing value for the member is kept
                    {
                        if (!member.MemberType.IsValueType && m_udtWrapper.IsNotAllowdNullObjectSerialization)
                        {
                            try
                            {
                                member.SetValue(o, null);
                            }
                            catch
                            {
                                this.OnExceptionOccurred(
                                    new YAXDefaultValueCannotBeAssigned(member.Alias, member.DefaultValue),
                                    this.m_defaultExceptionType);
                            }
                        }
                        else if (member.DefaultValue != null)
                        {
                            try
                            {
                                member.SetValue(o, member.DefaultValue);
                            }
                            catch
                            {
                                this.OnExceptionOccurred(
                                    new YAXDefaultValueCannotBeAssigned(member.Alias, member.DefaultValue),
                                    this.m_defaultExceptionType);
                            }
                        }
                        else
                        {
                            if (!member.MemberType.IsValueType)
                            {
                                member.SetValue(o, null /*the value to be assigned */);
                            }
                        }
                    }
                }
                else if (elemValue != null)
                {
                    RetreiveElementValue(o, member, elemValue, xelemValue);
                    if (createdFakeElement && !m_exceptionOccurredDuringMemberDeserialization)
                        foundAnyOfMembers = true;
                }

                if (createdFakeElement && xelemValue != null)
                {
                    // remove the fake element
                    xelemValue.Remove();
                }
            }

            // if an empty element was given and non of its members have been retreived then return null, not an instance
            if (!foundAnyOfMembers && !baseElement.HasElements && !baseElement.HasAttributes && baseElement.IsEmpty)
                return null;

            return o;
        }

        /// <summary>
        /// Checks whether at least one of the collection memebers of 
        /// the specified collection exists.
        /// </summary>
        /// <param name="elem">The XML element to check its content.</param>
        /// <param name="member">The class-member corresponding to the collection for
        /// which we intend to check existence of its members.</param>
        /// <returns></returns>
        private bool AtLeastOneOfCollectionMembersExists(XElement elem, MemberWrapper member)
        {
            if (!((member.IsTreatedAsCollection || member.IsTreatedAsDictionary) && member.CollectionAttributeInstance != null &&
                member.CollectionAttributeInstance.SerializationType == YAXCollectionSerializationTypes.RecursiveWithNoContainingElement))
                throw new ArgumentException("member should be a collection serialized without containing element");

            string eachElementName = null;

            if (member.CollectionAttributeInstance != null)
            {
                eachElementName = member.CollectionAttributeInstance.EachElementName;
            }

            if (member.DictionaryAttributeInstance != null && member.DictionaryAttributeInstance.EachPairName != null)
            {
                eachElementName = member.DictionaryAttributeInstance.EachPairName;
            }

            if (eachElementName == null)
            {
                Type colItemType = ReflectionUtils.GetCollectionItemType(member.MemberType);
                eachElementName = ReflectionUtils.GetTypeFriendlyName(colItemType);
            }

            if (elem.Element(eachElementName) != null) // if such an element exists
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks whether at least one of the memebers (property or field) of 
        /// the specified object exists.
        /// </summary>
        /// <param name="elem">The XML element to check its content.</param>
        /// <param name="type">The class-member corresponding to the object for
        /// which we intend to check existence of its members.</param>
        /// <returns></returns>
        private bool AtLeastOneOfMembersExists(XElement elem, Type type)
        {
            if (elem == null)
                throw new ArgumentNullException("elem");

            UdtWrapper typeWrapper = TypeWrappersPool.Pool.GetTypeWrapper(type, this);

            foreach (var member in GetFieldsToBeSerialized(typeWrapper))
            {
                if (!member.CanWrite)
                    continue;

                // ignore this member if it is attributed as dont serialize
                if (member.IsAttributedAsDontSerialize)
                    continue;

                if (member.IsSerializedAsAttribute)
                {
                    // find the parent element from its location
                    XAttribute attr = XMLUtils.FindAttribute(elem, member.SerializationLocation, member.Alias);
                    if (attr != null)
                    {
                        return true;
                    }
                    else
                    {
                        // maybe it has got a realtype attribute and hence have turned into an element
                        XElement theElem = XMLUtils.FindElement(elem, member.SerializationLocation, member.Alias);
                        if (theElem != null && theElem.Attribute(s_namespaceURI + s_trueTypeAttrName) != null)
                            return true;
                    }
                }
                else
                {
                    XElement xelem = XMLUtils.FindElement(elem, member.SerializationLocation, member.Alias);
                    if (xelem != null)
                    {
                        return true;
                    }
                    else
                    {
                        if (!ReflectionUtils.IsBasicType(member.MemberType) && !member.IsTreatedAsCollection && !member.IsTreatedAsDictionary)
                        {
                            // try to create a fake element 
                            XElement fakeElem = XMLUtils.CreateElement(elem, member.SerializationLocation, member.Alias);
                            if (fakeElem != null)
                            {
                                bool memberExists = AtLeastOneOfMembersExists(fakeElem, member.MemberType);
                                fakeElem.Remove();
                                if (memberExists)
                                    return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Retreives the value of the element from the specified XML element or attribute.
        /// </summary>
        /// <param name="o">The object to store the retrieved value at.</param>
        /// <param name="member">The member of the specified object whose value we intent to retreive.</param>
        /// <param name="elemValue">The value of the element stored as string.</param>
        /// <param name="xelemValue">The XML element value to be retreived. If the value to be retreived
        /// has been stored in an XML attribute, this reference is <c>null</c>.</param>
        private void RetreiveElementValue(object o, MemberWrapper member, string elemValue, XElement xelemValue)
        {
            Type memberType = member.MemberType;

            // try to retrieve the real-type if specified
            if (xelemValue != null)
            {
                XAttribute realTypeAttribute = xelemValue.Attribute(s_namespaceURI + s_trueTypeAttrName);
                if (realTypeAttribute != null)
                {
                    Type realType = ReflectionUtils.GetTypeByName(realTypeAttribute.Value);
                    if (realType != null)
                    {
                        memberType = realType;
                    }
                }
            }

            if (xelemValue != null && XMLUtils.IsElementCompletelyEmpty(xelemValue) &&
                !ReflectionUtils.IsBasicType(memberType) && !member.IsTreatedAsCollection && !member.IsTreatedAsDictionary &&
                !AtLeastOneOfMembersExists(xelemValue, memberType))
            {
                try
                {
                    member.SetValue(o, member.DefaultValue);
                }
                catch
                {
                    this.OnExceptionOccurred(new YAXDefaultValueCannotBeAssigned(member.Alias, member.DefaultValue), member.TreatErrorsAs);
                }
            }
            else if (memberType == typeof(string))
            {
                if (String.IsNullOrEmpty(elemValue) && xelemValue != null)
                {
                    if (xelemValue.IsEmpty)
                        elemValue = null;
                    else
                        elemValue = "";
                }

                try
                {
                    member.SetValue(o, elemValue);
                }
                catch
                {
                    this.OnExceptionOccurred(new YAXPropertyCannotBeAssignedTo(member.Alias), this.m_defaultExceptionType);
                }
            }
            else if (ReflectionUtils.IsBasicType(memberType))
            {
                object convertedObj;

                if (ReflectionUtils.IsNullable(memberType) && String.IsNullOrEmpty(elemValue))
                {
                    convertedObj = member.DefaultValue;
                }
                else
                {
                    convertedObj = ReflectionUtils.ConvertBasicType(elemValue, memberType);
                }

                try
                {
                    try
                    {
                        member.SetValue(o, convertedObj);
                    }
                    catch
                    {
                        this.OnExceptionOccurred(new YAXPropertyCannotBeAssignedTo(member.Alias), this.m_defaultExceptionType);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is YAXException)
                    {
                        throw;
                    }

                    this.OnExceptionOccurred(new YAXBadlyFormedInput(member.Alias, elemValue), member.TreatErrorsAs);

                    try
                    {
                        member.SetValue(o, member.DefaultValue);
                    }
                    catch
                    {
                        this.OnExceptionOccurred(new YAXDefaultValueCannotBeAssigned(member.Alias, member.DefaultValue), this.m_defaultExceptionType);
                    }
                }
            }
            else if (member.IsTreatedAsDictionary && member.DictionaryAttributeInstance != null)
            {
                DeserializeTaggedDictionaryMember(o, member, memberType, xelemValue);
            }
            else if (member.IsTreatedAsCollection)
            {
                DeserializeCollectionMember(o, member, memberType, elemValue, xelemValue);
            }
            else
            {
                YAXSerializer ser = new YAXSerializer(memberType, this.m_exceptionPolicy, this.m_defaultExceptionType, this.m_serializationOption);
                ser.IsCraetedToDeserializeANonCollectionMember = !(member.IsTreatedAsDictionary || member.IsTreatedAsCollection);

                if (m_desObject != null) // i.e. it is in resuming mode
                {
                    ser.SetDeserializationBaseObject(member.GetValue(o));
                }

                object convertedObj = ser.DeserializeBase(xelemValue);
                this.m_parsingErrors.AddRange(ser.ParsingErrors);

                try
                {
                    member.SetValue(o, convertedObj);
                }
                catch
                {
                    this.OnExceptionOccurred(new YAXPropertyCannotBeAssignedTo(member.Alias), this.m_defaultExceptionType);
                }
            }
        }

        /// <summary>
        /// Retreives the collection value.
        /// </summary>
        /// <param name="colType">Type of the collection to be retrieved.</param>
        /// <param name="xelemValue">The value of xml element.</param>
        /// <param name="memberAlias">The member's alias, used only in exception titles.</param>
        /// <param name="colAttrInstance">The collection attribute instance.</param>
        /// <returns></returns>
        private object DeserializeCollectionValue(Type colType, XElement xelemValue, string memberAlias, YAXCollectionAttribute colAttrInstance)
        {
            List<object> lst = new List<object>(); // this will hold the actual data items
            Type itemType = ReflectionUtils.GetCollectionItemType(colType);

            if (ReflectionUtils.IsBasicType(itemType) && colAttrInstance != null && colAttrInstance.SerializationType == YAXCollectionSerializationTypes.Serially)
            {
                // What if the collection was serialized serially
                char[] seps = colAttrInstance.SeparateBy.ToCharArray();

                // can white space characters be added to the separators?
                if (colAttrInstance == null || colAttrInstance.IsWhiteSpaceSeparator)
                {
                    seps = seps.Union(new char[] { ' ', '\t', '\r', '\n' }).ToArray();
                }

                string elemValue = xelemValue.Value;
                string[] items = elemValue.Split(seps, StringSplitOptions.RemoveEmptyEntries);

                foreach (string wordItem in items)
                {
                    try
                    {
                        lst.Add(ReflectionUtils.ConvertBasicType(wordItem, itemType));
                    }
                    catch
                    {
                        OnExceptionOccurred(new YAXBadlyFormedInput(memberAlias, elemValue), m_defaultExceptionType);
                    }
                }
            }
            else
            {
                //What if the collection was serialized recursive
                bool isPrimitive = false;

                if (ReflectionUtils.IsBasicType(itemType))
                {
                    isPrimitive = true;
                }

                string eachElemName = ReflectionUtils.GetTypeFriendlyName(itemType);
                if (colAttrInstance != null && colAttrInstance.EachElementName != null)
                {
                    eachElemName = colAttrInstance.EachElementName;
                }

                foreach (XElement childElem in xelemValue.Elements(eachElemName))
                {
                    Type curElementType = itemType;
                    bool curElementIsPrimitive = isPrimitive;

                    XAttribute realTypeAttribute = childElem.Attribute(s_namespaceURI + s_trueTypeAttrName);
                    if (realTypeAttribute != null)
                    {
                        Type theRealType = ReflectionUtils.GetTypeByName(realTypeAttribute.Value);
                        if (theRealType != null)
                        {
                            curElementType = theRealType;
                            curElementIsPrimitive = ReflectionUtils.IsBasicType(curElementType);
                        }
                    }

                    if (curElementIsPrimitive)
                    {
                        try
                        {
                            lst.Add(ReflectionUtils.ConvertBasicType(childElem.Value, curElementType));
                        }
                        catch
                        {
                            this.OnExceptionOccurred(new YAXBadlyFormedInput(childElem.Name.ToString(), childElem.Value), this.m_defaultExceptionType);
                        }
                    }
                    else
                    {
                        YAXSerializer ser = new YAXSerializer(curElementType, this.m_exceptionPolicy, this.m_defaultExceptionType, this.m_serializationOption);
                        lst.Add(ser.DeserializeBase(childElem));
                        this.m_parsingErrors.AddRange(ser.ParsingErrors);
                    }
                }
            } // end of else if 

            // Now what should I do with the filled list: lst
            Type dicKeyType, dicValueType;
            if (ReflectionUtils.IsArray(colType))
            {
                XAttribute dimsAttr = xelemValue.Attribute(s_namespaceURI + s_dimsAttrName);
                int[] dims = new int[0];
                if (dimsAttr != null)
                {
                    dims = StringUtils.ParseArrayDimsString(dimsAttr.Value);
                }

                Array arrayInstance = null;
                if (dims.Length > 0)
                {
                    int[] lowerBounds = new int[dims.Length]; // an array of zeros
                    arrayInstance = Array.CreateInstance(itemType, dims, lowerBounds); // create the array

                    int count = Math.Min(arrayInstance.Length, lst.Count);
                    // now fill the array
                    for (int i = 0; i < count; i++)
                    {
                        int[] inds = GetArrayDimentionalIndex(i, dims);
                        try
                        {
                            arrayInstance.SetValue(lst[i], inds);
                        }
                        catch
                        {
                            OnExceptionOccurred(
                                new YAXCannotAddObjectToCollection(memberAlias, lst[i]),
                                this.m_defaultExceptionType);
                        }
                    }
                }
                else
                {
                    arrayInstance = Array.CreateInstance(itemType, lst.Count); // create the array

                    int count = Math.Min(arrayInstance.Length, lst.Count);
                    // now fill the array
                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            arrayInstance.SetValue(lst[i], i);
                        }
                        catch
                        {
                            OnExceptionOccurred(
                                new YAXCannotAddObjectToCollection(memberAlias, lst[i]),
                                this.m_defaultExceptionType);
                        }
                    }
                }

                return arrayInstance;
            }
            else if (ReflectionUtils.IsIDictionary(colType, out dicKeyType, out dicValueType))
            {
                //The collection is a Dictionary
                object dic = colType.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, new object[0]);

                object key, value;
                foreach (var lstItem in lst)
                {
                    key = itemType.GetProperty("Key").GetValue(lstItem, null);
                    value = itemType.GetProperty("Value").GetValue(lstItem, null);
                    try
                    {
                        colType.InvokeMember("Add", BindingFlags.InvokeMethod, null, dic, new object[] { key, value });
                    }
                    catch
                    {
                        this.OnExceptionOccurred(new YAXCannotAddObjectToCollection(memberAlias, lstItem), this.m_defaultExceptionType);
                    }
                }

                return dic;
            }
            else if (ReflectionUtils.IsNonGenericIDictionary(colType))
            {
                object col = colType.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, new object[0]);
                foreach (var lstItem in lst)
                {
                    object key = lstItem.GetType().GetProperty("Key", BindingFlags.Instance | BindingFlags.Public).GetValue(lstItem, null);
                    object value = lstItem.GetType().GetProperty("Value", BindingFlags.Instance | BindingFlags.Public).GetValue(lstItem, null);

                    try
                    {
                        colType.InvokeMember("Add", BindingFlags.InvokeMethod, null, col, new object[] { key, value });
                    }
                    catch
                    {
                        this.OnExceptionOccurred(new YAXCannotAddObjectToCollection(memberAlias, lstItem), this.m_defaultExceptionType);
                    }
                }

                return col;
            }
            else if (ReflectionUtils.IsTypeEqualOrInheritedFromType(colType, typeof(BitArray)))
            {
                bool[] bArray = new bool[lst.Count];
                for (int i = 0; i < bArray.Length; i++)
                {
                    try
                    {
                        bArray[i] = (bool)lst[i];
                    }
                    catch
                    {
                    }
                }

                object col = colType.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, new object[] { bArray });

                return col;
            }
            else if (ReflectionUtils.IsTypeEqualOrInheritedFromType(colType, typeof(Stack)) ||
                ReflectionUtils.IsTypeEqualOrInheritedFromType(colType, typeof(Stack<>)))
            {
                object col = colType.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, new object[0]);

                string additionMethodName = "Push";

                for (int i = lst.Count - 1; i >= 0; i--) // the loop must be from end to front
                {
                    try
                    {
                        colType.InvokeMember(additionMethodName, BindingFlags.InvokeMethod, null, col, new object[] { lst[i] });
                    }
                    catch
                    {
                        this.OnExceptionOccurred(new YAXCannotAddObjectToCollection(memberAlias, lst[i]), this.m_defaultExceptionType);
                    }
                }

                return col;
            }
            else if (ReflectionUtils.IsIEnumerable(colType))
            {
                object col;
                try
                {
                    col = colType.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, new object[0]);
                }
                catch
                {
                    return lst;
                }

                string additionMethodName = "Add";

                if (ReflectionUtils.IsTypeEqualOrInheritedFromType(colType, typeof(Queue)) ||
                    ReflectionUtils.IsTypeEqualOrInheritedFromType(colType, typeof(Queue<>)))
                {
                    additionMethodName = "Enqueue";
                }
                else if (ReflectionUtils.IsTypeEqualOrInheritedFromType(colType, typeof(LinkedList<>)))
                {
                    additionMethodName = "AddLast";
                }

                foreach (var lstItem in lst)
                {
                    try
                    {
                        colType.InvokeMember(additionMethodName, BindingFlags.InvokeMethod, null, col, new object[] { lstItem });
                    }
                    catch
                    {
                        this.OnExceptionOccurred(new YAXCannotAddObjectToCollection(memberAlias, lstItem), this.m_defaultExceptionType);
                    }
                }

                return col;
            }

            return null;
        }

        /// <summary>
        /// Deserializes the collection member.
        /// </summary>
        /// <param name="o">The object to store the retrieved value at.</param>
        /// <param name="member">The member of the specified object whose value we intent to retreive.</param>
        /// <param name="colType">Type of the collection to be retrieved.</param>
        /// <param name="elemValue">The value of the element stored as string.</param>
        /// <param name="xelemValue">The XML element value to be retreived. If the value to be retreived
        /// has been stored in an XML attribute, this reference is <c>null</c>.</param>
        private void DeserializeCollectionMember(object o, MemberWrapper member, Type colType, string elemValue, XElement xelemValue)
        {
            object colObject = DeserializeCollectionValue(colType, xelemValue, member.Alias, member.CollectionAttributeInstance);

            try
            {
                member.SetValue(o, colObject);
            }
            catch
            {
                this.OnExceptionOccurred(new YAXPropertyCannotBeAssignedTo(member.Alias), this.m_defaultExceptionType);
            }
        }

        /// <summary>
        /// Gets the dimensional index for an element of a multi-dimensional array from a
        /// linear index specified. 
        /// </summary>
        /// <param name="linInd">The linear index.</param>
        /// <param name="dims">The dimensions of the array.</param>
        /// <returns></returns>
        private static int[] GetArrayDimentionalIndex(long linInd, int[] dims)
        {
            int[] result = new int[dims.Length];

            int d = (int)linInd;

            for (int n = dims.Length - 1; n > 0; n--)
            {
                result[n] = d % dims[n];
                d = (d - result[n]) / dims[n];
            }

            result[0] = d;
            return result;
        }

        /// <summary>
        /// Deserializes a dictionary member which also benefits from a YAXDictionary attribute
        /// </summary>
        /// <param name="o">The object to hold the deserialized value.</param>
        /// <param name="member">The member corresponding to the dictionary member.</param>
        /// <param name="memberType">Type of the dictionary member.</param>
        /// <param name="xelemValue">The XML element value.</param>
        private void DeserializeTaggedDictionaryMember(object o, MemberWrapper member, Type memberType, XElement xelemValue)
        {
            // otherwise the "else if(member.IsTreatedAsCollection)" block solves the problem
            Type keyType, valueType;
            if (!ReflectionUtils.IsIDictionary(memberType, out keyType, out valueType))
            {
                throw new Exception("elemValue must be a Dictionary");
            }

            Type pairType = null;
            ReflectionUtils.IsIEnumerable(memberType, out pairType);
            string eachElementName = ReflectionUtils.GetTypeFriendlyName(pairType);
            bool isKeyAttrib = false;
            bool isValueAttrib = false;
            string keyAlias = "Key";
            string valueAlias = "Value";

            if (member.CollectionAttributeInstance != null)
            {
                eachElementName = member.CollectionAttributeInstance.EachElementName ?? eachElementName;
            }

            if (member.DictionaryAttributeInstance != null)
            {
                eachElementName = member.DictionaryAttributeInstance.EachPairName ?? eachElementName;
                if (member.DictionaryAttributeInstance.SerializeKeyAs == YAXNodeTypes.Attribute)
                {
                    isKeyAttrib = ReflectionUtils.IsBasicType(keyType);
                }

                if (member.DictionaryAttributeInstance.SerializeValueAs == YAXNodeTypes.Attribute)
                {
                    isValueAttrib = ReflectionUtils.IsBasicType(valueType);
                }

                keyAlias = member.DictionaryAttributeInstance.KeyName ?? keyAlias;
                valueAlias = member.DictionaryAttributeInstance.ValueName ?? valueAlias;
            }

            object dic = memberType.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, new object[0]);

            foreach (XElement childElem in xelemValue.Elements(eachElementName))
            {
                object key = null, value = null;
                YAXSerializer keySer = null, valueSer = null;

                bool isKeyFound = VerifyDictionaryPairElements(ref keyType, ref isKeyAttrib, keyAlias, childElem);
                bool isValueFound = VerifyDictionaryPairElements(ref valueType, ref isValueAttrib, valueAlias, childElem);

                if (!isKeyFound && !isValueFound)
                    continue;

                if (isKeyFound)
                {
                    if (isKeyAttrib)
                    {
                        key = ReflectionUtils.ConvertBasicType(childElem.Attribute(keyAlias).Value, keyType);
                    }
                    else if (ReflectionUtils.IsBasicType(keyType))
                    {
                        key = ReflectionUtils.ConvertBasicType(childElem.Element(keyAlias).Value, keyType);
                    }
                    else
                    {
                        if (keySer == null)
                        {
                            keySer = new YAXSerializer(keyType, this.m_exceptionPolicy, this.m_defaultExceptionType, this.m_serializationOption);
                        }

                        key = keySer.DeserializeBase(childElem.Element(keyAlias));
                        this.m_parsingErrors.AddRange(keySer.ParsingErrors);
                    }
                }

                if (isValueFound)
                {
                    if (isValueAttrib)
                    {
                        value = ReflectionUtils.ConvertBasicType(childElem.Attribute(valueAlias).Value, valueType);
                    }
                    else if (ReflectionUtils.IsBasicType(valueType))
                    {
                        value = ReflectionUtils.ConvertBasicType(childElem.Element(valueAlias).Value, valueType);
                    }
                    else
                    {
                        if (valueSer == null)
                        {
                            valueSer = new YAXSerializer(valueType, this.m_exceptionPolicy, this.m_defaultExceptionType, this.m_serializationOption);
                        }

                        value = valueSer.DeserializeBase(childElem.Element(valueAlias));
                        this.m_parsingErrors.AddRange(valueSer.ParsingErrors);
                    }
                }

                try
                {
                    memberType.InvokeMember("Add", BindingFlags.InvokeMethod, null, dic, new object[] { key, value });
                }
                catch
                {
                    this.OnExceptionOccurred(
                        new YAXCannotAddObjectToCollection(member.Alias, new KeyValuePair<object, object>(key, value)),
                        this.m_defaultExceptionType);
                }
            }

            try
            {
                member.SetValue(o, dic);
            }
            catch
            {
                this.OnExceptionOccurred(new YAXPropertyCannotBeAssignedTo(member.Alias), this.m_defaultExceptionType);
            }
        }

        /// <summary>
        /// Verifies the existence of dictionary pair <c>Key</c> and <c>Value</c> elements.
        /// </summary>
        /// <param name="keyType">Type of the key.</param>
        /// <param name="isKeyAttrib">if set to <c>true</c> means that key has been serialize as an attribute.</param>
        /// <param name="keyAlias">The alias for <c>Key</c>.</param>
        /// <param name="childElem">The child XML elemenet to search <c>Key</c> and <c>Value</c> elements in.</param>
        /// <returns></returns>
        private static bool VerifyDictionaryPairElements(ref Type keyType, ref bool isKeyAttrib, string keyAlias, XElement childElem)
        {
            bool isKeyFound = false;
            if (isKeyAttrib && childElem.Attribute(keyAlias) != null)
            {
                isKeyFound = true;
            }
            else if (isKeyAttrib)
            {
                // loook for an element with the same name AND a yaxlib:realtype attribute
                XElement elem = childElem.Element(keyAlias);
                if (elem != null)
                {
                    XAttribute realTypeAttr = elem.Attribute(s_namespaceURI + s_trueTypeAttrName);
                    if (realTypeAttr != null)
                    {
                        Type theRealType = ReflectionUtils.GetTypeByName(realTypeAttr.Value);
                        if (theRealType != null)
                        {
                            keyType = theRealType;
                            isKeyAttrib = false;
                            isKeyFound = true;
                        }
                    }
                }
            }
            else
            {
                XElement elem = childElem.Element(keyAlias);
                if (elem != null)
                {
                    isKeyFound = true;

                    XAttribute realTypeAttr = elem.Attribute(s_namespaceURI + s_trueTypeAttrName);
                    if (realTypeAttr != null)
                    {
                        Type theRealType = ReflectionUtils.GetTypeByName(realTypeAttr.Value);
                        if (theRealType != null)
                        {
                            keyType = theRealType;
                        }
                    }
                }
            }

            return isKeyFound;
        }

        /// <summary>
        /// Deserializes the XML reperesentation of a key-value pair, as specified, and returns 
        /// a <c>KeyValuePair</c> instance containing the deserialized data.
        /// </summary>
        /// <param name="baseElement">The element contating the XML reperesentation of a key-value pair.</param>
        /// <returns>a <c>KeyValuePair</c> instance containing the deserialized data</returns>
        private object DeserializeKeyValuePair(XElement baseElement)
        {
            Type[] genArgs = this.m_type.GetGenericArguments();
            Type keyType = genArgs[0];
            Type valueType = genArgs[1];

            object keyValue, valueValue;
            if (ReflectionUtils.IsBasicType(keyType))
            {
                try
                {
                    keyValue = ReflectionUtils.ConvertBasicType(baseElement.Element("Key").Value, keyType);
                }
                catch (NullReferenceException)
                {
                    keyValue = null;
                }
            }
            else if (ReflectionUtils.IsStringConvertibleIFormattable(keyType))
            {
                keyValue = keyType.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[] { baseElement.Element("Key").Value });
            }
            else if (ReflectionUtils.IsCollectionType(keyType))
            {
                keyValue = DeserializeCollectionValue(keyType, baseElement.Element("Key"), "Key", null);
            }
            else
            {
                YAXSerializer ser = new YAXSerializer(keyType, this.m_exceptionPolicy, this.m_defaultExceptionType, this.m_serializationOption);
                keyValue = ser.DeserializeBase(baseElement.Element("Key"));
                this.m_parsingErrors.AddRange(ser.ParsingErrors);
            }

            if (ReflectionUtils.IsBasicType(valueType))
            {
                try
                {
                    valueValue = ReflectionUtils.ConvertBasicType(baseElement.Element("Value").Value, valueType);
                }
                catch (NullReferenceException)
                {
                    valueValue = null;
                }
            }
            else if (ReflectionUtils.IsStringConvertibleIFormattable(valueType))
            {
                valueValue = valueType.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[] { baseElement.Element("Value").Value });
            }
            else if (ReflectionUtils.IsCollectionType(valueType))
            {
                valueValue = DeserializeCollectionValue(valueType, baseElement.Element("Value"), "Value", null);
            }
            else
            {
                YAXSerializer ser = new YAXSerializer(valueType, this.m_exceptionPolicy, this.m_defaultExceptionType, this.m_serializationOption);
                valueValue = ser.DeserializeBase(baseElement.Element("Value"));
                this.m_parsingErrors.AddRange(ser.ParsingErrors);
            }

            object pair = this.m_type.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, new object[] { keyValue, valueValue });

            return pair;
        }

        /// <summary>
        /// Deserializes XML for some known .NET built in type.
        /// </summary>
        /// <param name="baseElement">The XML element to deserialize.</param>
        /// <param name="type">The type of the desired object.</param>
        /// <returns>deserialized object of some known .NET built-in type</returns>
        private object DeserializeKnownDotNetBuiltInType(XElement baseElement, Type type)
        {
            if (type == typeof(TimeSpan))
                return KnownDotNetTypesDeserializers.DeserializeTimeSpan(baseElement);
            else if (type == typeof(Guid))
                return KnownDotNetTypesDeserializers.DeserializeGuid(baseElement);
            return null;
        }

        /// <summary>
        /// Determines whether the specified type is a known .NET built-in type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// 	<c>true</c> if the specified type is a known .NET built-in type; otherwise, <c>false</c>.
        /// </returns>
        private bool IsKnownDotNetBuiltInType(Type type)
        {
            // add other cases here
            if (type == typeof(TimeSpan) || type == typeof(Guid))
                return true;
            return false;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets the sequence of fields to be serialized for the specified type. This sequence is retreived according to 
        /// the field-types specified by the user.
        /// </summary>
        /// <param name="typeWrapper">The type wrapper for the type whose serializable 
        /// fields is going to be retreived.</param>
        /// <returns>the sequence of fields to be serialized for the specified type</returns>
        private IEnumerable<MemberWrapper> GetFieldsToBeSerialized(UdtWrapper typeWrapper)
        {
            foreach (var member in typeWrapper.UnderlyingType.GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                char name0 = member.Name[0];
                if ((Char.IsLetter(name0) || name0 == '_') &&
                    (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field))
                {
                    MemberWrapper memInfo = new MemberWrapper(member, this);
                    if (memInfo.IsAllowedToBeSerialized(typeWrapper.FieldsToSerialize))
                    {
                        yield return memInfo;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the sequence of fields to be serialized for the serializer's underlying type. 
        /// This sequence is retreived according to the field-types specified by the user.
        /// </summary>
        /// <returns>the sequence of fields to be serialized for the serializer's underlying type.</returns>
        private IEnumerable<MemberWrapper> GetFieldsToBeSerialized()
        {
            return GetFieldsToBeSerialized(m_udtWrapper);
        }

        /// <summary>
        /// Called when an exception occurs inside the library. It applies the exception handling policies.
        /// </summary>
        /// <param name="ex">The exception that has occurred.</param>
        /// <param name="exceptionType">Type of the exception.</param>
        private void OnExceptionOccurred(YAXException ex, YAXExceptionTypes exceptionType)
        {
            m_exceptionOccurredDuringMemberDeserialization = true;
            if (exceptionType == YAXExceptionTypes.Ignore)
            {
                return;
            }

            this.m_parsingErrors.AddException(ex, exceptionType);
            if ((this.m_exceptionPolicy == YAXExceptionHandlingPolicies.ThrowWarningsAndErrors) ||
                (this.m_exceptionPolicy == YAXExceptionHandlingPolicies.ThrowErrorsOnly && exceptionType == YAXExceptionTypes.Error))
            {
                throw ex;
            }
        }

        #endregion

        #endregion
    }
}
