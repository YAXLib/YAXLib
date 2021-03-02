using System;
using System.Reflection;


namespace YAXLib
{
    internal static class ReflectionBridgeExtensions
    {
        /*
        MIT License

        Copyright (c) 2016 to 2099 Stef Heyenrath
        Sourcecode: https://github.com/StefH/ReflectionBridge

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.
        */

        /*
        Modified by axuno gGmbH (https://github.com/axuno and http://www.axuno.net) for YAXLib
        */

        public static object InvokeMethod<T>(this Type type, string methodName, object target, T value)
        {
            return type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, target, new object[] {value});
        }

        public static object InvokeMethod(this Type type, string methodName, object target, object[] arg)
        {
            return type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, target, arg);
        }
    }
}
