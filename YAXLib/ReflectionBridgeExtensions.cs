using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

#if FXCORE
using Microsoft.Extensions.DependencyModel;
#endif

namespace YAXLib
{
#if FXCORE
    /// <summary>
    /// Implementation of AppDomain for .NetCore
    /// </summary>
    public class AppDomain
    {
        /*
        MIT License

        Copyright (C) 2017 by axuno gGmbH (https://github.com/axuno and http://www.axuno.net)

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
        internal static AppDomain CurrentDomain { get; private set; }

        static AppDomain()
        {
            CurrentDomain = new AppDomain();
        }

        public Assembly[] GetAssemblies()
        {
            var assemblies = new List<Assembly>();

            var runtimeId = Microsoft.DotNet.PlatformAbstractions.RuntimeEnvironment.GetRuntimeIdentifier();
            var assemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(runtimeId);

            foreach (var assemblyName in assemblyNames)
            {
                var assembly = Assembly.Load(assemblyName);
                assemblies.Add(assembly);
            }
            return assemblies.ToArray();
        }
    }
#endif

    /// <summary>
    /// Provides extensions as a bridge for the differences 
    /// between .Net Framework "Type" and .Net Core "TypeInfo".
    /// </summary>
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
        public static Assembly GetAssembly(this Type type)
        {
#if FXCORE
            return type.GetTypeInfo().Assembly;
#else
            return type.Assembly;
#endif
        }

        public static bool IsAbstract(this Type type)
        {
#if FXCORE
            return type.GetTypeInfo().IsAbstract;
#else
            return type.IsAbstract;
#endif
        }

        public static bool IsEnum(this Type type)
        {
#if FXCORE
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        public static bool IsClass(this Type type)
        {
#if FXCORE
            return type.GetTypeInfo().IsClass;
#else
            return type.IsClass;
#endif
        }

        public static bool IsPrimitive(this Type type)
        {
#if FXCORE
            return type.GetTypeInfo().IsPrimitive;
#else
            return type.IsPrimitive;
#endif
        }

        public static bool IsGenericType(this Type type)
        {
#if FXCORE
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

        public static bool IsGenericTypeDefinition(this Type type)
        {
#if FXCORE
            return type.GetTypeInfo().IsGenericTypeDefinition;
#else
            return type.IsGenericTypeDefinition;
#endif
        }

        public static bool IsInterface(this Type type)
        {
#if FXCORE
            return type.GetTypeInfo().IsInterface;
#else
            return type.IsInterface;
#endif
        }

        public static Type BaseType(this Type type)
        {
#if FXCORE
            return type.GetTypeInfo().BaseType;
#else
            return type.BaseType;
#endif
        }

        public static bool IsValueType(this Type type)
        {
#if FXCORE
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }

        public static PropertyInfo GetProperty(this Type type, string name, Type[] types)
        {
#if FXCORE
            return type.GetTypeInfo().GetProperty(name, types);
#else
            return type.GetProperty(name, types);
#endif
        }

        public static object InvokeMethod<T>(this Type type, string methodName, object target, T value)
        {
#if FXCORE
            MethodInfo method = type.GetTypeInfo().GetMethod(methodName, new Type[] {value?.GetType()});
            return method.Invoke(target, new object[] { value });
#else
            return type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, target, new object[] {value});
#endif
        }

        public static object InvokeMethod(this Type type, string methodName, object target, object[] arg)
        {
#if FXCORE
            Type[] argTypes = null;
            dynamic x = null;
            if (arg != null)
            {
                argTypes = new Type[arg.Length];
                for (var i = 0; i < argTypes.Length; i++)
                {
                    argTypes[i] = arg[i]?.GetType();
                }
            }
            else
            {
                argTypes = Type.EmptyTypes;
            }

            if (argTypes.All(t => t != null))
            {
                MethodInfo method = type.GetTypeInfo().GetMethod(methodName, argTypes);
                return method.Invoke(target, arg);
            }

            // one of the arguments is null
            var potentialMethods = type.GetTypeInfo().GetMethods().Where(m => m.Name == methodName).ToArray();
            var ex = new Exception(nameof(InvokeMethod));
            //TODO: FXCORE Trial and error for method invocation is not the most elegant way...
            foreach (var method in potentialMethods)
            {
                try
                {
                    return method.Invoke(target, arg);
                }
                catch (Exception e)
                {
                    ex = e;
                }
            }
            
            throw new TargetInvocationException(ex);
#else
            return type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, target, arg);
#endif
        }

#if FXCORE

        public static Attribute[] GetCustomAttributes(this Type type, bool inherit)
        {
            return type.GetTypeInfo().GetCustomAttributes(inherit).ToArray();
        }
#endif
    }
}
