// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading;

namespace YAXLib.Customization;

internal class Locker : IDisposable
{
    private static readonly ThreadLocal<HashSet<Type>> LockedTypes = new(() => new HashSet<Type>());
    private readonly Type _lockedType;

    public Locker(Type typeToLock)
    {
        _lockedType = typeToLock;
        if (!LockedTypes.Value!.Add(typeToLock))
            throw new ArgumentException("The type is already locked.", nameof(typeToLock));
    }

    public static bool IsLocked(Type typeToTest)
    {
        return LockedTypes.Value!.Contains(typeToTest);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            LockedTypes.Value!.Remove(_lockedType);
        }
        else
        {
            LockedTypes.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
