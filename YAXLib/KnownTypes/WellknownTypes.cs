// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;

namespace YAXLib.KnownTypes;

/// <summary>
/// Provides serialization and deserialization methods for known types.
/// The target of a <see cref="KnownTypeBase{T}" /> may be defined at compile time or runtime.
/// </summary>
public static class WellKnownTypes
{
    /// <summary>
    /// This dictionary stores the known types derived from <see cref="KnownTypeBase{T}" />.
    /// <para>
    /// The underlying type of this known type is determined at compile time.
    /// </para>
    /// </summary>
    /// <example>Simple .NET built-in types like <see cref="TimeSpan" />, <see cref="DBNull" /> and others.</example>
    private static readonly Dictionary<Type, IKnownType> _dictKnownTypes = new();

    /// <summary>
    /// This dictionary stores the known <b>base</b> classes for other classes.
    /// That allows to have one <see cref="KnownTypeBase{T}" /> implementation for all the derived classes.
    /// </summary>
    /// <para>
    /// The underlying type of this known type is determined at compile time.
    /// </para>
    /// <example>
    /// For an <see cref="ArgumentException" />
    /// the known base class is <see cref="Exception" />.
    /// </example>
    private static readonly Dictionary<Type, IKnownType> _dictKnownBaseTypes = new();

    /// <summary>
    /// This dictionary stores the known types derived from <see cref="DynamicKnownTypeBase" />.
    /// <para>
    /// For classes derived from the <see cref="DynamicKnownTypeBase" /> base class,
    /// the <b>full type name</b> is defined at compile time.
    /// The underlying type of this known type is determined at runtime.
    /// </para>
    /// </summary>
    /// <example>
    /// <see cref="System.Drawing.Rectangle" />, <see cref="System.Drawing.Color" />,
    /// <see cref="System.Data.DataTable" /> and others.
    /// </example>
    private static readonly Dictionary<string, IKnownType> _dictDynamicKnownTypes = new();

    static WellKnownTypes()
    {
        RegisterDefaultKnownTypes();
    }

    private static void RegisterDefaultKnownTypes()
    {
        // Register all known types

        Add(new TimeSpanKnownType());
        Add(new DateOnlyKnownType());
        Add(new TimeOnlyKnownType());
        Add(new XElementKnownType());
        Add(new XAttributeKnownType());
        Add(new DbNullKnownType());
        Add(new TypeKnownType());

        Add(new ExceptionKnownBaseType());

        Add(new RectangleDynamicKnownType());
        Add(new ColorDynamicKnownType());
        Add(new RuntimeTypeDynamicKnownType());
        Add(new DataTableDynamicKnownType());
        Add(new DataSetDynamicKnownType());
    }

    /// <summary>
    /// Adds a known type to the dictionary of known types.
    /// </summary>
    /// <param name="knownType">The known type to add.</param>
    public static void Add<T>(KnownTypeBase<T> knownType)
    {
        _dictKnownTypes[knownType.Type] = knownType;
    }

    /// <summary>
    /// Adds a dynamic known type to the dictionary of known types.
    /// </summary>
    /// <param name="dynamicKnownType">The dynamic known type to add.</param>
    public static void Add(DynamicKnownTypeBase dynamicKnownType)
    {
        _dictDynamicKnownTypes[dynamicKnownType.TypeName] = dynamicKnownType;
    }

    /// <summary>
    /// Adds a known base type to the dictionary of known types.
    /// </summary>
    /// <param name="knownBaseType">The known base type to add.</param>
    public static void Add<T>(KnownBaseTypeBase<T> knownBaseType)
    {
        _dictKnownBaseTypes[knownBaseType.Type] = knownBaseType;
    }

    /// <summary>
    /// Removes the <see cref="IKnownType" /> for <paramref name="theType" /> from the list of well-known types.
    /// </summary>
    /// <param name="theType">The known type to remove.</param>
    /// <returns><see langword="true" />, if the type was removed successfully.</returns>
    public static bool Remove(Type theType)
    {
        return _dictKnownTypes.Remove(theType)
               || _dictDynamicKnownTypes.Remove(theType.FullName!)
               || _dictKnownBaseTypes.Remove(theType);
    }

    /// <summary>
    /// Removes all known types.
    /// </summary>
    public static void Clear()
    {
        _dictKnownTypes.Clear();
        _dictDynamicKnownTypes.Clear();
        _dictKnownBaseTypes.Clear();
    }

    /// <summary>
    /// Clears all known types and restores the default known types.
    /// </summary>
    public static void RestoreDefault()
    {
        Clear();
        RegisterDefaultKnownTypes();
    }

    /// <summary>
    /// Searches the known type dictionaries for the a <see cref="IKnownType" /> for the requested <paramref name="type" />.
    /// </summary>
    /// <param name="type">The requested <see cref="Type" /></param>
    /// <param name="knownType">The <see cref="IKnownType" />, if found.</param>
    /// <returns><see langword="true" />, if an <see cref="IKnownType" /> for the requested <paramref name="type" /> was found.</returns>
    public static bool TryGetKnownType(Type? type, out IKnownType? knownType)
    {
        knownType = null;

        if (type == null) return false;

        if (_dictKnownTypes.ContainsKey(type))
        {
            knownType = _dictKnownTypes[type];
            return true;
        }

        if (type.FullName != null && _dictDynamicKnownTypes.ContainsKey(type.FullName))
        {
            knownType = _dictDynamicKnownTypes[type.FullName];
            return true;
        }

        if (_dictKnownBaseTypes.Keys.Any(k => ReflectionUtils.IsBaseClassOrSubclassOf(type, k.FullName)))
        {
            knownType = _dictKnownBaseTypes[
                _dictKnownBaseTypes.Keys.First(k => ReflectionUtils.IsBaseClassOrSubclassOf(type, k.FullName))];
            return true;
        }

        return false;
    }
}
