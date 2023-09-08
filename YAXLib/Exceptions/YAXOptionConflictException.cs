// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;

namespace YAXLib.Exceptions;
/// <summary>
/// Raised When the YAXSerializationOptions conflictes
/// </summary>
public class YAXOptionConflictException : YAXException
{
    string _msg;
    /// <summary>
    /// Initializes a new instance of the <see cref="YAXOptionConflictException" /> class.
    /// </summary>
    /// <param name="message"></param>
    public YAXOptionConflictException(string message) : base(message)
    {
        _msg = message;
    }
    /// <summary>
    /// Gets a message that describes the current exception.
    /// </summary>
    public override string Message => _msg;
}
