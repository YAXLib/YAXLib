// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using YAXLib;

namespace YAXLibTests;

public class GeneralToStringProvider
{
    public static string GeneralToString(object? o)
    {
        return GeneralToString(o, 0);
    }

    private static string CollectionToString(object? collectionInstance, string? propName, int layer)
    {
        //object collectionInstance = prop.GetValue(o, null);
        var sb = new StringBuilder();
        if (collectionInstance == null)
        {
            if (string.IsNullOrEmpty(propName))
                sb.AppendLayerFormatLine(layer, "[null]");
            else
                sb.AppendLayerFormatLine(layer, "{0}: [null]", propName);
        }
        else
        {
            if (!string.IsNullOrEmpty(propName))
            {
                var strSize = "";
                if (collectionInstance.GetType().IsArray)
                {
                    var ar = collectionInstance as Array;
                    var rank = ar?.Rank;
                    var ars = new StringBuilder();
                    for (var i = 0; i < rank; i++)
                    {
                        if (i != 0)
                            ars.Append('*');
                        ars.Append(ar?.GetLength(i));
                    }

                    strSize = $"[size: {ars}]";
                }

                sb.AppendLayerFormatLine(layer, "{0}: {1}", propName, strSize);
            }

            foreach (var item in (collectionInstance as IEnumerable)!)
                if (item != null && IsBasicType(item.GetType()))
                {
                    sb.AppendLayerFormatLine(layer + 1, "[{0}]", item.ToString() ?? string.Empty);
                }
                else
                {
                    sb.AppendLayerFormatLine(layer + 1, "[");
                    sb.Append(GeneralToString(item, layer + 2));
                    sb.AppendLayerFormatLine(layer + 1, "]");
                }
        }

        return sb.ToString();
    }

    private static string NonGenericDictionaryToString(object? dicInstance, string? propName, int layer)
    {
        var sb = new StringBuilder();
        if (dicInstance == null)
        {
            if (string.IsNullOrEmpty(propName))
                sb.AppendLayerFormatLine(layer, "[null]");
            else
                sb.AppendLayerFormatLine(layer, "{0}: [null]", propName);
        }
        else
        {
            if (!string.IsNullOrEmpty(propName))
                sb.AppendLayerFormatLine(layer, "{0}:", propName);

            foreach (var pair in (dicInstance as IEnumerable)!)
                if (pair == null)
                {
                    sb.AppendLayerFormatLine(layer + 1, "[null]");
                }
                else
                {
                    sb.AppendLayerFormatLine(layer + 1, "[");

                    var objKey = pair.GetType().GetProperty("Key")?.GetValue(pair, null);
                    var objValue = pair.GetType().GetProperty("Value")?.GetValue(pair, null);

                    if (objKey == null || IsBasicType(objKey.GetType()))
                    {
                        sb.AppendLayerFormatLine(layer + 1, "Key: {0}",
                            (objKey == null ? "[null]" : objKey.ToString())!
                        );
                    }
                    else
                    {
                        sb.AppendLayerFormatLine(layer + 1, "Key: ");
                        sb.AppendLayerFormatLine(layer + 2, "[");
                        sb.Append(GeneralToString(objKey, layer + 3));
                        sb.AppendLayerFormatLine(layer + 2, "]");
                    }

                    if (objValue == null || IsBasicType(objValue.GetType()))
                    {
                        sb.AppendLayerFormatLine(layer + 1, "Value: {0}",
                            (objValue == null ? "[null]" : objValue.ToString())!
                        );
                    }
                    else
                    {
                        sb.AppendLayerFormatLine(layer + 1, "Value: ");
                        sb.AppendLayerFormatLine(layer + 2, "[");
                        sb.Append(GeneralToString(objValue, layer + 3));
                        sb.AppendLayerFormatLine(layer + 2, "]");
                    }

                    sb.AppendLayerFormatLine(layer + 1, "]");
                }
        }

        return sb.ToString();
    }


