// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Reflection;

namespace YAXLib;

public interface IYaxMemberInfo
{
    string Name { get; }
    MemberTypes MemberType { get; }
    bool IsPublic { get; }
    Type Type { get; }
    Attribute[] GetCustomAttributes(Type attrType, bool inherit);
    Attribute[] GetCustomAttributes(bool inherit);
}
