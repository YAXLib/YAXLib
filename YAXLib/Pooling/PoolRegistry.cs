// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Concurrent;

namespace YAXLib.Pooling;

/// <summary>
/// Registry for all object pools.
/// </summary>
internal static class PoolRegistry
{
    public static readonly ConcurrentDictionary<Type, object> Items = new();

    /// <summary>
    /// Adds a pool to the registry.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="poolFunc"></param>
    /// <returns>The instance of pool which was added.</returns>
    public static T GetOrAdd<T>(Func<T> poolFunc) where T : class
    {
        return (T) Items.GetOrAdd(typeof(T), _ => poolFunc());
    }

    /// <summary>
    /// Removes a pool from the registry.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pool"></param>
    public static void Remove<T>(T pool) where T : class
    {
        Items.TryRemove(pool.GetType(), out _);
    }

    /// <summary>
    /// Gets the pool of type <typeparamref name="T" /> from the registry, or <see langword="null" /> if not found.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>The pool of type <typeparamref name="T" /> from the registry, or <see langword="null" /> if not found.</returns>
    public static T? Get<T>() where T : class
    {
        return (T?) Items[typeof(T)];
    }
}
