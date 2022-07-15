// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

#nullable enable

using System;
using System.Collections.Generic;
using YAXLib.Pooling.SpecializedPools;

namespace YAXLib.Caching;

/// <summary>
///     Implements a singleton cache for <see cref="MemberWrapper"/>s
///     to prevent creation of <see cref="MemberWrapper"/>s for the same type repetitively.
///     <para>
///     <b>Note</b>: The cache contains <b>all members</b> of a type that can <b>generally</b> be serialized.
///     Filtering of undesired members takes place later in the de/serialization process.
///     </para>
/// </summary>
internal class MemberWrapperCache : TypeCacheAbstract<IList<MemberWrapper>>
{
    public const int DefaultCacheSize = 1000;

    /// <summary>
    ///     The singleton cache instance.
    /// </summary>
    private static MemberWrapperCache? _instance;

    private MemberWrapperCache()
    {
        MaxCacheSize = DefaultCacheSize;
    }

    /// <summary>
    ///     Gets the singleton instance of the <see cref="MemberWrapperCache"/>.
    /// </summary>
    public static MemberWrapperCache Instance => _instance ??= new MemberWrapperCache();

    /// <summary>
    ///     Gets the <see cref="MemberWrapper"/>s for to the specified type.
    /// </summary>
    /// <param name="t">The member whose wrapper is needed.</param>
    /// <param name="memberWrappers">
    /// The lists of <see cref="MemberWrapper"/>s from the cache,
    /// or an empty list, if the type did not exist in the cache.
    /// </param>
    /// <returns><see langword="true"/>, if <paramref name="t"/> was found in the cache.</returns>
    public bool TryGetItem(Type t, out IList<MemberWrapper> memberWrappers)
    {
        lock (Locker)
        {
            if (CacheDictionary.TryGetValue(t, out memberWrappers))
            {
                return true;
            }
        }
        
        memberWrappers = new List<MemberWrapper>();
        return false;
    }
}
