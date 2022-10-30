// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using YAXLib.Options;

namespace YAXLib;

/// <summary>
/// A manager that keeps a map of namespaces to their prefixes (if any) to be added ultimately to the xml result
/// </summary>
internal class XmlNamespaceManager
{
    /// <summary>
    /// A map of namespaces to their prefixes (if any) to be added ultimately to the xml result
    /// </summary>
    private readonly Dictionary<XNamespace, string> _namespaceToPrefix = new();

    internal Dictionary<XNamespace, string> NamespaceToPrefix => _namespaceToPrefix;

    /// <summary>
    /// This instance will be (re-) initialized it a way
    /// that it has the same virgin state like an instance that
    /// was created with one of the CTORs.
    /// </summary>
    internal void Initialize()
    {
        _namespaceToPrefix.Clear();
    }


    /// <summary>
    /// Registers the namespace to be added to the root element of the serialized document.
    /// </summary>
    /// <param name="ns">The namespace to be added</param>
    /// <param name="prefix">The prefix for the namespace.</param>
    internal void RegisterNamespace(XNamespace ns, string? prefix)
    {
        if (!ns.IsEmpty())
            return;

        prefix ??= string.Empty;

        if (_namespaceToPrefix.ContainsKey(ns))
        {
            var existingPrefix = _namespaceToPrefix[ns];
            // override the prefix only if already existing namespace has no prefix assigned
            if (string.IsNullOrEmpty(existingPrefix))
                _namespaceToPrefix[ns] = prefix;
        }
        else
        {
            _namespaceToPrefix.Add(ns, prefix);
        }
    }

    internal void ImportNamespaces(YAXSerializer otherSerializer)
    {
        foreach (var pair in otherSerializer.XmlNamespaceManager.NamespaceToPrefix)
            RegisterNamespace(pair.Key, pair.Value);
    }

    internal void AddNamespacesToElement(XElement rootNode, XNamespace documentDefaultNamespace,
        SerializerOptions options, UdtWrapper udtWrapper)
    {
        var nsNoPrefix = new List<XNamespace>();
        foreach (var ns in NamespaceToPrefix.Keys)
        {
            var prefix = NamespaceToPrefix[ns];
            if (string.IsNullOrEmpty(prefix))
            {
                nsNoPrefix.Add(ns);
            }
            else // if it has a prefix assigned
            {
                // if no namespace with this prefix already exists
                if (rootNode.GetNamespaceOfPrefix(prefix) == null)
                {
                    rootNode.AddAttributeNamespaceSafe(XNamespace.Xmlns + prefix, ns, documentDefaultNamespace,
                        options.Culture);
                }
                else // if this prefix is already added
                {
                    // check the namespace associated with this prefix
                    var existing = rootNode.GetNamespaceOfPrefix(prefix);
                    if (existing != ns)
                        throw new InvalidOperationException(string.Format(
                            "You cannot have two different namespaces with the same prefix." +
                            Environment.NewLine +
                            "Prefix: {0}, Namespaces: \"{1}\", and \"{2}\"",
                            prefix, ns, existing));
                }
            }
        }

        // if the main type wrapper has a default (no prefix) namespace
        if (udtWrapper.Namespace.IsEmpty() && string.IsNullOrEmpty(udtWrapper.NamespacePrefix))
            // it will be added automatically
            nsNoPrefix.Remove(udtWrapper.Namespace);

        // now generate namespaces for those without prefix
        foreach (var ns in nsNoPrefix)
            rootNode.AddAttributeNamespaceSafe(XNamespace.Xmlns + rootNode.GetRandomPrefix(), ns,
                documentDefaultNamespace, options.Culture);
    }
}