    private static string DictionaryToString(object? dicInstance, string? propName, int layer)
    {
        var sb = new StringBuilder();
        if (dicInstance == null)
        {
            if (string.IsNullOrEmpty(propName))
                sb.AppendLayerFormatLine(layer, "[null]");
            else
                sb.AppendLayerFormatLine(layer, "{0}: [null]", propName);
        }
        else
        {
            if (!string.IsNullOrEmpty(propName))
                sb.AppendLayerFormatLine(layer, "{0}:", propName);

            IsDictionary(dicInstance.GetType(), out var keyType, out var valueType);
            if (IsBasicType(keyType) && IsBasicType(valueType))
            {
                foreach (var pair in (dicInstance as IEnumerable)!)
                    if (pair == null)
                    {
                        sb.AppendLayerFormatLine(layer + 1, "[null]");
                    }
                    else
                    {
                        var objKey = pair.GetType().GetProperty("Key")?.GetValue(pair, null);
                        var objValue = pair.GetType().GetProperty("Value")?.GetValue(pair, null);
                        sb.AppendLayerFormatLine(layer + 1, "[{0} -> {1}]",
                            (objKey == null ? "[null]" : objKey.ToString())!,
                            (objValue == null ? "[null]" : objValue.ToString())!
                        );
                    }
            }
            else
            {
                foreach (var pair in (dicInstance as IEnumerable)!)
                    if (pair == null)
                    {
                        sb.AppendLayerFormatLine(layer + 1, "[null]");
                    }
                    else
                    {
                        sb.AppendLayerFormatLine(layer + 1, "[");

                        var objKey = pair.GetType().GetProperty("Key")?.GetValue(pair, null);
                        var objValue = pair.GetType().GetProperty("Value")?.GetValue(pair, null);

                        if (IsBasicType(keyType) || objKey == null)
                        {
                            sb.AppendLayerFormatLine(layer + 1, "Key: {0}",
                                (objKey == null ? "[null]" : objKey.ToString())!
                            );
                        }
                        else
                        {
                            sb.AppendLayerFormatLine(layer + 1, "Key: ");
                            sb.AppendLayerFormatLine(layer + 2, "[");
                            sb.Append(GeneralToString(objKey, layer + 3));
                            sb.AppendLayerFormatLine(layer + 2, "]");
                        }

                        if (IsBasicType(valueType) || objValue == null)
                        {
                            sb.AppendLayerFormatLine(layer + 1, "Value: {0}",
                                (objValue == null ? "[null]" : objValue.ToString())!
                            );
                        }
                        else
                        {
                            sb.AppendLayerFormatLine(layer + 1, "Value: ");
                            sb.AppendLayerFormatLine(layer + 2, "[");
                            sb.Append(GeneralToString(objValue, layer + 3));
                            sb.AppendLayerFormatLine(layer + 2, "]");
                        }

                        sb.AppendLayerFormatLine(layer + 1, "]");
                    }
            }
        }

        return sb.ToString();
    }

    private static string GeneralToString(object? o, int layer)
    {
        var sb = new StringBuilder();
        if (o == null)
        {
            sb.AppendLayerFormatLine(layer, "[null]");
        }
        else if (IsBasicType(o.GetType()))
        {
            sb.AppendLayerFormatLine(layer, o.ToString()!);
        }
        else if (IsDictionary(o.GetType()))
        {
            sb.Append(DictionaryToString(o, null, layer));
        }
        else if (IsCollection(o.GetType()))
        {
            sb.Append(CollectionToString(o, null, layer));
        }
        else if (IsIFormattable(o.GetType()))
        {
            sb.AppendLayerFormatLine(layer, o.ToString() ?? string.Empty);
        }
        else
        {
            foreach (var prop in o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!prop.CanRead)
                    continue;

                if (prop.GetIndexParameters().Length > 0) // do not print indexers
                    continue;

                if (ReflectionUtils.IsTypeEqualOrInheritedFromType(prop.PropertyType, typeof(Delegate)))
                    continue; // do not print delegates

                var propType = prop.PropertyType;
                if (IsBasicType(propType))
                {
                    sb.AppendLayerFormatLine(layer, "{0}: {1}", prop.Name, GetBasicPropertyValue(o, prop));
                }
                else if (IsDictionary(propType))
                {
                    sb.Append(DictionaryToString(prop.GetValue(o, null), prop.Name, layer));
                }
                else if (IsCollection(propType))
                {
                    sb.Append(CollectionToString(prop.GetValue(o, null), prop.Name, layer));
                }
                else if (IsNonGenericIDictionary(propType))
                {
                    sb.Append(NonGenericDictionaryToString(prop.GetValue(o, null), prop.Name, layer));
                }
                else if (IsNonGenericIEnumerable(propType))
                {
                    sb.Append(CollectionToString(prop.GetValue(o, null), prop.Name, layer));
                }
                else
                {
                    var propValue = prop.GetValue(o, null);
                    if (propValue == null)
                    {
                        sb.AppendLayerFormatLine(layer, "{0}: [null]", prop.Name);
                    }
                    else
                    {
                        sb.AppendLayerFormatLine(layer, "{0}:", prop.Name);

                        sb.AppendLayerFormatLine(layer, "[");
                        sb.Append(GeneralToString(propValue, layer + 1));
                        sb.AppendLayerFormatLine(layer, "]");
                    }
                }
            }
        }

