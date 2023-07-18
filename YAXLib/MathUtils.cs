// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib;

internal static class MathUtils
{
    /// <summary>
    /// Determines whether two double-precision floating-point numbers are approximately equal.
    /// </summary>
    /// <param name="d1">The first double-precision floating-point number to compare.</param>
    /// <param name="d2">The second double-precision floating-point number to compare.</param>
    /// <returns>
    /// <c>true</c> if the two numbers are approximately equal; otherwise, <c>false</c>.
    /// </returns>
    public static bool ApproxEquals(double d1, double d2)
    {
        const double epsilon = 2.2204460492503131E-16;

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (d1 == d2)
        {
            return true;
        }

        double tolerance = (Math.Abs(d1) + Math.Abs(d2) + 10.0) * epsilon;
        double difference = d1 - d2;

        return -tolerance < difference && tolerance > difference;
    }
}
