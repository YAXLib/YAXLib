// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using YAXLib;

namespace YAXLibTests.TestHelpers;

public static class SerializerExtensions
{
    public static int GetRecursionCount<T>(this IYAXSerializer<T> ser)
    {
        return ((IRecursionCounter) ser).RecursionCount;
    }

    public static void SetRecursionCount<T>(this IYAXSerializer<T> ser, int value)
    {
        ((IRecursionCounter) ser).RecursionCount = value;
    }
}