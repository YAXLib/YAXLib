// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

#nullable enable
using System;
using System.Text;
using System.Threading;

namespace YAXLib.Pooling.SpecializedPools;

/// <summary>
/// The object pool for <see cref="StringBuilder"/>.
/// </summary>
internal sealed class StringBuilderPool : SpecializedPoolAbstract<StringBuilder>
{
    private static readonly Lazy<StringBuilderPool> Lazy = new(() => new StringBuilderPool(),
        LazyThreadSafetyMode.PublicationOnly);
        
    /// <summary>
    /// CTOR.
    /// </summary>
    /// <remarks>
    /// <see cref="SpecializedPoolAbstract{T}.Policy"/> must be set before initializing the pool
    /// </remarks>
    private StringBuilderPool()
    {
        Policy.FunctionOnCreate = () => new StringBuilder(DefaultStringBuilderCapacity);
        Policy.ActionOnReturn = sb =>
        {
            sb.Capacity = DefaultStringBuilderCapacity;
            sb.Clear();
        };
    }

    /// <summary>
    /// Gets or sets the <see cref="StringBuilder.Capacity"/>, that is used
    /// when creating new instances, or when returning an instance to the pool.
    /// <para>The default capacity is 1024.</para>
    /// </summary>
    public int DefaultStringBuilderCapacity { get; set; } = 1024;

    /// <summary>
    /// Gets a singleton instance of the pool.
    /// </summary>
    public static StringBuilderPool Instance =>
        Lazy.IsValueCreated ? Lazy.Value : PoolRegistry.Add(Lazy.Value);
}
