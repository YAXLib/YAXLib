// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;

namespace YAXLib.Caching;

internal abstract class TypeCacheBase<T> : TypeCacheStaticBase
{
    /// <summary>
    /// The <see cref="TypeCacheBase{T}" /> instance.
    /// </summary>
    private protected static TypeCacheBase<T>? _instance { get; private set; }

    /// <summary>
    /// Sets the instance variable, if its current value is null.
    /// </summary>
    /// <param name="instance"></param>
    protected static void SetInstanceVariable(TypeCacheBase<T> instance)
    {
        lock (Locker)
        {
            _instance ??= instance;
        }
    }

    /// <summary>
    /// A dictionary from <see cref="Type" />s to the associated cache value.
    /// </summary>
    internal readonly Dictionary<Type, T> CacheDictionary = new();

    /// <summary>
    /// Stores the dictionary keys in the sequence as added
    /// </summary>
    protected List<Type> TypeList { get; } = new();

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
    /// Adds the value for key <see cref="Type" /> to the cache dictionary,
    /// if it does not exist yet.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns><see langword="true" /> if the item could be added, else <see langword="false" />.</returns>
    public bool TryAdd(Type key, T value)
    {
        if (_instance is null) return false;

        lock (Locker)
        {
            if (CacheDictionary.ContainsKey(key)) return false;

            Add(key, value);
            return true;
        }
    }

    /// <summary>
    /// Adds the value for key <see cref="Type" /> to the cache dictionary.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <exception cref="ArgumentException">Throws if the <paramref name="key" /> already exists.</exception>
    public void Add(Type key, T value)
    {
        if (_instance is null) return;

        lock (Locker)
        {
            if (CacheDictionary.Count == MaxCacheSize)
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
        if (_instance is null) return;

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
        if (_instance is null) return;

        lock (Locker)
        {
            CacheDictionary.Clear();
            TypeList.Clear();
        }
    }
}
