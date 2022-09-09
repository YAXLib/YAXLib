// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

namespace YAXLib
{
    /// <summary>
    /// Attributes used on type-level must implement this interface.
    /// </summary>
    internal interface IYaxTypeLevelAttribute
    {
        /// <summary>
        /// The method is invoked by <see cref="UdtWrapper"/>.
        /// Initial, attribute-specific properties for <see cref="UdtWrapper"/> will be set.
        /// </summary>
        /// <param name="udtWrapper"></param>
        void Setup (UdtWrapper udtWrapper);
    }
}
