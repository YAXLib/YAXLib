// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib
{
    /// <summary>
    ///     Specifies the order upon which a field or property is serialized / deserialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class YAXElementOrder : YAXBaseAttribute
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXElementOrder" /> class.
        /// </summary>
        /// <remarks>
        ///     The element this applies to will be given priority in being serialized or deserialized
        ///     depending on the relative value compared to other child elements.
        /// </remarks>
        /// <param name="order">The priority of the element in serializing and deserializing.</param>
        public YAXElementOrder(int order)
        {
            Order = order;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     The order used to prioritize serialization and deserialization.
        /// </summary>
        public int Order { get; }

        #endregion
    }
}