// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Text;

namespace YAXLib.Pooling.SpecializedPools;

/// <summary>
/// The object pool for <see cref="StringBuilder" />.
/// </summary>
internal sealed class StringBuilderPool : SpecializedPoolBase<StringBuilder>
{
    /// <summary>
    /// CTOR.
    /// </summary>
    /// <remarks>
    /// <see cref="SpecializedPoolBase{T}.Policy" /> must be set before initializing the pool
    /// </remarks>
    private StringBuilderPool()
    {
        Policy.FunctionOnCreate = () => new StringBuilder(DefaultStringBuilderCapacity);
        Policy.ActionOnReturn = sb =>
        {
            sb.Clear(); // Clear the StringBuilder before setting the new capacity
            sb.Capacity = DefaultStringBuilderCapacity;
        };
    }

    /// <summary>
    /// Gets or sets the <see cref="StringBuilder.Capacity" />, that is used
    /// when creating new instances, or when returning an instance to the pool.
    /// <para>The default capacity is 1024.</para>
    /// </summary>
    public int DefaultStringBuilderCapacity { get; set; } = 1024;

    /// <summary>
    /// Gets a singleton instance of the pool.
    /// </summary>
    public static StringBuilderPool Instance => PoolRegistry.GetOrAdd(() => new StringBuilderPool());
}