        return sb.ToString();
    }

    private static bool IsDictionary(Type type)
    {
        if (type.IsGenericType)
            type = type.GetGenericTypeDefinition();

        if (type == typeof(Dictionary<,>))
            return true;

        return false;
    }

    /// <summary>
    /// Determines whether the specified type is a generic dictionary.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="keyType">Type of the key.</param>
    /// <param name="valueType">Type of the value.</param>
    /// <returns>
    /// <c>true</c> if the specified type is dictionary; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsDictionary(Type type, out Type keyType, out Type valueType)
    {
        keyType = typeof(object);
        valueType = typeof(object);

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

    private static bool IsNonGenericIDictionary(Type type)
    {
        if (type == typeof(IDictionary))
            return true;

        foreach (var interfaceType in type.GetInterfaces())
            if (interfaceType == typeof(IDictionary))
                return true;

        return false;
    }

    private static bool IsNonGenericIEnumerable(Type type)
    {
        if (type == typeof(IEnumerable))
            return true;

        foreach (var interfaceType in type.GetInterfaces())
            if (interfaceType == typeof(IEnumerable))
                return true;

        return false;
    }


    private static bool IsCollection(Type type)
    {
        if (type == typeof(string))
            return false;

        if (IsArray(type))
            return true;

        if (type.IsGenericType)
            type = type.GetGenericTypeDefinition();

        if (type == typeof(List<>) || type == typeof(HashSet<>) || type == typeof(IEnumerable<>))
            return true;

        Type elemType;
        if (IsIEnumerableExceptArray(type, out elemType)) return true;

        return false;
    }

    private static bool IsIFormattable(Type type)
    {
        // is IFormattable
        // accept parameterless ToString
        // accept ctor of string
        foreach (var interfaceType in type.GetInterfaces())
            if (interfaceType == typeof(IFormattable))
                if (!HasSuitableProperties(type))
                    if (null != type.GetConstructor(new[] { typeof(string) }))
                        if (null != type.GetMethod("ToString", Type.EmptyTypes) &&
                            null != type.GetMethod("ToString", new[] { typeof(string) }))
                            return true;

        return false;
    }

    private static bool HasSuitableProperties(Type type)
    {
        var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var pi in props)
            if (pi.CanRead && pi.CanWrite)
            {
                var getPi = pi.GetGetMethod(false);
                var setPi = pi.GetSetMethod(false);
                if (setPi != null && getPi != null)
                    return true;
            }

        return false;
    }

    /// <summary>
    /// Gets the type of the items of a collection type.
    /// </summary>
    /// <param name="type">The type of the collection.</param>
    /// <returns>the type of the items of a collection type.</returns>
    private static Type GetCollectionItemType(Type type)
    {
        var itemType = typeof(object);

        if (type.IsInterface && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            itemType = type.GetGenericArguments()[0];
        else if (type.IsInterface && type == typeof(IEnumerable))
            itemType = typeof(object);
        else
            foreach (var interfaceType in type.GetInterfaces())
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    itemType = interfaceType.GetGenericArguments()[0];

        return itemType;
    }


    private static string GetBasicPropertyValue(object o, PropertyInfo prop)
    {
        var value = prop.GetValue(o, null);
        return (value == null ? "[null]" : value.ToString())!;
    }

    /// <summary>
    /// Determines whether the specified type is basic type. A basic type is one that can be wholly expressed
    /// as an XML attribute. All primitive data types and type <c>string</c> and <c>DataTime</c> are basic.
    /// </summary>
    /// <param name="t">The type</param>
    private static bool IsBasicType(Type t)
    {
        if (t == typeof(string) || t.IsPrimitive || t.IsEnum || t == typeof(DateTime) || t == typeof(decimal))
            return true;
        return false;
    }

    /// <summary>
    /// Determines whether the specified type is array.
    /// </summary>
    /// <param name="t">The type</param>
    /// <returns>
    /// <c>true</c> if the specified type is array; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsArray(Type t)
    {
        return t.BaseType == typeof(Array);
    }

    /// <summary>
    /// Determines whether the specified type has implemented or is an <c>IEnumerable</c> or <c>IEnumerable&lt;&gt;</c>.
    /// This method does not detect Arrays.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="seqType">Type of the sequence items.</param>
    /// <returns>
    ///     <value><c>true</c> if the specified type is enumerable; otherwise, <c>false</c>.</value>
    /// </returns>
    public static bool IsIEnumerableExceptArray(Type type, out Type seqType)
    {
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
}

public static class StringBuilderExtensions
{
    public static StringBuilder AppendLayerFormatLine(this StringBuilder sb, int layer, string format,
        params object?[] args)
    {
        return AppendLayerFormat(sb, layer, format + Environment.NewLine, args);
    }

    public static StringBuilder AppendLayerFormat(this StringBuilder sb, int layer, string format,
        params object?[] args)
    {
        var strToAppend = string.Format(format, args);
        return sb.AppendFormat("{0}{1}", GetLayerPrefix(layer), strToAppend);
    }

    private static string GetLayerPrefix(int layer)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < layer; ++i)
            sb.Append("   ");

        return sb.ToString();
    }
}