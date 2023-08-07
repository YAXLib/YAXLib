// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Pooling.SpecializedPools;

namespace YAXLib.Pooling.YAXLibPools;

/// <summary>
/// The object pool for <see cref="YAXSerializer" />.
/// </summary>
internal sealed class SerializerPool : PoolBase<YAXSerializer>
{
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
    public static SerializerPool Instance => PoolRegistry.GetOrAdd(() => new SerializerPool());
}
