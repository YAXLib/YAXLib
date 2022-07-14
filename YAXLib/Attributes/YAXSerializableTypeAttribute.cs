// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using YAXLib.Enums;

namespace YAXLib.Attributes
{
    /// <summary>
    ///     Add this attribute to types, structs or classes which you want to override
    ///     their default serialization behaviour. This attribute is optional.
    ///     This attribute is applicable to classes and structures.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class YAXSerializableTypeAttribute : YAXBaseAttribute, IYaxTypeLevelAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXSerializableTypeAttribute" /> class.
        /// </summary>
        public YAXSerializableTypeAttribute()
        {
            FieldsToSerialize = YAXSerializationFields.PublicPropertiesOnly;
        }

        /// <summary>
        ///     Determines whether the serialization options property has been explicitly
        ///     set by the user.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the serialization options property has been explicitly
        ///     set by the user; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSerializationOptionSet()
        {
            return _isOptionSet;
        }

        /// <summary>
        ///     determines whether the serialization options property has been explicitly
        ///     set by the user.
        /// </summary>
        private bool _isOptionSet;

        /// <summary>
        ///     Private variable to hold the serialization options
        /// </summary>
        private YAXSerializationOptions _serializationOptions = YAXSerializationOptions.SerializeNullObjects;

        /// <summary>
        ///     Gets or sets the fields which YAXLib selects for serialization
        /// </summary>
        /// <value>The fields to serialize.</value>
        public YAXSerializationFields FieldsToSerialize { get; set; }

        /// <summary>
        ///     Gets or sets the serialization options.
        /// </summary>
        /// <value>The options.</value>
        public YAXSerializationOptions Options
        {
            get { return _serializationOptions; }

            set
            {
                _serializationOptions = value;
                _isOptionSet = true;
            }
        }

        /// <inheritdoc/>
        void IYaxTypeLevelAttribute.Setup(UdtWrapper udtWrapper)
        {
            udtWrapper.FieldsToSerialize = FieldsToSerialize;
            if (IsSerializationOptionSet())
            {
                udtWrapper.SetSerializationOptionsFromAttribute(Options);
            } 
        }
    }
}