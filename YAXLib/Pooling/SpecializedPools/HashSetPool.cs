// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace YAXLib.Pooling.SpecializedPools;

/// <summary>
/// Generic object pool implementation for <see cref="HashSet{T}" />s.
/// </summary>
internal sealed class HashSetPool<T> : CollectionPool<HashSet<T>, T>
{
    /// <summary>
    /// CTOR.
    /// </summary>
    /// <remarks>
    /// <see cref="SpecializedPoolBase{T}.Policy" /> must be set before initializing the pool
    /// </remarks>
    private HashSetPool()
    {
        // Use initialization of base class
    }

    /// <summary>
    /// Gets a singleton instance of the pool.
    /// </summary>
    public static new HashSetPool<T> Instance => PoolRegistry.GetOrAdd(() => new HashSetPool<T>());
}
