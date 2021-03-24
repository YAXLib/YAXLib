// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;

namespace YAXLib
{
    /// <summary>
    ///     Implements a singleton pool of type-wrappers to prevent excessive creation of
    ///     repetetive type wrappers
    /// </summary>
    internal class TypeWrappersPool
    {
        /// <summary>
        ///     The instance to the pool, to implement the singleton
        /// </summary>
        private static TypeWrappersPool _instance;

        /// <summary>
        ///     A dictionary from types to their corresponding wrappers
        /// </summary>
        private readonly Dictionary<Type, UdtWrapper> _dicTypes = new Dictionary<Type, UdtWrapper>();

        /// <summary>
        ///     An object to lock type-wrapper dictionary to make it thread-safe
        /// </summary>
        private readonly object _lockDic = new object();

        /// <summary>
        ///     Prevents a default instance of the <c>TypeWrappersPool</c> class from being created, from
        ///     outside the scope of this class.
        /// </summary>
        private TypeWrappersPool()
        {
        }

        /// <summary>
        ///     Gets the type wrappers pool.
        /// </summary>
        /// <value>The type wrappers pool.</value>
        public static TypeWrappersPool Pool
        {
            get
            {
                if (_instance == null)
                    _instance = new TypeWrappersPool();
                return _instance;
            }
        }

        /// <summary>
        ///     Cleans up the pool.
        /// </summary>
        [Obsolete("Will be removed in v4. Do not use.")]
        public static void CleanUp()
        {
            if (_instance != null)
            {
                _instance = null;
                // TODO: not sure if it's good work to do
                GC.Collect();
            }
        }

        /// <summary>
        ///     Gets the type wrapper corresponding to the specified type.
        /// </summary>
        /// <param name="t">The type whose wrapper is needed.</param>
        /// <param name="caller">reference to the serializer instance which called this method.</param>
        /// <returns>the type wrapper corresponding to the specified type</returns>
        public UdtWrapper GetTypeWrapper(Type t, YAXSerializer caller)
        {
            lock (_lockDic)
            {
                UdtWrapper result;
                if (!_dicTypes.TryGetValue(t, out result))
                {
                    result = new UdtWrapper(t, caller);
                    _dicTypes.Add(t, result);
                }
                else
                {
                    result.SetYAXSerializerOptions(caller);
                }

                return result;
            }
        }
    }
}