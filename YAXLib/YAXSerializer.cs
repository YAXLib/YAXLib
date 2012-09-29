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
        private bool m_needsYaxLibNamespaceAddition = false;

        /// <summary>
        /// The URI address which holds the xmlns:yaxlib definition.
        /// </summary>
        private static readonly XNamespace s_yaxLibNamespaceUri = XNamespace.Get("http://www.sinairv.com/yaxlib/");

        /// <summary>
        /// The initials used for the xml namespace
        /// </summary>
        private const string s_yaxLibNamespaceInits = "yaxlib";

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
            m_type = t;
            m_exceptionPolicy = exceptionPolicy;
            m_defaultExceptionType = defaultExType;
            m_serializationOption = option;
            // this must be the last call
            m_udtWrapper = TypeWrappersPool.Pool.GetTypeWrapper(m_type, this);
            if (m_udtWrapper.HasNamespace)
                TypeNamespace = m_udtWrapper.Namespace;
        }

        #endregion

        #region Properties

        internal XNamespace TypeNamespace { get; set; }

        internal bool HasTypeNamespace 
        { 
            get 
            {
                return TypeNamespace.HasNamespace();
            }
        }

        internal void SetNamespaceToOverrideEmptyNamespace(XNamespace otherNamespace)
        {
            // if namespace info is not already set during construction, 
            // then set it from the other YAXSerializer instance
            if (otherNamespace.HasNamespace() && !this.HasTypeNamespace)
            {
                this.TypeNamespace = otherNamespace;
            }
        }

        //internal void SetNamespaceToOverrideDefaultNamespace(YAXSerializer other)
        //{
        //    // if namespace info is not already set during construction, 
        //    // then set it from the other YAXSerializer instance
        //    if (other.HasTypeNamespace && !this.HasTypeNamespace)
        //    {
        //        this.TypeNamespace = other.TypeNamespace;
        //    }
        //}

        /// <summary>
        /// Gets the default type of the exception.
        /// </summary>
        /// <value>The default type of the exception.</value>
        public YAXExceptionTypes DefaultExceptionType
        {
            get
            {
                return m_defaultExceptionType;
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
                return m_serializationOption;
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
                return m_exceptionPolicy;
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
                return m_parsingErrors;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is created to deserialize a non collection member of another object.
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
        /// Serializes the specified object and returns an instance of <c>XDocument</c> containing the result.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>An instance of <c>XDocument</c> containing the resulting XML</returns>
        public XDocument SerializeToXDocument(object obj)
        {
            return SerializeXDocument(obj);
        }


        /// <summary>
        /// Serializes the specified object into a <c>TextWriter</c> instance.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="textWriter">The <c>TextWriter</c> instance.</param>
        public void Serialize(object obj, TextWriter textWriter)
        {
            textWriter.Write(Serialize(obj));
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
                    var xdoc = XDocument.Load(tr);
                    var baseElement = xdoc.Root;
                    return DeserializeBase(baseElement);
                }
            }
            catch (XmlException ex)
            {
                OnExceptionOccurred(new YAXBadlyFormedXML(ex), m_defaultExceptionType);
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
                return DeserializeBase(baseElement);
            }
            catch (XmlException ex)
            {
                OnExceptionOccurred(new YAXBadlyFormedXML(ex), m_defaultExceptionType);
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
                return DeserializeBase(baseElement);
            }
            catch (XmlException ex)
            {
                OnExceptionOccurred(new YAXBadlyFormedXML(ex), m_defaultExceptionType);
                return null;
            }
        }

        /// <summary>
        /// Deserializes an object while reading from an instance of <c>XElement</c>
        /// </summary>
        /// <param name="element">The <c>XElement</c> instance to read from.</param>
        /// <returns>The deserialized object</returns>
        public object Deserialize(XElement element)
        {
            try
            {
                var xdoc = new XDocument();
                xdoc.Add(element);
                return DeserializeBase(element);
            }
            catch (XmlException ex)
            {
                OnExceptionOccurred(new YAXBadlyFormedXML(ex), m_defaultExceptionType);
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
                return Deserialize(File.ReadAllText(fileName));
            }
            catch (XmlException ex)
            {
                OnExceptionOccurred(new YAXBadlyFormedXML(ex), m_defaultExceptionType);
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
            if (obj != null && !m_type.IsInstanceOfType(obj))
            {
                throw new YAXObjectTypeMismatch(m_type, obj.GetType());
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
            m_mainDocument.Add(SerializeBase(obj));
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
            if (!m_type.IsInstanceOfType(obj))
            {
                throw new YAXObjectTypeMismatch(m_type, obj.GetType());
            }

            // to serialize stand-alone collection or dictionary objects
            if (m_udtWrapper.IsTreatedAsDictionary)
            {
                var elemResult = MakeDictionaryElement(null, m_udtWrapper.Alias, obj, null, null);
                if (m_udtWrapper.PreservesWhitespace)
                    XMLUtils.AddPreserveSpaceAttribute(elemResult);
                return elemResult;
            }
            else if (m_udtWrapper.IsTreatedAsCollection)
            {
                var elemResult = MakeCollectionElement(null, m_udtWrapper.Alias, obj, null, null);
                if (m_udtWrapper.PreservesWhitespace)
                    XMLUtils.AddPreserveSpaceAttribute(elemResult);
                return elemResult;
            }
            else if(ReflectionUtils.IsBasicType(m_udtWrapper.UnderlyingType))
            {
                bool dummyAlreadyAdded;
                var elemResult = MakeBaseElement(null, m_udtWrapper.Alias, obj, out dummyAlreadyAdded);
                if (m_udtWrapper.PreservesWhitespace)
                    XMLUtils.AddPreserveSpaceAttribute(elemResult);
                return elemResult;
            }
            else if(m_udtWrapper.UnderlyingType != obj.GetType())
            {
                // this block of code runs if the serializer is instantiated with a
                // another base value such as System.Object but is provided with an
                // object of its child

                var ser = new YAXSerializer(obj.GetType(), m_exceptionPolicy, 
                    m_defaultExceptionType, m_serializationOption);
                ser.SetNamespaceToOverrideEmptyNamespace(this.TypeNamespace);
                
                //ser.SetBaseElement(insertionLocation);
                var xdoc = ser.SerializeToXDocument(obj);
                var elem = xdoc.Root;

                //if (ser.m_needsNamespaceAddition)
                //    this.m_needsNamespaceAddition = true;
                m_parsingErrors.AddRange(ser.ParsingErrors);
                elem.Name = m_udtWrapper.Alias;
                elem.Add(new XAttribute(s_yaxLibNamespaceUri + s_trueTypeAttrName, obj.GetType().FullName));
                var nsAttrName = XNamespace.Xmlns + s_yaxLibNamespaceInits;
                if(elem.Attribute(nsAttrName) == null)
                    elem.Add(new XAttribute(nsAttrName, s_yaxLibNamespaceUri));
                return elem;
            }
            else
            {
                return SerializeBase(obj, m_udtWrapper.Alias);
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
        private XElement SerializeBase(object obj, XName className)
        {
            XNamespace xmlns = XNamespace.None;
            if (m_udtWrapper.HasNamespace)
                xmlns = m_udtWrapper.Namespace;

            if (m_baseElement == null)
            {
                m_baseElement = CreateElementWithNamespace(m_udtWrapper, className);
            }
            else
            {
                var baseElem = new XElement(className, null);
                m_baseElement.Add(baseElem);
                m_baseElement = baseElem;
            }
            

            if (m_udtWrapper.HasComment && m_baseElement.Parent == null && m_mainDocument != null)
            {
                foreach (string comment in m_udtWrapper.Comment)
                    m_mainDocument.Add(new XComment(comment));
            }

            // if the containing element is set to preserve spaces, then emit the 
            // required attribute
            if(m_udtWrapper.PreservesWhitespace)
            {
                XMLUtils.AddPreserveSpaceAttribute(m_baseElement);
            }

            // check if the main class/type has defined custom serializers
            if (m_udtWrapper.HasCustomSerializer)
            {
                InvokeCustomSerializerToElement(m_udtWrapper.CustomSerializerType, obj, m_baseElement);
            }
            else if(KnownTypes.IsKnowType(m_type))
            {
                KnownTypes.Serialize(obj, m_baseElement, TypeNamespace);
            }
            else // if it has no custom serializers
            {
                // a flag that indicates whether the object had any fields to be serialized
                // if an object did not have any fields to serialize, then we should not remove
                // the containing element from the resulting xml!
                bool isAnythingFoundToSerialize = false;

                // iterate through public properties
                foreach (var member in GetFieldsToBeSerialized())
                {
                    object elementValue = null;

                    XNamespace xmlns_local = xmlns;
                    if (member.HasNamespace)
                    {
                        AddNamespace(member, m_baseElement);
                        xmlns_local = member.Namespace;
                    }


                    if (!member.CanRead)
                        continue;

                    // ignore this member if it is attributed as dont serialize
                    if (member.IsAttributedAsDontSerialize)
                        continue;

                    elementValue = member.GetValue(obj);

                    if (elementValue != null && member.MemberType == m_type)
                    {
                        throw new YAXCannotSerializeSelfReferentialTypes(m_type);
                    }

                    // make this flat true, so that we know that this object was not empty of fields
                    isAnythingFoundToSerialize = true;

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

                    bool hasCustomSerializer = member.HasCustomSerializer || member.MemberTypeWrapper.HasCustomSerializer;
                    bool isCollectionSerially = member.CollectionAttributeInstance != null &&
                                                member.CollectionAttributeInstance.SerializationType == YAXCollectionSerializationTypes.Serially;
                    bool isKnownType = member.IsKnownType;

                    var serializationLocation = XMLUtils.CreateExplicitNamespaceLocationString(m_baseElement, member.SerializationLocation);

                    // it gets true only for basic data types
                    if (member.IsSerializedAsAttribute && (areOfSameType || hasCustomSerializer || isCollectionSerially || isKnownType))
                    {
                        if (!XMLUtils.AttributeExists(m_baseElement, serializationLocation, xmlns_local + member.Alias))
                        {
                            XAttribute attrToCreate = XMLUtils.CreateAttribute(m_baseElement,
                                serializationLocation, xmlns_local + member.Alias, 
                                (hasCustomSerializer || isCollectionSerially || isKnownType) ? "" : elementValue);

                            if (attrToCreate == null)
                            {
                                throw new YAXBadLocationException(serializationLocation);
                            }

                            if (member.HasCustomSerializer)
                            {
                                InvokeCustomSerializerToAttribute(member.CustomSerializerType, elementValue, attrToCreate);
                            }
                            else if (member.MemberTypeWrapper.HasCustomSerializer)
                            {
                                InvokeCustomSerializerToAttribute(member.MemberTypeWrapper.CustomSerializerType, elementValue, attrToCreate);
                            }
                            else if (member.IsKnownType)
                            {
                                // TODO: create a functionality to serialize to XAttributes
                                //KnownTypes.Serialize(attrToCreate, member.MemberType);
                            }
                            else if(isCollectionSerially)
                            {
                                var tempLoc = new XElement("temp");
                                var added = MakeCollectionElement(tempLoc, "name", elementValue, member.CollectionAttributeInstance, member.Format);
                                attrToCreate.Value = added.Value;
                            }

                            // if member does not have any typewrappers then it has been already populated with the CreateAttribute method
                        }
                        else
                        {
                            throw new YAXAttributeAlreadyExistsException(member.Alias);
                        }
                    }
                    else if (member.IsSerializedAsValue && (areOfSameType || hasCustomSerializer || isCollectionSerially || isKnownType))
                    {
                        // find the parent element from its location
                        XElement parElem = XMLUtils.FindLocation(m_baseElement, serializationLocation);
                        if (parElem == null) // if the parent element does not exist
                        {
                            // see if the location can be created
                            if (!XMLUtils.CanCreateLocation(m_baseElement, serializationLocation))
                                throw new YAXBadLocationException(serializationLocation);
                            // try to create the location
                            parElem = XMLUtils.CreateLocation(m_baseElement, serializationLocation);
                            if (parElem == null)
                                throw new YAXBadLocationException(serializationLocation);
                        }

                        // if control is moved here, it means that the parent 
                        // element has been found/created successfully

                        string valueToSet;
                        if (member.HasCustomSerializer)
                        {
                            valueToSet = InvokeCustomSerializerToValue(member.CustomSerializerType, elementValue);
                        }
                        else if (member.MemberTypeWrapper.HasCustomSerializer)
                        {
                            valueToSet = InvokeCustomSerializerToValue(member.MemberTypeWrapper.CustomSerializerType, elementValue);
                        }
                        else if(isKnownType)
                        {
                            var tempLoc = new XElement("temp");
                            KnownTypes.Serialize(elementValue, tempLoc, "");
                            valueToSet = tempLoc.Value;
                        }
                        else if (isCollectionSerially)
                        {
                            var tempLoc = new XElement("temp");
                            var added = MakeCollectionElement(tempLoc, "name", elementValue, member.CollectionAttributeInstance, member.Format);
                            valueToSet = added.Value;
                        }
                        else
                        {
                            valueToSet = (elementValue ?? String.Empty).ToString();
                        }

                        parElem.Add(new XText(valueToSet));
                        if (member.PreservesWhitespace)
                            XMLUtils.AddPreserveSpaceAttribute(parElem);
                    }
                    else // if the data is going to be serialized as an element
                    {
                        // find the parent element from its location
                        XElement parElem = XMLUtils.FindLocation(m_baseElement, serializationLocation);
                        if (parElem == null) // if the parent element does not exist
                        {
                            // see if the location can be created
                            if (!XMLUtils.CanCreateLocation(m_baseElement, serializationLocation))
                                throw new YAXBadLocationException(serializationLocation);
                            // try to create the location
                            parElem = XMLUtils.CreateLocation(m_baseElement, serializationLocation);
                            if (parElem == null)
                                throw new YAXBadLocationException(serializationLocation);
                        }

                        // if control is moved here, it means that the parent 
                        // element has been found/created successfully

                        if (member.HasComment)
                        {
                            foreach (string comment in member.Comment)
                                parElem.Add(new XComment(comment));
                        }

                        if (hasCustomSerializer)
                        {
                            var elemToFill = new XElement(xmlns_local + member.Alias);
                            parElem.Add(elemToFill);
                            if (member.HasCustomSerializer)
                            {
                                InvokeCustomSerializerToElement(member.CustomSerializerType, elementValue, elemToFill);
                            }
                            else if (member.MemberTypeWrapper.HasCustomSerializer)
                            {
                                InvokeCustomSerializerToElement(member.MemberTypeWrapper.CustomSerializerType, elementValue, elemToFill);
                            }

                            if (member.PreservesWhitespace)
                                XMLUtils.AddPreserveSpaceAttribute(elemToFill);
                        }
                        else if(isKnownType)
                        {
                            var elemToFill = new XElement(xmlns_local + member.Alias);
                            parElem.Add(elemToFill);
                            KnownTypes.Serialize(elementValue, elemToFill, xmlns_local.NamespaceName);
                            if (member.PreservesWhitespace)
                                XMLUtils.AddPreserveSpaceAttribute(elemToFill);
                        }
                        else
                        {
                            // make an element with the provided data
                            bool moveDescOnly = false;
                            bool alreadyAdded = false;
                            XElement elemToAdd = MakeElement(parElem, member, elementValue, out moveDescOnly, out alreadyAdded);
                            if (!areOfSameType)
                            {
                                elemToAdd.Add(new XAttribute(s_yaxLibNamespaceUri + s_trueTypeAttrName, elementValue.GetType().FullName));
                                m_needsYaxLibNamespaceAddition = true;
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
                                    XElement existingElem = parElem.Element(GetXNameForMember(member, member.Alias));
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
                    } // end of if serialize data as Element
                } // end of foreach var member

                // This if statement is important. It checks if all the members of an element
                // have been serialized somewhere else, leaving the containing member empty, then
                // remove that element by itself. However if the element is empty, because the 
                // corresponding object did not have any fields to serialize (e.g., DBNull, Random)
                // then keep that element
                if (m_baseElement.Parent != null &&
                    XMLUtils.IsElementCompletelyEmpty(m_baseElement) &&
                    isAnythingFoundToSerialize)
                {
                    m_baseElement.Remove();
                }

            } // end of else if it has no custom serializers

            if (m_baseElement.Parent == null && m_needsYaxLibNamespaceAddition)
            {
                m_baseElement.Add(new XAttribute(XNamespace.Xmlns + s_yaxLibNamespaceInits, s_yaxLibNamespaceUri));
            }

            return m_baseElement;
        }

        /// <summary>
        /// Adds the namespace applying to the object type specified in <paramref name="wrapper"/>
        /// to the <paramref name="className"/>
        /// </summary>
        /// <param name="wrapper">The wrapper around the object who's namespace should be added</param>
        /// <param name="className">The root node of the document to which the namespace should be written</param>
        private XElement CreateElementWithNamespace(UdtWrapper wrapper, XName className)
        {
            if (!wrapper.HasNamespace)
                return new XElement(className.LocalName, null);

            XNamespace targetNs = wrapper.Namespace;

            if (string.IsNullOrEmpty(wrapper.NamespacePrefix))
                return new XElement(targetNs + className.LocalName, null);
            else
                return new XElement(targetNs + className.LocalName, new XAttribute(XNamespace.Xmlns + wrapper.NamespacePrefix, wrapper.Namespace));
        }
        
        /// <summary>
        /// Adds the namespace applying to the object type specified in <paramref name="wrapper"/>
        /// to the <paramref name="rootNode"/>
        /// </summary>
        /// <param name="wrapper">The wrapper around the object who's namespace should be added</param>
        /// <param name="rootNode">The root node of the document to which the namespace should be written</param>
        private void AddNamespace(MemberWrapper wrapper, XElement rootNode)
        {
            if (!wrapper.HasNamespace)
                return;

            //Adds the defined namespace to the document root
            if (!String.IsNullOrEmpty(wrapper.NamespacePrefix) && rootNode.GetNamespaceOfPrefix(wrapper.NamespacePrefix) == null)
                rootNode.Add(new XAttribute(XNamespace.Xmlns + wrapper.NamespacePrefix, wrapper.Namespace));
            else if (String.IsNullOrEmpty(wrapper.NamespacePrefix))
                throw new InvalidOperationException("Fields or Properties cannot redefine the document's default namespace");
            else
            {
                var existing = rootNode.GetNamespaceOfPrefix(wrapper.NamespacePrefix);

                if (existing.NamespaceName != wrapper.Namespace)
                    throw new InvalidOperationException("You cannot have two different namespaces with the same prefix");
            }
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

            XNamespace xmlns = m_baseElement.Name.Namespace;
            if (member.HasNamespace)
                xmlns = member.Namespace;

            AddNamespace(member, m_baseElement);

            XElement elemToAdd;
            if (member.IsTreatedAsDictionary)
            {
                elemToAdd = MakeDictionaryElement(insertionLocation, xmlns + member.Alias, elementValue, member.DictionaryAttributeInstance, member.CollectionAttributeInstance);
                if (member.CollectionAttributeInstance != null &&
                    member.CollectionAttributeInstance.SerializationType == YAXCollectionSerializationTypes.RecursiveWithNoContainingElement &&
                    !elemToAdd.HasAttributes)
                    moveDescOnly = true;
            }
            else if (member.IsTreatedAsCollection)
            {
                elemToAdd = MakeCollectionElement(insertionLocation, xmlns + member.Alias, elementValue, member.CollectionAttributeInstance, member.Format);

                if (member.CollectionAttributeInstance != null &&
                    member.CollectionAttributeInstance.SerializationType == YAXCollectionSerializationTypes.RecursiveWithNoContainingElement &&
                    !elemToAdd.HasAttributes)
                    moveDescOnly = true;
            }
            else
            {
                elemToAdd = MakeBaseElement(insertionLocation, xmlns + member.Alias, elementValue, out alreadyAdded);
            }

            if (member.PreservesWhitespace)
                XMLUtils.AddPreserveSpaceAttribute(elemToAdd);

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
            XName elementName,
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

            var dicInst = elementValue as IEnumerable;
            bool isKeyAttrib = false;
            bool isValueAttrib = false;
            string keyFormat = null;
            string valueFormat = null;
            XName keyAlias = GetXNameForMember(elementName, "Key");
            XName valueAlias = GetXNameForMember(elementName, "Value");

            XName eachElementName = null;
            if(collectionAttrInst != null && !String.IsNullOrEmpty(collectionAttrInst.EachElementName))
                eachElementName = GetXNameForMember(elementName, collectionAttrInst.EachElementName);

            if (dicAttrInst != null)
            {
                if (dicAttrInst.EachPairName != null)
                    eachElementName =  GetXNameForMember(elementName, dicAttrInst.EachPairName);

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

                keyAlias = GetXNameForMember(elementName, dicAttrInst.KeyName ?? "Key");
                valueAlias = GetXNameForMember(elementName, dicAttrInst.ValueName ?? "Value");
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


                XElement elemChild = new XElement(eachElementName ?? GetXNameForMember(elementName, 
                                                                                       ReflectionUtils.GetTypeFriendlyName(obj.GetType()))
                                                  , null);

                if (isKeyAttrib && areKeyOfSameType)
                {
                    elemChild.Add(new XAttribute(keyAlias, (keyObj ?? string.Empty).ToString()));
                }
                else
                {
                    XElement addedElem = AddObjectToElement(elemChild, keyAlias, keyObj);
                    if (!areKeyOfSameType)
                    {
                        if (addedElem.Parent == null)
                        {
                            // sometimes empty elements are removed because its members are serialized in
                            // other elements, therefore we need to make sure to re-add the element.
                            elemChild.Add(addedElem);
                        }

                        addedElem.Add(new XAttribute(s_yaxLibNamespaceUri + s_trueTypeAttrName, keyObj.GetType().FullName));
                        m_needsYaxLibNamespaceAddition = true;
                    }
                }

                if (isValueAttrib && areValueOfSameType)
                {
                    elemChild.Add(new XAttribute(valueAlias, (valueObj ?? string.Empty).ToString()));
                }
                else
                {
                    XElement addedElem = AddObjectToElement(elemChild, valueAlias, valueObj);
                    if (!areValueOfSameType)
                    {
                        if(addedElem.Parent == null)
                        {
                            // sometimes empty elements are removed because its members are serialized in
                            // other elements, therefore we need to make sure to re-add the element.
                            elemChild.Add(addedElem);
                        }

                        addedElem.Add(new XAttribute(s_yaxLibNamespaceUri + s_trueTypeAttrName, valueObj.GetType().FullName));
                        m_needsYaxLibNamespaceAddition = true;
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
        private XElement AddObjectToElement(XElement elem, XName alias, object obj)
        {
            UdtWrapper udt = TypeWrappersPool.Pool.GetTypeWrapper(obj.GetType(), this);
            
            if (alias == null)
                alias = GetXNameForMember(udt, udt.Alias);

            XElement elemToAdd = null;

            if (udt.IsTreatedAsDictionary)
            {
                elemToAdd = MakeDictionaryElement(elem, alias, obj, null, null);
                elem.Add(elemToAdd);
            }
            else if (udt.IsTreatedAsCollection)
            {
                elemToAdd = MakeCollectionElement(elem, alias, obj, null, null);
                elem.Add(elemToAdd);
            }
            else if (udt.IsEnum)
            {
                bool alreadyAdded;
                elemToAdd = MakeBaseElement(elem, alias, udt.EnumWrapper.GetAlias(obj), out alreadyAdded);
                if (!alreadyAdded)
                    elem.Add(elemToAdd);
            }
            else
            {
                bool alreadyAdded;
                elemToAdd = MakeBaseElement(elem, alias, obj, out alreadyAdded);
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
            XElement insertionLocation, XName elementName, object elementValue,
            YAXCollectionAttribute collectionAttrInst, string format)
        {
            if (elementValue == null)
            {
                return new XElement(elementName, elementValue);
            }

            if (!(elementValue is IEnumerable))
            {
                throw new ArgumentException("elementValue must be an IEnumerable");
            }

            var collectionInst = elementValue as IEnumerable;
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

            if (serType == YAXCollectionSerializationTypes.Serially && !ReflectionUtils.IsBasicType(colItemType))
                serType = YAXCollectionSerializationTypes.Recursive;

            UdtWrapper colItemsUdt = TypeWrappersPool.Pool.GetTypeWrapper(colItemType, this);

            XElement elemToAdd = null; // will hold the resulting element
            if (serType == YAXCollectionSerializationTypes.Serially)
            {
                var sb = new StringBuilder();

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
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", seperator, objToAdd);
                    }
                }

                bool alreadyAdded = false;
                elemToAdd = MakeBaseElement(insertionLocation, elementName, sb.ToString(), out alreadyAdded);
                if (alreadyAdded)
                    elemToAdd = null;
            }
            else
            {
                var elem = new XElement(elementName, null);
                object objToAdd = null;

                foreach (object obj in collectionInst)
                {
                    objToAdd = (format == null) ? obj : ReflectionUtils.TryFormatObject(obj, format);
                    var curElemName = eachElementName;
                    
                    if(curElemName == null)
                    {
                        UdtWrapper udt = TypeWrappersPool.Pool.GetTypeWrapper(obj.GetType(), this);
                        curElemName = udt.Alias;
                    }

                    XElement itemElem = this.AddObjectToElement(elem, elementName.Namespace + curElemName, objToAdd);
                    if (obj.GetType() != colItemType)
                    {
                        itemElem.Add(new XAttribute(s_yaxLibNamespaceUri + s_trueTypeAttrName, obj.GetType().FullName));
                        if (itemElem.Parent == null) // i.e., it has been removed, e.g., because all its members have been serialized outside the element
                            elem.Add(itemElem); // return it back, or undelete this item

                        m_needsYaxLibNamespaceAddition = true;
                    }
                }

                elemToAdd = elem;
            }

            int[] arrayDims = ReflectionUtils.GetArrayDimensions(elementValue);
            if (arrayDims != null && arrayDims.Length > 1)
            {
                elemToAdd.Add(new XAttribute(s_yaxLibNamespaceUri + s_dimsAttrName, StringUtils.GetArrayDimsString(arrayDims)));
                m_needsYaxLibNamespaceAddition = true;
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
        private XElement MakeBaseElement(XElement insertionLocation, XName name, object value, out bool alreadyAdded)
        {
            alreadyAdded = false;
            if (value == null || ReflectionUtils.IsBasicType(value.GetType()))
            {
                if (value != null)
                    value = Convert.ToString(value, CultureInfo.InvariantCulture);

                return new XElement(name, value);
            }
            else if (ReflectionUtils.IsStringConvertibleIFormattable(value.GetType()))
            {
                object elementValue = value.GetType().InvokeMember("ToString", BindingFlags.InvokeMethod, null, value, new object[0]);
                return new XElement(name, elementValue);
            }
            else
            {
                YAXSerializer ser = new YAXSerializer(value.GetType(), m_exceptionPolicy, m_defaultExceptionType, m_serializationOption);
                ser.SetNamespaceToOverrideEmptyNamespace(name.Namespace);
                ser.SetBaseElement(insertionLocation);
                XElement elem = ser.SerializeBase(value, name);

                if (ser.m_needsYaxLibNamespaceAddition)
                    this.m_needsYaxLibNamespaceAddition = true;

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

            if (m_udtWrapper.HasCustomSerializer)
            {
                return InvokeCustomDeserializerFromElement(m_udtWrapper.CustomSerializerType, baseElement);
            }

            var realTypeAttr = baseElement.Attribute(s_yaxLibNamespaceUri + s_trueTypeAttrName);
            if (realTypeAttr != null)
            {
                Type theRealType = ReflectionUtils.GetTypeByName(realTypeAttr.Value);
                if (theRealType != null)
                {
                    m_type = theRealType;
                    m_udtWrapper = TypeWrappersPool.Pool.GetTypeWrapper(m_type, this);
                }
            }

            if (m_type.IsGenericType && m_type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            {
                return DeserializeKeyValuePair(baseElement);
            }

            if (KnownTypes.IsKnowType(m_type))
            {
                return KnownTypes.Deserialize(baseElement, m_type, TypeNamespace);
            }

            if ((m_udtWrapper.IsTreatedAsCollection || m_udtWrapper.IsTreatedAsDictionary) && !IsCraetedToDeserializeANonCollectionMember)
            {
                return DeserializeCollectionValue(m_type, baseElement, ReflectionUtils.GetTypeFriendlyName(m_type), null);
            }

            if (ReflectionUtils.IsBasicType(m_type))
            {
                return ReflectionUtils.ConvertBasicType(baseElement.Value, m_type);
            }

            object o = null;
            if (m_desObject != null)
                o = m_desObject;
            else
                o = m_type.InvokeMember(string.Empty, BindingFlags.CreateInstance, null, null, new object[0]);

            bool foundAnyOfMembers = false;
            foreach (var member in GetFieldsToBeSerialized())
            {
                if (!member.CanWrite)
                    continue;

                if (member.IsAttributedAsDontSerialize)
                    continue;

                // reset handled exceptions status
                m_exceptionOccurredDuringMemberDeserialization = false;

                string elemValue = string.Empty; // the element value gathered at the first phase
                XElement xelemValue = null; // the XElement instance gathered at the first phase
                XAttribute xattrValue = null; // the XAttribute instance gathered at the first phase


                //We get the default namespace for our element
                XNamespace xmlns = baseElement.Name.Namespace;

                //If our member overrides the namespace, then use that instead
                if (member.HasNamespace)
                    xmlns = member.Namespace;

                // first evaluate elemValue
                bool createdFakeElement = false;

                //We compute a namespace-safe location string
                var serializationLocation = XMLUtils.CreateExplicitNamespaceLocationString(baseElement, member.SerializationLocation);

                if (member.IsSerializedAsAttribute)
                {

                    // find the parent element from its location
                    XAttribute attr = XMLUtils.FindAttribute(baseElement, serializationLocation, xmlns + member.Alias);
                    if (attr == null) // if the parent element does not exist
                    {
                        // loook for an element with the same name AND a yaxlib:realtype attribute
                        XElement elem = XMLUtils.FindElement(baseElement, serializationLocation, xmlns + member.Alias);
                        if (elem != null && elem.Attribute(s_yaxLibNamespaceUri + s_trueTypeAttrName) != null)
                        {
                            elemValue = elem.Value;
                            xelemValue = elem;
                        }
                        else
                        {
                            this.OnExceptionOccurred(new YAXAttributeMissingException(
                                StringUtils.CombineLocationAndElementName(serializationLocation, member.Alias)),
                                (!member.MemberType.IsValueType && m_udtWrapper.IsNotAllowdNullObjectSerialization) ? YAXExceptionTypes.Ignore : member.TreatErrorsAs);
                        }
                    }
                    else
                    {
                        foundAnyOfMembers = true;
                        elemValue = attr.Value;
                        xattrValue = attr;
                    }
                }
                else if (member.IsSerializedAsValue)
                {
                    XElement elem = XMLUtils.FindLocation(baseElement, serializationLocation);
                    if (elem == null) // such element is not found
                    {
                        this.OnExceptionOccurred(new YAXElementMissingException(
                                serializationLocation),
                                (!member.MemberType.IsValueType && m_udtWrapper.IsNotAllowdNullObjectSerialization) ? YAXExceptionTypes.Ignore : member.TreatErrorsAs);
                    }
                    else
                    {
                        XText[] values = elem.Nodes().OfType<XText>().ToArray();
                        if(values.Length <= 0)
                        {
                            // loook for an element with the same name AND a yaxlib:realtype attribute
                            XElement innerelem = XMLUtils.FindElement(baseElement, serializationLocation, xmlns + member.Alias);
                            if (innerelem != null && innerelem.Attribute(s_yaxLibNamespaceUri + s_trueTypeAttrName) != null)
                            {
                                elemValue = innerelem.Value;
                                xelemValue = innerelem;
                            }
                            else
                            {
                                this.OnExceptionOccurred(new YAXElementValueMissingException(serializationLocation),
                                    (!member.MemberType.IsValueType && m_udtWrapper.IsNotAllowdNullObjectSerialization) ? YAXExceptionTypes.Ignore : member.TreatErrorsAs);
                            }
                        }
                        else
                        {
                            foundAnyOfMembers = true;
                            elemValue = values[0].Value;
                            values[0].Remove();
                        }
                    }
                }
                else // if member is serialized as an xml element
                {
                    bool canContinue = false;
                    XElement elem = XMLUtils.FindElement(baseElement, serializationLocation, xmlns + member.Alias);
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
                            XElement fakeElem = XMLUtils.CreateElement(baseElement, serializationLocation, xmlns + member.Alias);
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
                                StringUtils.CombineLocationAndElementName(serializationLocation, xmlns + member.Alias)),
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

                // Phase2: Now try to retrieve elemValue's value, based on values gathered in xelemValue, xattrValue, and elemValue
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
                                OnExceptionOccurred(
                                    new YAXDefaultValueCannotBeAssigned(member.Alias, member.DefaultValue),
                                    m_defaultExceptionType);
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
                                OnExceptionOccurred(
                                    new YAXDefaultValueCannotBeAssigned(member.Alias, member.DefaultValue),
                                    m_defaultExceptionType);
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
                else if (member.HasCustomSerializer || member.MemberTypeWrapper.HasCustomSerializer)
                {
                    Type deserType = member.HasCustomSerializer ?
                        member.CustomSerializerType :
                        member.MemberTypeWrapper.CustomSerializerType;

                    object desObj;
                    if (member.IsSerializedAsAttribute)
                    {
                        desObj = InvokeCustomDeserializerFromAttribute(deserType, xattrValue);
                    }
                    else if (member.IsSerializedAsElement)
                    {
                        desObj = InvokeCustomDeserializerFromElement(deserType, xelemValue);
                    }
                    else if (member.IsSerializedAsValue)
                    {
                        desObj = InvokeCustomDeserializerFromValue(deserType, elemValue);
                    }
                    else
                    {
                        throw new Exception("unknown situation");
                    }

                    try
                    {
                        member.SetValue(o, desObj);
                    }
                    catch
                    {
                        OnExceptionOccurred(new YAXPropertyCannotBeAssignedTo(member.Alias), m_defaultExceptionType);
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

            XName xElemName = GetXNameForMember(member, eachElementName);

            if (elem.Element(xElemName) != null) // if such an element exists
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
                        if (theElem != null && theElem.Attribute(s_yaxLibNamespaceUri + s_trueTypeAttrName) != null)
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
                XAttribute realTypeAttribute = xelemValue.Attribute(s_yaxLibNamespaceUri + s_trueTypeAttrName);
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
                    OnExceptionOccurred(new YAXDefaultValueCannotBeAssigned(member.Alias, member.DefaultValue), member.TreatErrorsAs);
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
                        this.OnExceptionOccurred(new YAXDefaultValueCannotBeAssigned(member.Alias, member.DefaultValue), m_defaultExceptionType);
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
                var ser = new YAXSerializer(memberType, m_exceptionPolicy, m_defaultExceptionType, m_serializationOption);
                ser.SetNamespaceToOverrideEmptyNamespace(
                    member.Namespace.
                        IfInvalidNext(this.TypeNamespace).
                        IfInvalidNext(null));

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
        private object DeserializeCollectionValue(Type colType, XElement xelemValue, XName memberAlias, YAXCollectionAttribute colAttrInstance)
        {
            var lst = new List<object>(); // this will hold the actual data items
            Type itemType = ReflectionUtils.GetCollectionItemType(colType);

            if (ReflectionUtils.IsBasicType(itemType) && colAttrInstance != null && colAttrInstance.SerializationType == YAXCollectionSerializationTypes.Serially)
            {
                // What if the collection was serialized serially
                char[] seps = colAttrInstance.SeparateBy.ToCharArray();

                // can white space characters be added to the separators?
                if (colAttrInstance == null || colAttrInstance.IsWhiteSpaceSeparator)
                {
                    seps = seps.Union(new [] { ' ', '\t', '\r', '\n' }).ToArray();
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
                        OnExceptionOccurred(new YAXBadlyFormedInput(memberAlias.ToString(), elemValue), m_defaultExceptionType);
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

                string eachElemName = null;// = ReflectionUtils.GetTypeFriendlyName(itemType);
                if (colAttrInstance != null && colAttrInstance.EachElementName != null)
                {
                    eachElemName = colAttrInstance.EachElementName;
                }

                XName xEachElemName = null;
                if(eachElemName != null)
                {
                    xEachElemName = GetXNameForMember(memberAlias, eachElemName);
                }

                var elemsToSearch = xEachElemName == null ? xelemValue.Elements() : xelemValue.Elements(xEachElemName);

                foreach (XElement childElem in elemsToSearch)
                {
                    Type curElementType = itemType;
                    bool curElementIsPrimitive = isPrimitive;

                    XAttribute realTypeAttribute = childElem.Attribute(s_yaxLibNamespaceUri + s_trueTypeAttrName);
                    if (realTypeAttribute != null)
                    {
                        Type theRealType = ReflectionUtils.GetTypeByName(realTypeAttribute.Value);
                        if (theRealType != null)
                        {
                            curElementType = theRealType;
                            curElementIsPrimitive = ReflectionUtils.IsBasicType(curElementType);
                        }
                    }

                    // TODO: check if curElementType is derived or is the same is itemType, for speed concerns perform this check only when elementName is null
                    if (eachElemName == null && (curElementType == typeof(object) || !ReflectionUtils.IsTypeEqualOrInheritedFromType(curElementType, itemType)))
                        continue;

                    if (curElementIsPrimitive)
                    {
                        try
                        {
                            lst.Add(ReflectionUtils.ConvertBasicType(childElem.Value, curElementType));
                        }
                        catch
                        {
                            this.OnExceptionOccurred(new YAXBadlyFormedInput(childElem.Name.ToString(), childElem.Value), m_defaultExceptionType);
                        }
                    }
                    else
                    {
                        var ser = new YAXSerializer(curElementType, m_exceptionPolicy, m_defaultExceptionType, m_serializationOption);
                        ser.SetNamespaceToOverrideEmptyNamespace(
                            memberAlias.Namespace.
                                IfInvalidNext(this.TypeNamespace).
                                IfInvalidNext(null));

                        lst.Add(ser.DeserializeBase(childElem));
                        m_parsingErrors.AddRange(ser.ParsingErrors);
                    }
                }
            } // end of else if 

            // Now what should I do with the filled list: lst
            Type dicKeyType, dicValueType;
            if (ReflectionUtils.IsArray(colType))
            {
                XAttribute dimsAttr = xelemValue.Attribute(s_yaxLibNamespaceUri + s_dimsAttrName);
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
                                new YAXCannotAddObjectToCollection(memberAlias.ToString(), lst[i]),
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
                                new YAXCannotAddObjectToCollection(memberAlias.ToString(), lst[i]),
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

                foreach (var lstItem in lst)
                {
                    object key = itemType.GetProperty("Key").GetValue(lstItem, null);
                    object value = itemType.GetProperty("Value").GetValue(lstItem, null);
                    try
                    {
                        colType.InvokeMember("Add", BindingFlags.InvokeMethod, null, dic, new object[] { key, value });
                    }
                    catch
                    {
                        this.OnExceptionOccurred(new YAXCannotAddObjectToCollection(memberAlias.ToString(), lstItem), this.m_defaultExceptionType);
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
                        this.OnExceptionOccurred(new YAXCannotAddObjectToCollection(memberAlias.ToString(), lstItem), this.m_defaultExceptionType);
                    }
                }

                return col;
            }
            else if (ReflectionUtils.IsTypeEqualOrInheritedFromType(colType, typeof(BitArray)))
            {
                var bArray = new bool[lst.Count];
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

                const string additionMethodName = "Push";

                for (int i = lst.Count - 1; i >= 0; i--) // the loop must be from end to front
                {
                    try
                    {
                        colType.InvokeMember(additionMethodName, BindingFlags.InvokeMethod, null, col, new object[] { lst[i] });
                    }
                    catch
                    {
                        this.OnExceptionOccurred(new YAXCannotAddObjectToCollection(memberAlias.ToString(), lst[i]), this.m_defaultExceptionType);
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
                        this.OnExceptionOccurred(new YAXCannotAddObjectToCollection(memberAlias.ToString(), lstItem), this.m_defaultExceptionType);
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
            object colObject;

            if (member.CollectionAttributeInstance != null && member.CollectionAttributeInstance.SerializationType == YAXCollectionSerializationTypes.Serially &&
                (member.IsSerializedAsAttribute || member.IsSerializedAsValue))
            {
                colObject = DeserializeCollectionValue(colType, new XElement("temp", elemValue), "temp", member.CollectionAttributeInstance);
            }
            else
            {
                XName memberAlias = GetXNameForMember(member);
                colObject = DeserializeCollectionValue(colType, xelemValue, memberAlias, member.CollectionAttributeInstance);
            }

            try
            {
                member.SetValue(o, colObject);
            }
            catch
            {
                OnExceptionOccurred(new YAXPropertyCannotBeAssignedTo(member.Alias), m_defaultExceptionType);
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
            var result = new int[dims.Length];

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
            XName keyAlias = GetXNameForMember(member, "Key");
            XName valueAlias = GetXNameForMember(member, "Value");

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

                if(member.DictionaryAttributeInstance.KeyName != null)
                    keyAlias = GetXNameForMember(member, member.DictionaryAttributeInstance.KeyName);
                if(member.DictionaryAttributeInstance.ValueName != null)
                    valueAlias = GetXNameForMember(member, member.DictionaryAttributeInstance.ValueName);
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
                            keySer = new YAXSerializer(keyType, m_exceptionPolicy, m_defaultExceptionType, m_serializationOption);
                            keySer.SetNamespaceToOverrideEmptyNamespace(
                                keyAlias.Namespace.
                                    IfInvalidNext(this.TypeNamespace).
                                    IfInvalidNext(null));
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
                            valueSer = new YAXSerializer(valueType, m_exceptionPolicy, m_defaultExceptionType, m_serializationOption);
                            valueSer.SetNamespaceToOverrideEmptyNamespace(
                                valueAlias.Namespace.
                                    IfInvalidNext(this.TypeNamespace).
                                    IfInvalidNext(null));
                        }

                        value = valueSer.DeserializeBase(childElem.Element(valueAlias));
                        m_parsingErrors.AddRange(valueSer.ParsingErrors);
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
        private static bool VerifyDictionaryPairElements(ref Type keyType, ref bool isKeyAttrib, XName keyAlias, XElement childElem)
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
                    XAttribute realTypeAttr = elem.Attribute(s_yaxLibNamespaceUri + s_trueTypeAttrName);
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

                    XAttribute realTypeAttr = elem.Attribute(s_yaxLibNamespaceUri + s_trueTypeAttrName);
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
            Type[] genArgs = m_type.GetGenericArguments();
            Type keyType = genArgs[0];
            Type valueType = genArgs[1];

            XName xnameKey = GetXNameForMember("Key");
            XName xnameValue = GetXNameForMember("Value");

            object keyValue, valueValue;
            if (ReflectionUtils.IsBasicType(keyType))
            {
                try
                {
                    keyValue = ReflectionUtils.ConvertBasicType(
                        baseElement.Element(xnameKey).Value, keyType);
                }
                catch (NullReferenceException)
                {
                    keyValue = null;
                }
            }
            else if (ReflectionUtils.IsStringConvertibleIFormattable(keyType))
            {
                keyValue = keyType.InvokeMember(string.Empty, 
                    BindingFlags.CreateInstance, 
                    null, null, 
                    new object[] { baseElement.Element(xnameKey).Value });
            }
            else if (ReflectionUtils.IsCollectionType(keyType))
            {
                keyValue = DeserializeCollectionValue(keyType, 
                    baseElement.Element(xnameKey), xnameKey, null);
            }
            else
            {
                var ser = new YAXSerializer(keyType, m_exceptionPolicy, 
                    m_defaultExceptionType, m_serializationOption);
                ser.SetNamespaceToOverrideEmptyNamespace(xnameKey.Namespace.IfInvalidNext(null));

                keyValue = ser.DeserializeBase(baseElement.Element(xnameKey));
                m_parsingErrors.AddRange(ser.ParsingErrors);
            }

            if (ReflectionUtils.IsBasicType(valueType))
            {
                try
                {
                    valueValue = ReflectionUtils.ConvertBasicType(
                        baseElement.Element(xnameValue).Value, valueType);
                }
                catch (NullReferenceException)
                {
                    valueValue = null;
                }
            }
            else if (ReflectionUtils.IsStringConvertibleIFormattable(valueType))
            {
                valueValue = valueType.InvokeMember(string.Empty, BindingFlags.CreateInstance, 
                    null, null, new object[] { baseElement.Element(xnameValue).Value });
            }
            else if (ReflectionUtils.IsCollectionType(valueType))
            {
                valueValue = DeserializeCollectionValue(valueType, 
                    baseElement.Element(xnameValue), xnameValue, null);
            }
            else
            {
                var ser = new YAXSerializer(valueType, m_exceptionPolicy, 
                    m_defaultExceptionType, m_serializationOption);
                ser.SetNamespaceToOverrideEmptyNamespace(xnameValue.Namespace.IfInvalidNext(null));
                valueValue = ser.DeserializeBase(baseElement.Element(xnameValue));
                m_parsingErrors.AddRange(ser.ParsingErrors);
            }

            object pair = m_type.InvokeMember(string.Empty, 
                System.Reflection.BindingFlags.CreateInstance, 
                null, null, new object[] { keyValue, valueValue });

            return pair;
        }

        #endregion

        #region Utilities

        private static object InvokeCustomDeserializerFromElement(Type customDeserType, XElement elemToDeser)
        {
            object customDeserializer = customDeserType.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, new object[0]);
            return customDeserType.InvokeMember("DeserializeFromElement", BindingFlags.InvokeMethod, null, customDeserializer, new object[] { elemToDeser });
        }

        private static object InvokeCustomDeserializerFromAttribute(Type customDeserType, XAttribute attrToDeser)
        {
            object customDeserializer = customDeserType.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, new object[0]);
            return customDeserType.InvokeMember("DeserializeFromAttribute", BindingFlags.InvokeMethod, null, customDeserializer, new object[] { attrToDeser });
        }

        private static object InvokeCustomDeserializerFromValue(Type customDeserType, string valueToDeser)
        {
            object customDeserializer = customDeserType.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, new object[0]);
            return customDeserType.InvokeMember("DeserializeFromValue", BindingFlags.InvokeMethod, null, customDeserializer, new object[] { valueToDeser });
        }

        private static void InvokeCustomSerializerToElement(Type customSerType, object objToSerialize, XElement elemToFill)
        {
            object customSerializer = customSerType.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, new object[0]);
            customSerType.InvokeMember("SerializeToElement", BindingFlags.InvokeMethod, null, customSerializer, new object[] { objToSerialize, elemToFill });
        }

        private static void InvokeCustomSerializerToAttribute(Type customSerType, object objToSerialize, XAttribute attrToFill)
        {
            object customSerializer = customSerType.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, new object[0]);
            customSerType.InvokeMember("SerializeToAttribute", BindingFlags.InvokeMethod, null, customSerializer, new object[] { objToSerialize, attrToFill });
        }

        private static string InvokeCustomSerializerToValue(Type customSerType, object objToSerialize)
        {
            object customSerializer = customSerType.InvokeMember(string.Empty, System.Reflection.BindingFlags.CreateInstance, null, null, new object[0]);
            return (string) customSerType.InvokeMember("SerializeToValue", BindingFlags.InvokeMethod, null, customSerializer, new object[] { objToSerialize });
        }

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

        private XName GetXNameForMember(MemberWrapper member, string memberName = null)
        {
            string alias = memberName == null ? member.Alias : memberName;
            XName xname;
            if (member.HasNamespace)
                xname = XName.Get(alias, member.Namespace.NamespaceName);
            else if (this.HasTypeNamespace)
                xname = XName.Get(alias, this.TypeNamespace.NamespaceName);
            else
                xname = XName.Get(alias);
            return xname;
        }

        private XName GetXNameForMember(string memberName)
        {
            XName xname;
            if (this.HasTypeNamespace)
                xname = XName.Get(memberName, this.TypeNamespace.NamespaceName);
            else
                xname = XName.Get(memberName);
            return xname;
        }

        /// <summary>
        /// Creates an instance of <c>XName</c> for the given <c>memberName</c>, 
        /// getting the namespace from <c>otherMember</c> or inheriting from the type-namespace.
        /// </summary>
        /// <param name="otherMember">An instance of <c>XName</c> to copy namespace info from.
        /// Note: only the namespace info for this parameter is used, nothing else.</param>
        /// <param name="memberName">The member name for which an instance of <c>XName</c> is going
        /// to be created.</param>
        /// <returns></returns>
        private XName GetXNameForMember(XName otherMember, string memberName)
        {
            XName xname;

            if (!String.IsNullOrEmpty(otherMember.NamespaceName))
                xname = XName.Get(memberName, otherMember.NamespaceName);
            else if (this.HasTypeNamespace)
                xname = XName.Get(memberName, this.TypeNamespace.NamespaceName);
            else
                xname = XName.Get(memberName);

            return xname;
        }

        private XName GetXNameForMember(UdtWrapper memberType, string memberName = null)
        {
            string alias = memberName == null ? memberType.Alias : memberName;
            XName xname;
            if (memberType.HasNamespace)
                xname = XName.Get(alias, memberType.Namespace.NamespaceName);
            else if (this.HasTypeNamespace)
                xname = XName.Get(alias, this.TypeNamespace.NamespaceName);
            else
                xname = XName.Get(alias);
            return xname;
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
