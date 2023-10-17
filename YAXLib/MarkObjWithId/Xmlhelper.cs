// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace YAXLib.MarkObjWithId;

internal static class Xmlhelper
{
    public readonly static XNamespace YaxlibNamespace = "http://www.sinairv.com/yaxlib/";
    public readonly static string PrefixNamespace = "yaxlib";
    readonly static XName OBJID_ELEMENT_NAME = XName.Get("id", YaxlibNamespace.ToString());
    readonly static XName OBJREF_ELEMENT_NAME = XName.Get("ref", YaxlibNamespace.ToString());

    /// <summary>
    /// Only the repeated ObjId is keept
    /// </summary>
    /// <param name="mainDocument"></param>
    /// <returns></returns>
    internal static XDocument ClearUnNecessaryObjId(this XDocument mainDocument)
    {
        var root = mainDocument.Root;
        var ItemArr = CollectYaxlibItems(root, new List<ObjIdItem>());
        var group = ItemArr.GroupBy(x => x.ObjId, (idx, parents) => (ID: idx, CNT: parents.Count())).ToList();
        var IdToDelArr = group.Where(x => x.CNT == 1).Select(x => x.ID).ToList();
        ItemArr.Where(item => IdToDelArr.Exists(id => id == item.ObjId))
            .ToList()
            .ForEach(item => item.Parent.Attribute(OBJID_ELEMENT_NAME).Remove());

        return mainDocument;
    }
    static List<ObjIdItem> CollectYaxlibItems(XElement xelement, List<ObjIdItem> ItemArr)
    {
        var attr = xelement.Attribute(OBJID_ELEMENT_NAME);
        if (attr != null)
            ItemArr.Add(new ObjIdItem { Parent = xelement, ObjId = int.Parse(attr.Value) });
        foreach (var xe in xelement.Elements())
            ItemArr = CollectYaxlibItems(xe, ItemArr);
        return ItemArr;
    }
    internal static XDocument FixObjIdToRef(this XDocument mainDocument)
    {
        var root = mainDocument.Root;
        var ItemArr = CollectYaxlibItems(root, new List<ObjIdItem>());
        var group = ItemArr.GroupBy(x => x.ObjId).ToList();
        foreach (var gpItem in group)
        {
            var objid = gpItem.Key;
            var arrHasMark = gpItem.ToList();
            var IdxSourceNode = arrHasMark.FindIndex(x => !x.Parent.IsNullXmlNode());
            if (IdxSourceNode == -1)
            {
                IdxSourceNode = 0;
            }
            for (int i = 0; i < arrHasMark.Count; i++)
            {
                if (i == IdxSourceNode)
                    continue;
                var node = arrHasMark[i].Parent;
                node.Attribute(OBJID_ELEMENT_NAME).Remove();
                node.Add(new XAttribute(OBJREF_ELEMENT_NAME, objid));
            }
        }
        return mainDocument;
    }

    class ObjIdItem
    {
        public int ObjId { get; set; }
        public XElement Parent { get; set; }

    }


    static public bool ShouldMarkObjId(this MemberWrapper member)
    {
        var memtype = member.MemberType;
        if (memtype.IsValueType)
            return false;
        if (!member.IsSerializedAsElement)
            return false;

        if (memtype.FullName == "System.String")
            return false;
        return true;

    }
    static public bool ShouldMarkObjId(this Type memtype)
    {
        if (memtype.IsValueType)
            return false;

        if (memtype.FullName == "System.String")
            return false;
        return true;

    }

    public static int? GetRefObjId(this XElement? xe)
    {
        var attr = xe?.Attribute(OBJID_ELEMENT_NAME);
        if (attr == null)
        {
            return null;
        }
        var rst = Int32.Parse(attr.Value);
        return rst;
    }
    public static int? GetMarkObjId(this XElement? xe)
    {
        var attr = xe?.Attribute(OBJREF_ELEMENT_NAME);
        if (attr == null)
        {
            return null;
        }
        var rst = Int32.Parse(attr.Value);
        return rst;
    }

    public static void AddAttribute_MarkObjId(this XElement xe, int objId, YAXSerializer _serializer)
    {
        _serializer.XmlNamespaceManager.RegisterNamespace(YaxlibNamespace, PrefixNamespace);
        xe.Add(new XAttribute(OBJID_ELEMENT_NAME, objId));
    }
    public static bool IsNullXmlNode(this XElement xe)
    {
        // var debug = xe.Attributes().Select(x => x.Name.NamespaceName).ToList();
        var attributCnt = xe.Attributes()
            .Where(x => x.Name.NamespaceName != YaxlibNamespace)
            .Count();
        var isEmpty = xe.IsEmpty;
        var HasElements = xe.HasElements;

        return attributCnt == 0 && isEmpty && !HasElements;
    }
}

