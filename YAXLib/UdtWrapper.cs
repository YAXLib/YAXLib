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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YAXLib
{
    /// <summary>
    /// a wrapper class around user-defined types, for quick acess to their YAXLib related attributes
    /// </summary>
    internal class UdtWrapper
    {
        #region Private Fields

        /// <summary>
        /// the underlying type for this instance of <c>TypeWrapper</c>
        /// </summary>
        private Type m_udtType = typeof(object);

        /// <summary>
        /// boolean value indicating whether this instance is a wrapper around a collection type
        /// </summary>
        private bool m_isTypeCollection = false;

        /// <summary>
        /// boolean value indicating whether this instance is a wrapper around a dictionary type
        /// </summary>
        private bool m_isTypeDictionary = false;

        /// <summary>
        /// reference to an instance of <c>EnumWrapper</c> in case that the current instance is an enum.
        /// </summary>
        private EnumWrapper m_enumWrapper = null;

        /// <summary>
        /// value indicating whether the serialization options has been explicitly adjusted
        /// using attributes for the class
        /// </summary>
        private bool m_isSerializationOptionSetByAttribute = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UdtWrapper"/> class.
        /// </summary>
        /// <param name="udtType">The underlying type to create the wrapper around.</param>
        /// <param name="callerSerializer">reference to the serializer 
        /// instance which is building this instance.</param>
        public UdtWrapper(Type udtType, YAXSerializer callerSerializer)
        {
            this.m_udtType = udtType;
            this.m_isTypeCollection = ReflectionUtils.IsCollectionType(m_udtType);
            this.m_isTypeDictionary = ReflectionUtils.IsIDictionary(m_udtType);

            this.Alias = ReflectionUtils.GetTypeFriendlyName(m_udtType);
            this.Comment = null;
            this.FieldsToSerialize = YAXSerializationFields.PublicPropertiesOnly;
            this.IsAttributedAsNotCollection = false;

            SetYAXSerializerOptions(callerSerializer);

            foreach (var attr in m_udtType.GetCustomAttributes(false))
            {
                if (attr is YAXBaseAttribute)
                    ProcessYAXAttribute(attr);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the alias of the type.
        /// </summary>
        /// <value>The alias of the type.</value>
        public string Alias { get; private set; }

        /// <summary>
        /// Gets an array of comments for the underlying type.
        /// </summary>
        /// <value>The array of comments for the underlying type.</value>
        public string[] Comment { get; private set; }

        /// <summary>
        /// Gets the fields to be serialized.
        /// </summary>
        /// <value>The fields to be serialized.</value>
        public YAXSerializationFields FieldsToSerialize { get; private set; }

        /// <summary>
        /// Gets the serialization options.
        /// </summary>
        /// <value>The serialization options.</value>
        public YAXSerializationOptions SerializationOption { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is attributed as not collection.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is attributed as not collection; otherwise, <c>false</c>.
        /// </value>
        public bool IsAttributedAsNotCollection { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance has comment.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has comment; otherwise, <c>false</c>.
        /// </value>
        public bool HasComment
        {
            get
            {
                return this.Comment != null && this.Comment.Length > 0;
            }
        }

        /// <summary>
        /// Gets the underlying type corresponding to this wrapper.
        /// </summary>
        /// <value>The underlying type corresponding to this wrapper.</value>
        public Type UnderlyingType
        {
            get
            {
                return m_udtType;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance wraps around an enum.
        /// </summary>
        /// <value><c>true</c> if this instance wraps around an enum; otherwise, <c>false</c>.</value>
        public bool IsEnum
        {
            get
            {
                return m_udtType.IsEnum;
            }
        }

        /// <summary>
        /// Gets the enum wrapper, provided that this instance wraps around an enum.
        /// </summary>
        /// <value>The enum wrapper, provided that this instance wraps around an enum.</value>
        public EnumWrapper EnumWrapper
        {
            get
            {
                if (IsEnum)
                {
                    if (m_enumWrapper == null)
                        m_enumWrapper = new EnumWrapper(m_udtType);

                    return m_enumWrapper;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Determines whether serialization of null objects is not allowd.
        /// </summary>
        /// <returns>
        /// <c>true</c> if serialization of null objects is not allowd; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotAllowdNullObjectSerialization
        {
            get
            {
                return this.SerializationOption == YAXSerializationOptions.DontSerializeNullObjects;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance wraps around a collection type.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance wraps around a collection type; otherwise, <c>false</c>.
        /// </value>
        public bool IsCollectionType
        {
            get
            {
                return m_isTypeCollection;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance wraps around a dictionary type.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance wraps around a dictionary type; otherwise, <c>false</c>.
        /// </value>
        public bool IsDictionaryType
        {
            get
            {
                return m_isTypeDictionary;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is treated as collection.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is treated as collection; otherwise, <c>false</c>.
        /// </value>
        public bool IsTreatedAsCollection
        {
            get
            {
                return (!IsAttributedAsNotCollection && IsCollectionType);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is treated as dictionary.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is treated as dictionary; otherwise, <c>false</c>.
        /// </value>
        public bool IsTreatedAsDictionary
        {
            get
            {
                return !IsAttributedAsNotCollection && IsDictionaryType;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the serializer options.
        /// </summary>
        /// <param name="caller">The caller serializer.</param>
        public void SetYAXSerializerOptions(YAXSerializer caller)
        {
            if (!m_isSerializationOptionSetByAttribute)
            {
                if (caller != null)
                {
                    this.SerializationOption = caller.SerializationOption;
                }
                else
                {
                    this.SerializationOption = YAXSerializationOptions.SerializeNullObjects;
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return m_udtType.ToString();
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (obj is UdtWrapper)
            {
                UdtWrapper other = obj as UdtWrapper;
                return this.m_udtType == other.m_udtType;
            }

            return false;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return m_udtType.GetHashCode();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Processes the specified attribute.
        /// </summary>
        /// <param name="attr">The attribute to process.</param>
        private void ProcessYAXAttribute(object attr)
        {
            if (attr is YAXCommentAttribute)
            {
                string comment = (attr as YAXCommentAttribute).Comment;
                if(!String.IsNullOrEmpty(comment))
                {
                    string[] comments = comment.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    for(int i = 0; i < comments.Length; i++)
                    {
                        comments[i] = String.Format(" {0} ", comments[i].Trim());
                    }

                    this.Comment = comments;
                }
            }
            else if (attr is YAXSerializableTypeAttribute)
            {
                YAXSerializableTypeAttribute theAttr = attr as YAXSerializableTypeAttribute;
                this.FieldsToSerialize = theAttr.FieldsToSerialize;
                if (theAttr.IsSerializationOptionSet())
                {
                    this.SerializationOption = theAttr.Options;
                    m_isSerializationOptionSetByAttribute = true;
                }
            }
            else if (attr is YAXSerializeAsAttribute)
            {
                this.Alias = (attr as YAXSerializeAsAttribute).SerializeAs;
            }
            else if (attr is YAXNotCollectionAttribute)
            {
                if(!ReflectionUtils.IsArray(m_udtType))
                    this.IsAttributedAsNotCollection = true;
            }
            else
            {
                throw new Exception("Attribute not applicable to types!");
            }
        }

        #endregion
    }
}
