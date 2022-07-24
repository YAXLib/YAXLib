// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

#nullable enable

using System;

namespace YAXLib.Caching;

/// <summary>
///     Implements a singleton cache for <see cref="UdtWrapper"/>s
///     to prevent creation of <see cref="UdtWrapper"/>s for the same type repetitively.
/// </summary>
internal class UdtWrapperCache : TypeCacheAbstract<UdtWrapper>
{
    public const int DefaultCacheSize = 500;

    /// <summary>
    ///     The singleton cache instance.
    /// </summary>
    private static UdtWrapperCache? _instance;

    private UdtWrapperCache()
    {
        MaxCacheSize = DefaultCacheSize;
    }

    /// <summary>
    ///     Gets the singleton instance of the <see cref="UdtWrapperCache"/>.
    /// </summary>
    public static UdtWrapperCache Instance => _instance ??= new UdtWrapperCache();

    /// <summary>
    ///     Gets the <see cref="UdtWrapper"/> for to the specified type.
    /// </summary>
    /// <param name="t">The type whose wrapper is needed.</param>
    /// <param name="caller">reference to the serializer instance which called this method.</param>
    /// <returns>the type wrapper corresponding to the specified type</returns>
    public UdtWrapper GetOrAddItem(Type t, YAXSerializer? caller)
    {
        lock (Locker)
        {
            if (!CacheDictionary.TryGetValue(t, out var udtWrapper))
            {
                udtWrapper = new UdtWrapper(t, caller);
                Add(t, udtWrapper);
            }
            else
                udtWrapper.SetYAXSerializationOptions(caller);
                
            return udtWrapper;
        }
    }
}
