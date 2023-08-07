// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace YAXLib.Pooling.SpecializedPools;

/// <summary>
/// Generic object pool implementation for <see cref="IDictionary{TKey,TValue}" />s.
/// </summary>
internal sealed class
    DictionaryPool<TKey, TValue> : CollectionPool<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    /// <summary>
    /// CTOR.
    /// </summary>
    /// <remarks>
    /// <see cref="SpecializedPoolBase{T}.Policy" /> must be set before initializing the pool
    /// </remarks>
    private DictionaryPool()
    {
        // Use initialization of base class
    }

    /// <summary>
    /// Gets a singleton instance of the pool.
    /// </summary>
    public static new DictionaryPool<TKey, TValue> Instance => PoolRegistry.GetOrAdd(() => new DictionaryPool<TKey, TValue>());
}
