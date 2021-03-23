// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Xml.Linq;

namespace YAXLib
{
    /// <summary>
    /// Settings to influence the process of serialization or de-serialization of <see cref="YAXSerializer"/>s.
    /// </summary>
    public class SerializerOptions
    {
        private const int DefaultMaxRecursion = 300;

        /// <summary>
        ///     Creates a new instances of settings to influence the process of
        ///     serialization or de-serialization of <see cref="YAXSerializer"/>s.
        /// </summary>
        public SerializerOptions()
        {
            MaxRecursion = DefaultMaxRecursion;

            // Initialization with compatibility to v2.x:

            ExceptionBehavior = YAXExceptionTypes.Warning;
            ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowErrorsOnly;
            SerializationOptions = YAXSerializationOptions.DontSerializeNullObjects;
            AttributeName = new YAXAttributeName {Dimensions = "dims", RealType = "realtype"};
            Namespace = new YAXNameSpace {Prefix = "yaxlib", Uri = XNamespace.Get("http://www.sinairv.com/yaxlib/")};
        }

        /// <summary>
        /// Definitions for special attribute names.
        /// </summary>
        public class YAXAttributeName
        {
            /// <summary>
            ///     The attribute name used to de-serialize meta-data for multi-dimensional arrays.
            /// </summary>
            public string Dimensions { get; set; }
            
            /// <summary>
            ///     The attribute name used to de-serialize meta-data for real types of objects serialized through
            ///     a reference to their base class or interface.
            /// </summary>
            public string RealType { get; set; }
        }

        /// <summary>
        /// XML Namespace definitions for the <see cref="YAXSerializer"/>.
        /// </summary>
        public class YAXNameSpace
        {
            /// <summary>
            ///     The URI address which holds the xmlns:yaxlib definition.
            /// </summary>
            public XNamespace Uri { get; set; }
            
            /// <summary>
            ///     The prefix used for the xml namespace.
            /// </summary>
            public string Prefix { get; set; }
        }
        
        /// <summary>
        ///     Gets or sets the kinds of <see cref="YAXExceptionHandlingPolicies"/>.
        /// </summary>
        public YAXExceptionHandlingPolicies ExceptionHandlingPolicies { get; set; }
        
        /// <summary>
        ///     Gets or sets the behavior when <see cref="Exception"/>s are thrown.
        /// </summary>
        public YAXExceptionTypes ExceptionBehavior { get; set; }
        
        /// <summary>
        ///     Gets or sets the <see cref="YAXSerializationOptions"/> for serialization and/or de-serialization.
        /// </summary>
        public YAXSerializationOptions SerializationOptions { get; set; }

        /// <summary>
        ///     Definitions for special attribute names.
        /// </summary>
        public YAXAttributeName AttributeName { get; set; }
        
        /// <summary>
        ///     XML Namespace definitions for the <see cref="YAXSerializer"/>.
        /// </summary>
        public YAXNameSpace Namespace { get; set; }
        
        /// <summary>
        ///     Specifies the maximum serialization depth (defaults to 300).
        ///     This roughly equals the maximum element depth of the resulting XML.
        ///     0 means unlimited.
        ///     1 means an empty XML tag with no content.
        /// </summary>
        public int MaxRecursion { get; set; }
    }
}
