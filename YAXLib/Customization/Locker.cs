// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Concurrent;

namespace YAXLib.Customization;

internal sealed class Locker : IDisposable
{
    private static readonly ConcurrentDictionary<(int, Type), int> LockedTypes = new();
    private readonly Type _lockedType;

    public Locker(Type typeToLock)
    {
        _lockedType = typeToLock;
        if (!LockedTypes.TryAdd((Environment.CurrentManagedThreadId, typeToLock), 0))
            throw new ArgumentException("The type is already locked.", nameof(typeToLock));
    }

    public static bool IsLocked(Type typeToTest)
    {
        return LockedTypes.ContainsKey((Environment.CurrentManagedThreadId, typeToTest));
    }

    public void Dispose()
    {
        LockedTypes.TryRemove((Environment.CurrentManagedThreadId, _lockedType), out _);
    }
}
