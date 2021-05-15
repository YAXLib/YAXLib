// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace YAXLib
{
    /// <summary>
    ///     A utility class for reflection related stuff
    /// </summary>
    internal static class ReflectionUtils
    {
        /// <summary>
        ///     Determines whether the specified type is basic type. A basic type is one that can be wholly expressed
        ///     as an XML attribute. All primitive data types and type <c>string</c> and <c>DataTime</c> are basic.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>
        ///     <value><c>true</c> if the specified type is a basic type; otherwise, <c>false</c>.</value>
        /// </returns>
        public static bool IsBasicType(Type t)
        {
            if (t == typeof(string) || t.IsPrimitive || t.IsEnum || t == typeof(DateTime) || t == typeof(decimal) ||
                t == typeof(Guid))
                return true;

            Type nullableValueType;
            if (IsNullable(t, out nullableValueType))
                return IsBasicType(nullableValueType);
            return false;
        }

        /// <summary>
        ///     Determines whether the specified type is array.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <param name="elementType">Type of the containing element.</param>
        /// <returns>
        ///     <value><c>true</c> if the specified type is array; otherwise, <c>false</c>.</value>
        /// </returns>
        public static bool IsArray(Type type, out Type elementType)
        {
            if (type.IsArray)
            {
                elementType = type.GetElementType();
                return true;
            }

            if (type == typeof(Array)) // i.e., a direct ref to System.Array
            {
                elementType = typeof(object);
                return true;
            }

            elementType = typeof(object);
            return false;
        }

        /// <summary>
        ///     Determines whether the specified type is array.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///     <c>true</c> if the specified type is array; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsArray(Type type)
        {
            Type elementType;
            return IsArray(type, out elementType);
        }

        /// <summary>
        ///     Gets the array dimensions.
        /// </summary>
        /// <param name="ar">The array to return its dimensions.</param>
        /// <returns>the specified array's dimensions</returns>
        public static int[] GetArrayDimensions(object ar)
        {
            int[] dims = null;
            if (IsArray(ar.GetType()))
            {
                var arObj = ar as Array;
                dims = new int[arObj.Rank];
                for (var i = 0; i < dims.Length; i++)
                    dims[i] = arObj.GetLength(i);
            }

            return dims;
        }

        /// <summary>
        ///     Gets the friendly name for the type. Recommended for generic types.
        /// </summary>
        /// <param name="type">The type to get its friendly name</param>
        /// <returns>The friendly name for the type</returns>
        public static string GetTypeFriendlyName(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            var name = type.Name;
            if (type.IsGenericType)
            {
                var backqIndex = name.IndexOf('`');
                if (backqIndex == 0)
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Bad type name: {0}",
                        name));
                if (backqIndex > 0) name = name.Substring(0, backqIndex);

                name += "Of";

                foreach (var genType in type.GetGenericArguments()) name += GetTypeFriendlyName(genType);
            }
            else if (type.IsArray)
            {
                var t = type.GetElementType();
                name = string.Format(CultureInfo.InvariantCulture, "Array{0}Of{1}", type.GetArrayRank(),
                    GetTypeFriendlyName(t));
            }

            return name;
        }

        /// <summary>
        ///     Determines whether the type specified contains generic parameters or not.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///     <value><c>true</c> if the type contains generic parameters; otherwise,<c>false</c>.</value>
        /// </returns>
        public static bool TypeContainsGenericParameters(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");

            if (type.IsGenericType)
                foreach (var genType in type.GetGenericArguments())
                    if (genType.IsGenericParameter)
                        return true;
                    else if (TypeContainsGenericParameters(genType)) return true;

            return false;
        }

        /// <summary>
        ///     Determines whether the specified type is a collection type, i.e., it implements IEnumerable.
        ///     Although System.String is derived from IEnumerable, it is considered as an exception.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///     <value><c>true</c> if the specified type is a collection type; otherwise, <c>false</c>.</value>
        /// </returns>
        public static bool IsCollectionType(Type type)
        {
            if (type == typeof(string)) return false;

            return IsIEnumerable(type);
        }

        /// <summary>
        ///     Determines whether the specified type has implemented or is an <c>IEnumerable</c> or <c>IEnumerable&lt;&gt;</c>
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///     <value><c>true</c> if the specified type is enumerable; otherwise, <c>false</c>.</value>
        /// </returns>
        public static bool IsIEnumerable(Type type)
        {
            Type seqType;
            return IsIEnumerable(type, out seqType);
        }

        public static bool IsDerivedFromGenericInterfaceType(Type givenType, Type genericInterfaceType,
            out Type genericType)
        {
            genericType = null;
            if ((givenType.IsClass || givenType.IsValueType) && !givenType.IsAbstract)
                foreach (var interfaceType in givenType.GetInterfaces())
                    if (interfaceType.IsGenericType &&
                        interfaceType.GetGenericTypeDefinition() == genericInterfaceType)
                    {
                        var genArgs = interfaceType.GetGenericArguments();
                        if (genArgs.Length != 1)
                            return false;

                        genericType = genArgs[0];
                        return true;
                    }

            return false;
        }


        /// <summary>
        ///     Determines whether the specified type has implemented or is an <c>IEnumerable</c> or <c>IEnumerable&lt;&gt;</c> .
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="seqType">Type of the sequence items.</param>
        /// <returns>
        ///     <value><c>true</c> if the specified type is enumerable; otherwise, <c>false</c>.</value>
        /// </returns>
        public static bool IsIEnumerable(Type type, out Type seqType)
        {
            // detect arrays early
            if (IsArray(type, out seqType))
                return true;

            seqType = typeof(object);
            if (type == typeof(IEnumerable))
                return true;

            var isNongenericEnumerable = false;

            if (type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                seqType = type.GetGenericArguments()[0];
                return true;
            }

            foreach (var interfaceType in type.GetInterfaces())
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    var genArgs = interfaceType.GetGenericArguments();
                    seqType = genArgs[0];
                    return true;
                }
                else if (interfaceType == typeof(IEnumerable))
                {
                    isNongenericEnumerable = true;
                }

            // the second case is a direct reference to IEnumerable
            if (isNongenericEnumerable || type == typeof(IEnumerable))
            {
                seqType = typeof(object);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Gets the type of the items of a collection type.
        /// </summary>
        /// <param name="type">The type of the collection.</param>
        /// <returns>The type of the items of a collection type.</returns>
        public static Type GetCollectionItemType(Type type)
        {
            var itemType = typeof(object);
            if (IsIEnumerable(type, out itemType))
                return itemType;
            throw new Exception("The specified type must be a collection");
        }

        /// <summary>
        ///     Determines whether the specified type has implemented <c>IList</c>.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///     <value><c>true</c> if the specified type has implemented <c>IList</c>; otherwise, <c>false</c>.</value>
        /// </returns>
        public static bool IsIList(Type type)
        {
            // a direct reference to the interface itself is also OK.
            if (type.IsInterface && type.GetGenericTypeDefinition() == typeof(IList<>)) return true;

            foreach (var interfaceType in type.GetInterfaces())
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(IList<>))
                    return true;

            return false;
        }

        /// <summary>
        ///     Determines whether the specified type has implemented the <c>ICollection</c> interface.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="itemType">Type of the member items.</param>
        /// <returns>
        ///     <value><c>true</c> if the specified type has implemented the <c>ICollection</c> interface; otherwise, <c>false</c>.</value>
        /// </returns>
        public static bool IsICollection(Type type, out Type itemType)
        {
            itemType = typeof(object);

            // a direct reference to the interface itself is also OK.
            if (type.IsInterface && type.GetGenericTypeDefinition() == typeof(ICollection<>))
            {
                itemType = type.GetGenericArguments()[0];
                return true;
            }

            foreach (var interfaceType in type.GetInterfaces())
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    itemType = interfaceType.GetGenericArguments()[0];
                    return true;
                }

            return false;
        }

        /// <summary>
        ///     Determines whether the specified type is a generic dictionary.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="keyType">Type of the key.</param>
        /// <param name="valueType">Type of the value.</param>
        /// <returns>
        ///     <value><c>true</c> if the specified type has implemented the IDictionary interface; otherwise, <c>false</c>.</value>
        /// </returns>
        public static bool IsIDictionary(Type type, out Type keyType, out Type valueType)
        {
            keyType = typeof(object);
            valueType = typeof(object);

            // a direct reference to the interface itself is also OK.
            if (type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                var genArgs = type.GetGenericArguments();
                keyType = genArgs[0];
                valueType = genArgs[1];
                return true;
            }

            foreach (var interfaceType in type.GetInterfaces())
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    var genArgs = interfaceType.GetGenericArguments();
                    keyType = genArgs[0];
                    valueType = genArgs[1];
                    return true;
                }

            return false;
        }

        /// <summary>
        ///     Determines whether the specified type is a generic dictionary.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///     <value><c>true</c> if the specified type is dictionary; otherwise, <c>false</c>.</value>
        /// </returns>
        public static bool IsIDictionary(Type type)
        {
            Type keyType, valueType;
            return IsIDictionary(type, out keyType, out valueType);
        }

        /// <summary>
        ///     Determines whether the specified type is a non generic IDictionary, e.g., a Hashtable.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///     <c>true</c> if the specified type is a non generic IDictionary; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNonGenericIDictionary(Type type)
        {
            // a direct reference to the interface itself is also OK.
            if (type == typeof(IDictionary)) return true;

            foreach (var interfaceType in type.GetInterfaces())
                if (interfaceType == typeof(IDictionary))
                    return true;

            return false;
        }

        /// <summary>
        ///     Determines whether the specified type is equal to this type,
        ///     or is a nullable of this type, or this type is a nullable of
        ///     the other type.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool EqualsOrIsNullableOf(this Type self, Type other)
        {
            if (self == other)
                return true;

            Type selfBaseType;
            Type otherBaseType;
            if (!IsNullable(self, out selfBaseType))
                selfBaseType = self;
            if (!IsNullable(other, out otherBaseType))
                otherBaseType = other;

            return selfBaseType == otherBaseType;
        }

        /// <summary>
        ///     Determines whether the specified type is equal or inherited from another specified type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="baseType">
        ///     Another type that the specified type is checked whether it is equal or
        ///     has been driven from.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified type is equal or inherited from another specified type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTypeEqualOrInheritedFromType(Type type, Type baseType)
        {
            if (type == baseType)
                return true;

            var isTypeGenDef = type.IsGenericTypeDefinition;
            var isBaseGenDef = baseType.IsGenericTypeDefinition;
            Type[] typeGenArgs = null;
            Type[] baseGenArgs = null;

            if (type.IsGenericType)
            {
                if (isBaseGenDef)
                {
                    if (!isTypeGenDef)
                    {
                        type = type.GetGenericTypeDefinition();
                        isTypeGenDef = true;
                    }
                }
                else
                {
                    typeGenArgs = type.GetGenericArguments();
                }
            }

            if (baseType.IsGenericType)
            {
                if (isTypeGenDef)
                {
                    if (!isBaseGenDef)
                    {
                        baseType = baseType.GetGenericTypeDefinition();
                        isBaseGenDef = true;
                    }
                }
                else
                {
                    baseGenArgs = baseType.GetGenericArguments();
                }
            }

            if (type == baseType)
                return true;

            if (typeGenArgs != null && baseGenArgs != null)
            {
                if (typeGenArgs.Length != baseGenArgs.Length)
                    return false;

                for (var i = 0; i < typeGenArgs.Length; i++)
                    // TODO: check if I should call this method for type args recersively
                    if (typeGenArgs[i] != baseGenArgs[i])
                        return false;
            }

            if (baseType.IsInterface)
            {
                foreach (var iface in type.GetInterfaces())
                    if (iface.Name == baseType.Name)
                        return true;
                return false;
            }

            var curBaseType = type.BaseType;
            while (curBaseType != null)
            {
                if (curBaseType.Name == baseType.Name)
                    return true;

                curBaseType = curBaseType.BaseType;
            }

            return false;
        }

        /// <summary>
        ///     Determines whether the specified type is nullable.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="valueType">The value type of the corresponding nullable type.</param>
        /// <returns>
        ///     <c>true</c> if the specified type is nullable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullable(Type type, out Type valueType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                valueType = type.GetGenericArguments()[0];
                return true;
            }

            valueType = null;
            return false;
        }

        /// <summary>
        ///     Determines whether the specified type is nullable.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///     <c>true</c> if the specified type is nullable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullable(Type type)
        {
            Type valueType;
            return IsNullable(type, out valueType);
        }

        /// <summary>
        ///     Determines whether the specified type implements <c>IFormattable</c>
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///     <c>true</c> if the specified type implements <c>IFormattable</c>; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIFormattable(Type type)
        {
            // a direct reference to the interface itself is also OK.
            if (type == typeof(IFormattable)) return true;

            foreach (var interfaceType in type.GetInterfaces())
                if (interfaceType == typeof(IFormattable))
                    return true;

            return false;
        }

        /// <summary>
        ///     Determines whether the type provides the functionality
        ///     to format the value of an object into a string representation.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///     <value><c>true</c> if the specified type impliments the <c>IFormattable</c> interface; otherwise, <c>false</c>.</value>
        /// </returns>
        public static bool IsStringConvertibleIFormattable(Type type)
        {
            // is IFormattable
            // accept parameterless ToString
            // accept ctor of string
            if (IsIFormattable(type) && !HasOneReadWriteProperty(type))
                if (null != type.GetConstructor(new[] {typeof(string)}))
                    if (null != type.GetMethod("ToString", new Type[0]) &&
                        null != type.GetMethod("ToString", new[] {typeof(string)}))
                        return true;

            return false;
        }

        /// <summary>
        ///     Checks to see if the specified type has readable and writable properties.
        /// </summary>
        /// <param name="type">The type to check for.</param>
        /// <returns>
        ///     <value><c>true</c> if the specified type has readable and writable properties; otherwise, <c>false</c>.</value>
        /// </returns>
        public static bool HasOneReadWriteProperty(Type type)
        {
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var pi in props)
                if (pi.CanRead && pi.CanWrite)
                {
                    var getPi = pi.GetGetMethod(false);
                    var setPi = pi.GetSetMethod(false);
                    if (setPi != null && getPi != null) return true;
                }

            return false;
        }

        /// <summary>
        ///     Tries to format the specified object using the format string provided.
        ///     If the formatting operation is not applicable, the source object is returned intact.
        ///     Note: The type of the returned object will be 'System.String' if formatting succeeds.
        /// </summary>
        /// <param name="src">The source object.</param>
        /// <param name="format">The format string.</param>
        /// <returns><code>System.String</code> if the format is successful; otherwise, the original object</returns>
        public static object TryFormatObject(object src, string format)
        {
            if (format == null || src == null) return src;

            object formattedObject = null;

            try
            {
                formattedObject = src.GetType().InvokeMethod("ToString", src, new object[] {format});
                //formattedObject = src.GetType().InvokeMember("ToString", BindingFlags.InvokeMethod, null, src, new object[] { format });
            }
            catch
            {
                return src;
                //throw new YAXInvalidFormatProvided(src.GetType(), format);
                //this.OnExceptionOccurred(new YAXInvalidFormatProvided(src.GetType(), format), this.m_defaultExceptionType);
            }

            return formattedObject ?? src;
        }

        /// <summary>
        ///     Converts the specified object from a basic type to another type as specified.
        ///     It is meant by basic types, primitive data types, strings, and enums.
        /// </summary>
        /// <param name="value">The object to be converted.</param>
        /// <param name="dstType">the destination type of conversion.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> to use for culture-specific value formats.</param>
        /// <returns>the converted object</returns>
        public static object ConvertBasicType(object value, Type dstType, CultureInfo culture)
        {
            object convertedObj = null;
            if (dstType.IsEnum)
            {
                var typeWrapper = TypeWrappersPool.Pool.GetTypeWrapper(dstType, null);
                convertedObj = typeWrapper.EnumWrapper.ParseAlias(value.ToString());
            }
            else if (dstType == typeof(DateTime))
            {
                convertedObj = StringUtils.ParseDateTimeTimeZoneSafe(value.ToString(), culture);
            }
            else if (dstType == typeof(decimal))
            {
                // to fix the asymmetry of used locales for this type between serialization and deserialization
                convertedObj = Convert.ChangeType(value, dstType, culture);
            }
            else if (dstType == typeof(bool))
            {
                var strValue = value.ToString().Trim().ToLower();
                if (strValue == "false" || strValue == "no" || strValue == "0")
                {
                    convertedObj = false;
                }
                else if (strValue == "true" || strValue == "yes" || strValue == "1")
                {
                    convertedObj = true;
                }
                else
                {
                    var boolIntValue = 0;
                    if (int.TryParse(strValue, out boolIntValue))
                        convertedObj = boolIntValue != 0;
                    else
                        throw new Exception("The specified value is not recognized as boolean: " + strValue);
                }
            }
            else if (dstType == typeof(Guid))
            {
                return new Guid(value.ToString());
            }
            else
            {
                Type nullableType;
                if (IsNullable(dstType, out nullableType))
                {
                    if (value == null || value.ToString() == string.Empty)
                        return null;
                    return ConvertBasicType(value, nullableType, culture);
                }

                convertedObj = Convert.ChangeType(value, dstType, culture);
            }

            return convertedObj;
        }

        /// <summary>
        ///     Searches all loaded assemblies to find a type with a special name.
        /// </summary>
        /// <remarks>
        /// Types from System.Private.CoreLib (NETSTANDARD, NET5.0) the corresponding type from mscorlib (NETFRAMEWORK)
        /// will be returned and vice versa, depending on the framework the executing assembly is compiled for.
        /// </remarks>
        /// <param name="name">The <see cref="Type.AssemblyQualifiedName"/> of the type to find.</param>
        /// <returns><see cref="Type"/> found using the specified name</returns>
        public static Type GetTypeByName(string name)
        {
            var pattern =
                RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework")
                    // Forward compatibility:
                    // if we get a yaxlib:realtype which is NETSTANDARD or NET5.0 System.Private.CoreLib, replace it with its equivalent
                    ? @"\,\s+(System\.Private\.CoreLib)\,\s+Version\=\d+(\.\d+)*\,\s+Culture=\b\w+\b\,\s+PublicKeyToken\=\b\w+\b"
                    // Backward compatibility:
                    // if we get a yaxlib:realtype which is .Net Framework 2.x/3.x/4.x mscorlib, replace it with its equivalent
                    : @"\,\s+(mscorlib)\,\s+Version\=\d+(\.\d+)*\,\s+Culture=\b\w+\b\,\s+PublicKeyToken\=\b\w+\b";
            
            var execAppFxName  = System.Text.RegularExpressions.Regex.Replace(name, pattern, name.GetType().Assembly.FullName);
            
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // first search the 1st assembly (i.e. the mscorlib), then start from the last assembly backward, 
            // the last assemblies are user defined ones
            for (var i = assemblies.Length; i > 0; i--)
            {
                var curAssembly = i == assemblies.Length ? assemblies[0] : assemblies[i];

                try
                {
                    var type = curAssembly.GetType(execAppFxName, false, true);
                    if (type != null)
                        return type;
                }
                catch
                {
                    // ignored
                }
            }

            return null;
        }

        /// <summary>
        ///     Determines whether the specified property is public.
        /// </summary>
        /// <param name="pi">The property.</param>
        /// <returns>
        ///     <c>true</c> if the specified property is public; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPublicProperty(PropertyInfo pi)
        {
            foreach (var m in pi.GetAccessors())
                if (m.IsPublic)
                    return true;

            return false;
        }

        /// <summary>
        ///     Test whether the <see cref="MemberInfo"/> parameter is part of a .NET module.
        /// </summary>
        /// <remarks>
        ///     Might require modifications when supporting future versions of .NET.
        /// </remarks>
        /// <param name="memberInfo"></param>
        /// <returns>Returns <see langword="true"/>, if the <see cref="MemberInfo"/> parameter is part of a .NET module, else <see langword="false"/>.</returns>
        /// <summary>
        ///     Test whether the <see cref="MemberInfo"/> parameter is part of a .NET module.
        /// </summary>
        /// <remarks>
        ///     Might require modifications when supporting future versions of .NET.
        /// </remarks>
        /// <returns>Returns <see langword="true"/>, if the <see cref="MemberInfo"/> parameter is part of a .NET module, else <see langword="false"/>.</returns>
        public static bool IsPartOfNetFx(MemberInfo memberInfo)
        {
            var moduleName = memberInfo.Module.Name;
#if NETSTANDARD
            return moduleName.StartsWith("System.", StringComparison.OrdinalIgnoreCase) ||
                   moduleName.StartsWith("mscorlib.", StringComparison.OrdinalIgnoreCase) ||
                   moduleName.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase);
