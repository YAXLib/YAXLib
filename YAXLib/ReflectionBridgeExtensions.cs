// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Reflection;

namespace YAXLib
{
    internal static class ReflectionBridgeExtensions
    {
        public static object InvokeMethod(this Type type, string methodName, object target, object[] arg)
        {
            return type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, target, arg);
        }
    }
}