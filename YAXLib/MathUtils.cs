// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib;

internal static class MathUtils
{
    public static bool ApproxEquals(double d1, double d2)
    {
        const double epsilon = 2.2204460492503131E-16;

        if (d1 == d2)
        {
            return true;
        }

        double tolerance = (Math.Abs(d1) + Math.Abs(d2) + 10.0) * epsilon;
        double difference = d1 - d2;

        return -tolerance < difference && tolerance > difference;
    }
}
