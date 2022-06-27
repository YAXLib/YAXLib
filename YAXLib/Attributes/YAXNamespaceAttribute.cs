// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;

namespace YAXLib.Attributes
{
    /// <summary>
    ///     Makes an element make use of a specific XML namespace.
    ///     This attribute is applicable to classes, structs, fields, enums and properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Field |
                    AttributeTargets.Property | AttributeTargets.Struct)]
    public class YAXNamespaceAttribute : YAXBaseAttribute, IYaxMemberLevelAttribute, IYaxTypeLevelAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXNamespaceAttribute" /> class.
        /// </summary>
        /// <remarks>
        ///     The element this applies to will take on the given XML namespace. In the case
        ///     of this constructor, the default one defined by xmlns="namespace"
        /// </remarks>
        /// <param name="defaultNamespace">The default namespace to use for this item</param>
        public YAXNamespaceAttribute(string defaultNamespace)
        {
            Namespace = defaultNamespace;
            Prefix = null;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAXNamespaceAttribute" /> class.
        /// </summary>
        /// <remarks>
        ///     The element this applies to will take on the given XML namespace. The namespace
        ///     will be added to the root XML element, with the given prefix in the form:
        ///     xmlns:prefix="namespace"
        /// </remarks>
        /// <param name="namespacePrefix">The prefix to use for this element's namespace</param>
        /// <param name="xmlNamespace">The xml namespace to use for this item</param>
        public YAXNamespaceAttribute(string namespacePrefix, string xmlNamespace)
        {
            Namespace = xmlNamespace;
            Prefix = namespacePrefix;
        }

        /// <summary>
        ///     The namespace path
        /// </summary>
        public string Namespace { get; }

        /// <summary>
        ///     The xml prefix used for the namespace
        /// </summary>
        public string Prefix { get; }

        /// <inheritdoc/>
        void IYaxMemberLevelAttribute.Setup(MemberWrapper memberWrapper)
        {
            memberWrapper.Namespace = Namespace;
            memberWrapper.NamespacePrefix = Prefix;
        }

        /// <inheritdoc/>
        void IYaxTypeLevelAttribute.Setup(UdtWrapper udtWrapper)
        {
            udtWrapper.Namespace = Namespace;
            udtWrapper.NamespacePrefix = Prefix;
        }
    }
}