#else
            return moduleName.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase)
                   || moduleName.Equals("System.dll", StringComparison.OrdinalIgnoreCase)
                   || moduleName.Equals("System.Core.dll", StringComparison.OrdinalIgnoreCase);
#endif
        }

        public static bool IsInstantiableCollection(Type colType)
        {
            return colType.GetConstructor(Type.EmptyTypes) != null;
        }

        public static T InvokeGetProperty<T>(object srcObj, string propertyName)
        {
            return (T) srcObj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)
                ?.GetValue(srcObj, null);
        }

        public static T InvokeIntIndexer<T>(object srcObj, string propertyName, int index)
        {
            var pi = srcObj.GetType().GetProperty("Item", new[] {typeof(int)});
            return (T) pi.GetValue(srcObj, new object[] {index});
        }

        public static object InvokeStaticMethod(Type type, string methodName, params object[] args)
        {
            var argTypes = args.Select(x => x.GetType()).ToArray();
            var method = type.GetMethod(methodName, argTypes);
            var result = method?.Invoke(null, args);
            return result;
        }

        public static object InvokeMethod(object srcObj, string methodName, params object[] args)
        {
            var argTypes = args.Select(x => x.GetType()).ToArray();
            var method = srcObj.GetType().GetMethod(methodName, argTypes);
            var result = method?.Invoke(srcObj, args);
            return result;
        }

        public static bool IsBaseClassOrSubclassOf(Type subType, string baseName)
        {
            if (baseName == null || subType == null) return false;
            var baseType = Type.GetType(baseName);
            return baseType != null && (subType.FullName == baseName || subType.IsSubclassOf(baseType));
        }
    }
}