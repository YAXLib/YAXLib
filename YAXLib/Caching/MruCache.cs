// 
// Copyright (c) 2004-2021 Jaroslaw Kowalski <jaak@jkowalski.net>, Kim Christensen, Julian Verdurmen
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions 
// are met:
// 
// * Redistributions of source code must retain the above copyright notice, 
//   this list of conditions and the following disclaimer. 
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution. 
// 
// * Neither the name of Jaroslaw Kowalski nor the names of its 
//   contributors may be used to endorse or promote products derived from this
//   software without specific prior written permission. 
// 

#nullable enable

using System.Collections.Generic;

namespace YAXLib.Caching;

/// <summary>
/// Most-Recently-Used-Cache, that discards less frequently used items on overflow
/// </summary>
internal class MruCache<TKey, TValue>
{
    private readonly Dictionary<TKey, MruCacheItem> _dictionary;
    private readonly int _maxCapacity;
    private long _currentVersion;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="maxCapacity">Maximum number of items the cache will hold before discarding.</param>
    public MruCache(int maxCapacity)
    {
        _maxCapacity = maxCapacity;
        _dictionary = new Dictionary<TKey, MruCacheItem>(_maxCapacity / 4);
        _currentVersion = 1;
    }

    /// <summary>
    /// Attempt to insert item into cache.
    /// </summary>
    /// <param name="key">Key of the item to be inserted in the cache.</param>
    /// <param name="value">Value of the item to be inserted in the cache.</param>
    /// <returns><c>true</c> when the key does not already exist in the cache, <c>false</c> otherwise.</returns>
    public bool TryAddValue(TKey key, TValue value)
    {
        lock (_dictionary)
        {
            if (_dictionary.TryGetValue(key, out var item))
            {
                var version = _currentVersion;
                if (item.Version != version || !EqualityComparer<TValue>.Default.Equals(value, item.Value))
                {
                    _dictionary[key] = new MruCacheItem(value, version, false);
                }

                return false; // Already exists
            }

            if (_dictionary.Count >= _maxCapacity)
            {
                ++_currentVersion;
                PruneCache();
            }

            _dictionary.Add(key, new MruCacheItem(value, _currentVersion, true));
            return true;
        }
    }

    private void PruneCache()
    {
        // There are 3 priorities:
        //  - High Priority - Non-Virgins with version very close to _currentVersion
        //  - Medium Priority - Version close to _currentVersion
        //  - Low Priority - Old-Virgins
        // Make 3 sweeps:
        //  - Kill all the old ones
        //  - Kill all the virgins
        //  - Slaughterhouse
        var latestGeneration = _currentVersion - 2;
        long oldestGeneration = 1;
        var pruneKeys = new List<TKey>((int) (_dictionary.Count / 2.5));
        for (var i = 0; i < 3; ++i)
        {
            var oldGeneration = i switch {
                0 => _currentVersion - (int) (_maxCapacity / 1.5),
                1 => _currentVersion - 10,
                _ => _currentVersion - 5
            };
            if (oldGeneration < oldestGeneration)
                oldGeneration = oldestGeneration;

            oldestGeneration = long.MaxValue;

            foreach (var element in _dictionary)
            {
                var elementGeneration = element.Value.Version;
                if (elementGeneration <= oldGeneration ||
                    (element.Value.Virgin && (i != 0 || elementGeneration < latestGeneration)))
                {
                    pruneKeys.Add(element.Key);
                    if (_dictionary.Count - pruneKeys.Count < _maxCapacity / 1.5)
                    {
                        i = 3;
                        break; // Do not clear the entire cache
                    }
                }
                else if (elementGeneration < oldestGeneration)
                {
                    oldestGeneration = elementGeneration;
                }
            }
        }

        foreach (var pruneKey in pruneKeys)
        {
            _dictionary.Remove(pruneKey);
        }

        if (_dictionary.Count >= _maxCapacity)
        {
            _dictionary.Clear(); // Failed to perform sweep, fallback to fail safe
        }
    }

    /// <summary>
    /// Lookup existing item in cache.
    /// </summary>
    /// <param name="key">Key of the item to be searched in the cache.</param>
    /// <param name="value">Output value of the item found in the cache.</param>
    /// <returns><c>True</c> when the key is found in the cache, <c>false</c> otherwise.</returns>
    public bool TryGetValue(TKey key, out TValue? value)
    {
        MruCacheItem item;
        try
        {
            lock (_dictionary)
            {
                if (!_dictionary.TryGetValue(key, out item))
                {
                    value = default;
                    return false;
                }
            }
        }
        catch
        {
            item = default; // Too many people in the same room
        }

        if (item.Version != _currentVersion || item.Virgin)
        {
            // Update item version to mark as recently used
            lock (_dictionary)
            {
                var version = _currentVersion;
                if (_dictionary.TryGetValue(key, out item))
                {
                    if (item.Version != version || item.Virgin)
                    {
                        if (item.Virgin)
                        {
                            version = ++_currentVersion;
                        }

                        _dictionary[key] = new MruCacheItem(item.Value, version, false);
                    }
                }
                else
                {
                    value = default;
                    return false;
                }
            }
        }

        value = item.Value;
        return true;
    }

    private struct MruCacheItem
    {
        public readonly TValue Value;
        public readonly long Version;
        public readonly bool Virgin;

        public MruCacheItem(TValue value, long version, bool virgin)
        {
            Value = value;
            Version = version;
            Virgin = virgin;
        }
    }
}