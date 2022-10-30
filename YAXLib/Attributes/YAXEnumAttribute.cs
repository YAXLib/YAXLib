// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib.Attributes;

/// <summary>
/// Specifies an alias for an enum member.
/// This attribute is applicable to enum members.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class YAXEnumAttribute : YAXBaseAttribute
{
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="YAXEnumAttribute" /> class.
    /// </summary>
    /// <param name="alias">The alias.</param>
    public YAXEnumAttribute(string alias)
    {
        Alias = alias.Trim();
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the alias for the enum member.
    /// </summary>
    /// <value>The alias for the enum member.</value>
    public string Alias { get; }

    #endregion
}