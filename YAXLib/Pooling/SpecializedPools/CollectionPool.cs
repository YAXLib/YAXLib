// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;

namespace YAXLib.Pooling.SpecializedPools;

/// <summary>
/// Generic object pool implementation for <see cref="ICollection{T}" />s.
/// </summary>
/// <typeparam name="TCollection"></typeparam>
/// name=""/>
/// <typeparam name="TItem"></typeparam>
/// name=""/>
internal class CollectionPool<TCollection, TItem> : SpecializedPoolBase<TCollection>
    where TCollection : class, ICollection<TItem>, new()
{
    private static readonly Lazy<CollectionPool<TCollection, TItem>> Lazy =
        new(() => new CollectionPool<TCollection, TItem>(), LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// CTOR.
    /// </summary>
    /// <remarks>
    /// <see cref="SpecializedPoolBase{T}.Policy" /> must be set before initializing the pool
    /// </remarks>
    protected CollectionPool()
    {
        Policy.FunctionOnCreate = () => new TCollection();
        Policy.ActionOnReturn = coll => coll.Clear();
    }

    /// <summary>
    /// Gets a singleton instance of the pool.
    /// </summary>
    public static CollectionPool<TCollection, TItem> Instance =>
        Lazy.IsValueCreated ? Lazy.Value : PoolRegistry.Add(Lazy.Value);
}
