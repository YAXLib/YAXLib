// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib.Pooling.SpecializedPools;

namespace YAXLib.Pooling.YAXLibPools;

/// <summary>
/// The abstract base class for YAXLib pools.
/// Used to identify pools belonging to YAXLib.
/// </summary>
/// <typeparam name="T">The <see langword="type" /> of the YAXLib pool.</typeparam>
internal abstract class PoolBase<T> : SpecializedPoolBase<T> where T : class
{
    // No custom properties or methods for YAXLib pools yet.
}
