// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YAXLib.Pooling;
using YAXLib.Pooling.ObjectPools;
using YAXLib.Pooling.YAXLibPools;

#nullable enable

namespace YAXLibTests.TestHelpers;

public class PoolingHelpers
{
    internal static List<(Type? Type, IPoolCounters? Counters)> GetAllPoolCounters()
    {
        var l = new List<(Type? Type, IPoolCounters? Counters)>();

        foreach (dynamic p in PoolRegistry.Items.Values)
        {
            if (p.Pool is IPoolCounters counters)
                l.Add((p.GetType(), counters));
        }

        return l;
    }

    internal static List<(Type? Type, IPoolCounters? Counters)> GetAllPoolsCleared()
    {
        var l = new List<(Type? Type, IPoolCounters? Counters)>();

        // get pools
        var poolTypes = GetSubclassesOf(typeof(PoolBase<>).Assembly, typeof(PoolBase<>));

        foreach (var poolType in poolTypes.Concat(PoolRegistry.Items.Keys).Distinct())
        {
            dynamic? instance = poolType.GetProperty("Instance")?.GetValue(null, null);
            instance?.Pool.Clear();
            l.Add((poolType, instance?.Pool as IPoolCounters));
        }

        return l;
    }

    /// <summary>
    /// Gets all subclasses for a generic type definition.
    /// </summary>
    /// <example>
    /// var types = typeof(ObjectPool&lt;&gt;).Assembly.GetSubclassesOf(typeof(ObjectPool&lt;&gt;));
    /// </example>
    /// <param name="assembly"></param>
    /// <param name="genericTypeDefinition"></param>
    /// <returns>The subclasses for a generic type definition.</returns>
    public static IEnumerable<Type> GetSubclassesOf(
        Assembly assembly,
        Type genericTypeDefinition)
    {
        // Scan all base types of the type
        IEnumerable<Type> GetAllAscendants(Type t)
        {
            var current = t;

            while (current is { BaseType: { } } && current.BaseType != typeof(object))
            {
                yield return current.BaseType;
                current = current.BaseType;
            }
        }

        if (assembly == null)
            throw new ArgumentNullException(nameof(assembly));

        if (genericTypeDefinition == null)
            throw new ArgumentNullException(nameof(genericTypeDefinition));

        if (!genericTypeDefinition.IsGenericTypeDefinition)
            throw new ArgumentException(
                """Specified type is not a valid generic type definition.""",
                nameof(genericTypeDefinition));

        return Enumerable.Where<Type>(assembly.GetTypes(), t => GetAllAscendants(t).Any(d =>
            d.IsGenericType &&
            d.GetGenericTypeDefinition() == genericTypeDefinition));
    }

    /// <summary>
    /// Tests whether the <paramref name="typeToTest" /> is a subclass of <paramref name="genericTypeDefinition" />
    /// in <paramref name="assembly" />.
    /// </summary>
    /// <param name="typeToTest"></param>
    /// <param name="assembly"></param>
    /// <param name="genericTypeDefinition"></param>
    /// <returns><see langword="true" />, if the <paramref name="typeToTest" /> is a subclass.</returns>
    public static bool IsSubclassOf(Type typeToTest, Assembly assembly, Type genericTypeDefinition)
    {
        return GetSubclassesOf(assembly, genericTypeDefinition).Any(t => t == typeToTest);
    }
}
