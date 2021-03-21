﻿// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib
{
    /// <summary>
    ///     Controls the serialization of generic Dictionary instances.
    ///     This attribute is applicable to fields and properties, and
    ///     classes derived from the <c>Dictionary</c> base class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class |
                    AttributeTargets.Struct)]
    public class YAXDictionaryAttribute : YAXBaseAttribute
    {
        private YAXNodeTypes _serializeKeyAs = YAXNodeTypes.Element;
        private YAXNodeTypes _serializeValueAs = YAXNodeTypes.Element;

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXDictionaryAttribute" /> class.
        /// </summary>
        public YAXDictionaryAttribute()
        {
            KeyName = null;
            ValueName = null;
            EachPairName = null;
            KeyFormatString = null;
            ValueFormatString = null;
        }

        /// <summary>
        ///     Gets or sets the alias for the key part of the dicitonary.
        /// </summary>
        /// <value></value>
        public string KeyName { get; set; }

        /// <summary>
        ///     Gets or sets alias for the value part of the dicitonary.
        /// </summary>
        /// <value></value>
        public string ValueName { get; set; }

        /// <summary>
        ///     Gets or sets alias for the element containing the Key-Value pair.
        /// </summary>
        /// <value></value>
        public string EachPairName { get; set; }

        /// <summary>
        ///     Gets or sets the node type according to which the key part of the dictionary is serialized.
        /// </summary>
        /// <value></value>
        public YAXNodeTypes SerializeKeyAs
        {
            get { return _serializeKeyAs; }

            set
            {
                _serializeKeyAs = value;
                CheckIntegrity();
            }
        }

        /// <summary>
        ///     Gets or sets the node type according to which the value part of the dictionary is serialized.
        /// </summary>
        /// <value></value>
        public YAXNodeTypes SerializeValueAs
        {
            get { return _serializeValueAs; }

            set
            {
                _serializeValueAs = value;
                CheckIntegrity();
            }
        }

        /// <summary>
        ///     Gets or sets the key format string.
        /// </summary>
        /// <value></value>
        public string KeyFormatString { get; set; }

        /// <summary>
        ///     Gets or sets the value format string.
        /// </summary>
        /// <value></value>
        public string ValueFormatString { get; set; }

        private void CheckIntegrity()
        {
            if (_serializeKeyAs == _serializeValueAs && _serializeValueAs == YAXNodeTypes.Content)
                throw new Exception("Key and Value cannot both be serialized as Content at the same time.");
        }
    }
}