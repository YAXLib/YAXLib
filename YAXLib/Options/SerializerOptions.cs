// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.Xml.Linq;
using YAXLib.Enums;

namespace YAXLib.Options;

/// <summary>
/// Settings to influence the process of serialization or de-serialization of <see cref="YAXSerializer" />s.
/// </summary>
public class SerializerOptions
{
    private const int DefaultMaxRecursion = 50;

    /// <summary>
    /// Creates a new instances of settings to influence the process of
    /// serialization or de-serialization of <see cref="YAXSerializer" />s.
    /// </summary>
    public SerializerOptions()
    {
        MaxRecursion = DefaultMaxRecursion;

        // Initialization with compatibility to v2.x:

        Culture = CultureInfo.InvariantCulture;
        ExceptionBehavior = YAXExceptionTypes.Error;
        ExceptionHandlingPolicies = YAXExceptionHandlingPolicies.ThrowWarningsAndErrors;
        SerializationOptions = YAXSerializationOptions.SerializeNullObjects;
        AttributeName = new YAXAttributeName { Dimensions = "dims", RealType = "realtype" };
        Namespace = new YAXNameSpace { Prefix = "yaxlib", Uri = XNamespace.Get("http://www.sinairv.com/yaxlib/") };
    }

    /// <summary>
    /// Gets or sets the kinds of <see cref="YAXExceptionHandlingPolicies" />.
    /// </summary>
    public YAXExceptionHandlingPolicies ExceptionHandlingPolicies { get; set; }

    /// <summary>
    /// Gets or sets the behavior when <see cref="Exception" />s are thrown.
    /// </summary>
    public YAXExceptionTypes ExceptionBehavior { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="YAXSerializationOptions" /> for serialization and/or de-serialization.
    /// </summary>
    public YAXSerializationOptions SerializationOptions { get; set; }

    /// <summary>
    /// Definitions for special attribute names.
    /// </summary>
    public YAXAttributeName AttributeName { get; set; }

    /// <summary>
    /// XML Namespace definitions for the <see cref="YAXSerializer" />.
    /// </summary>
    public YAXNameSpace Namespace { get; set; }

    /// <summary>
    /// Specifies the maximum serialization depth.
    /// This roughly equals the maximum element depth of the resulting XML.
    /// 1 means an empty XML tag with no content.
    /// For unlimited depth set <see cref="int.MaxValue" />.
    /// </summary>
    public int MaxRecursion { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="CultureInfo" /> for serialization and de-serialization.
    /// Default is <see cref="CultureInfo.CurrentCulture" />.
    /// </summary>
    public CultureInfo Culture { get; set; }
}