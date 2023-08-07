// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using YAXLib.Options;

namespace YAXLib.Caching;

/// <summary>
/// Implements a singleton cache for <see cref="UdtWrapper" />s
/// to prevent creation of <see cref="UdtWrapper" />s for the same type repetitively.
/// </summary>
internal class UdtWrapperCache : CacheBase<(Type type, SerializerOptions options), UdtWrapper>
{
    public const int DefaultCacheSize = 500;

    private UdtWrapperCache()
    {
        MaxCacheSize = DefaultCacheSize;
    }

    /// <summary>
    /// Gets the singleton instance of the <see cref="UdtWrapperCache" />.
    /// </summary>
    public static UdtWrapperCache Instance
    {
        get
        {
            // Implementing double-checked locking pattern
            if (_instance is not null) return (UdtWrapperCache) _instance; // First check

            lock (Locker)
            {
                if (_instance is null) SetInstanceVariable(new UdtWrapperCache()); // The second (double) check
            }

            return (UdtWrapperCache) _instance!;
        }
    }

    /// <summary>
    /// Gets the <see cref="UdtWrapper" /> for to the specified type.
    /// </summary>
    /// <param name="t">The type whose wrapper is needed.</param>
    /// <param name="serializerOptions">The <see cref="SerializerOptions" /> to use.</param>
    /// <returns>the type wrapper corresponding to the specified type</returns>
    public UdtWrapper GetOrAddItem(Type t, SerializerOptions serializerOptions)
    {
        lock (Locker)
        {
            if (CacheDictionary.TryGetValue((t, serializerOptions), out var udtWrapper))
            {
                return udtWrapper;
            }

            udtWrapper = new UdtWrapper(t, serializerOptions);
            Add((t, serializerOptions), udtWrapper);
            return udtWrapper;

        }
    }
}
