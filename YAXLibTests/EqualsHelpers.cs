// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace YAXLibTests;

/// <summary>
/// Helpers for .Equals
/// </summary>
internal class EqualsHelpers
{
    /// <summary>
    /// Equals ICollection, should be in same order
    /// </summary>
    /// <param name="me"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static bool CollectionEquals(ICollection? me, ICollection? other)
    {
        if (me == null) return other == null;
        if (other == null) return false;
        if (me.Count != other.Count) return false;

        return me.Cast<object>().SequenceEqual(other.Cast<object>());
    }

    /// <summary>
    /// Equals IDictionary, order doesn't matter
    /// </summary>
    public static bool DictionaryEquals(IDictionary? me, IDictionary? other)
    {
        if (me == null) return other == null;
        if (other == null) return false;

        if (me.Count != other.Count) return false;

        foreach (var k in me.Keys)
        {
            if (k is null) return false;
            if (!other.Contains(k)) return false;
            var val1 = me[k];
            var val2 = other[k];
            if (!Equals(val1, val2)) return false;
        }

        foreach (var k in other.Keys)
            if (k is null || !me.Contains(k))
                return false;
        return true;
    }
}
