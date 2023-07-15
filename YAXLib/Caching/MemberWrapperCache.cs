// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using YAXLib.Options;

namespace YAXLib.Caching;

/// <summary>
/// Implements a singleton cache for <see cref="MemberWrapper" />s
/// to prevent creation of <see cref="MemberWrapper" />s for the same type repetitively.
/// <para>
/// <b>Note</b>: The cache contains <b>all members</b> of a type that can <b>generally</b> be serialized.
/// Filtering of undesired members takes place later in the de/serialization process.
/// </para>
/// </summary>
internal class MemberWrapperCache : CacheBase<(Type type, SerializerOptions options), IList<MemberWrapper>>
{
    public const int DefaultCacheSize = 1000;

    private MemberWrapperCache()
    {
        MaxCacheSize = DefaultCacheSize;
    }

    /// <summary>
    /// Gets the singleton instance of the <see cref="MemberWrapperCache" />.
    /// </summary>
    public static MemberWrapperCache Instance
    {
        get
        {
            if (_instance is not null) return (MemberWrapperCache) _instance;

            // Implementing double-checked locking pattern
            lock (Locker)
            {
                if (_instance is null) SetInstanceVariable(new MemberWrapperCache()); // The second (double) check
            }

            return (MemberWrapperCache) _instance!;
        }
    }

    /// <summary>
    /// Gets the <see cref="MemberWrapper" />s for to the specified type.
    /// </summary>
    /// <param name="to">The member whose wrapper is needed.</param>
    /// <param name="memberWrappers">
    /// The lists of <see cref="MemberWrapper" />s from the cache,
    /// or an empty list, if the type did not exist in the cache.
    /// </param>
    /// <returns><see langword="true" />, if <paramref name="to" /> was found in the cache.</returns>
    public bool TryGetItem((Type type, SerializerOptions options) to, out IList<MemberWrapper> memberWrappers)
    {
        if (_instance is not null)
        {
            lock (Locker)
            {
                if (CacheDictionary.TryGetValue(to, out var mw))
                {
                    memberWrappers = mw;
                    return true;
                }
            }
        }

        memberWrappers = new List<MemberWrapper>();
        return false;
    }
}
