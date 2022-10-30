// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Threading;
using YAXLib.Pooling.SpecializedPools;

namespace YAXLib.Pooling.YAXLibPools;

/// <summary>
/// The object pool for <see cref="YAXSerializer" />.
/// </summary>
internal sealed class SerializerPool : PoolBase<YAXSerializer>
{
    private static readonly Lazy<SerializerPool> Lazy = new(() => new SerializerPool(),
        LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// CTOR.
    /// </summary>
    /// <remarks>
    /// <see cref="SpecializedPoolBase{T}.Policy" /> must be set before initializing the pool
    /// </remarks>
    private SerializerPool()
    {
        // This means 50 recursive de/serializations for an object, which should usually suffice.
        // The pool is focusing on the high frequency of creating child serializers,
        // not a high number of child serializers at a time.
        Policy.MaximumPoolSize = 50;
        Policy.FunctionOnCreate = () => new YAXSerializer();
        Policy.ActionOnReturn = s => s.ReturnToPool();
    }

    /// <summary>
    /// Gets a singleton instance of the pool.
    /// </summary>
    public static SerializerPool Instance =>
        Lazy.IsValueCreated ? Lazy.Value : PoolRegistry.Add(Lazy.Value);
}
