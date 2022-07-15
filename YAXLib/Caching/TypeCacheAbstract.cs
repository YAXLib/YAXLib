// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

#nullable enable

using System;
using System.Collections.Generic;

namespace YAXLib.Caching;

internal abstract class TypeCacheAbstract<TV> : TypeCacheStaticAbstract
{
    /// <summary>
    /// A dictionary from <see cref="Type"/>s to the associated cache value.
    /// </summary>
    internal readonly Dictionary<Type, TV> CacheDictionary = new();

    /// <summary>
    /// Stores the dictionary keys in the sequence as added
    /// </summary>
    protected readonly List<Type> TypeList = new();

    /// <summary>
    /// Gets or sets the maximum number of items in the cache.
    /// </summary>
    public int MaxCacheSize
    {
        get => _maxCacheSize;
        set => _maxCacheSize = value >= 0 ? value : 0;
    }

    private int _maxCacheSize;

    /// <summary>
    /// Adds the value for key <see cref="Type"/> to the cache dictionary,
    /// if it does not exist yet.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns><see langword="true"/> if the item could be added, else <see langword="false"/>.</returns>
    public bool TryAdd(Type key, TV value)
    {
        lock (Locker)
        {
            if (!CacheDictionary.ContainsKey(key))
            {
                Add(key, value);
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Adds the value for key <see cref="Type"/> to the cache dictionary.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <exception cref="ArgumentException">Throws if the <paramref name="key"/> already exists.</exception>
    public void Add(Type key, TV value)
    {
        lock (Locker)
        {
            if(CacheDictionary.Count == MaxCacheSize)
                EvictItems();

            CacheDictionary.Add(key, value);
            TypeList.Add(key);
        }
    }

    /// <summary>
    /// Evict items from cache to keep its maximum size.
    /// </summary>
    protected void EvictItems()
    {
        lock (Locker)
        {
            while (CacheDictionary.Count > 0 && CacheDictionary.Count > MaxCacheSize - 1)
            {
                CacheDictionary.Remove(TypeList[0]);
                TypeList.RemoveAt(0);
            }
        }
    }

    /// <summary>
    /// Clears the cache dictionary
    /// </summary>
    public void Clear()
    {
        lock (Locker)
        {
            CacheDictionary.Clear();
            TypeList.Clear();
        }
    }
}
