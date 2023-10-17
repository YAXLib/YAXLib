// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace YAXLib.MarkObjWithId;

internal static class Xmlhelper
{
//    public static string ATTR_FLAG_MARK_OBJID = "yaxlib:id";
//    public static string ATTR_FLAG_REF_OBJID = "yaxlib:ref";
    public readonly static XNamespace YaxlibNamespace = "http://www.sinairv.com/yaxlib/";
    public readonly static string PrefixNamespace = "yaxlib";
    readonly static XName OBJID_ELEMENT_NAME = XName.Get("id", YaxlibNamespace.ToString());

    /// <summary>
    /// Only the repeated ObjId is keept
    /// </summary>
    /// <param name="mainDocument"></param>
    /// <returns></returns>
    internal static XDocument ClearUnNecessaryObjId(this XDocument mainDocument)
    {


        var root = mainDocument.Root;
        var ItemArr = CollectItems(root, new List<ObjIdItem>());
        var group = ItemArr.GroupBy(x => x.ObjId, (idx, parents) => (ID: idx, CNT: parents.Count())).ToList();
        var IdToDelArr = group.Where(x => x.CNT == 1).Select(x => x.ID).ToList();
        ItemArr.Where(item => IdToDelArr.Exists(id => id == item.ObjId))
            .ToList()
            .ForEach(item => item.Parent.Attribute(OBJID_ELEMENT_NAME).Remove());

        return mainDocument;


        List<ObjIdItem> CollectItems(XElement xelement, List<ObjIdItem> ItemArr)
        {
            var attr = xelement.Attribute(OBJID_ELEMENT_NAME);
            if (attr != null)
                ItemArr.Add(new ObjIdItem { Parent = xelement, ObjId = int.Parse(attr.Value) });
            foreach (var xe in xelement.Elements())
                ItemArr = CollectItems(xe, ItemArr);
            return ItemArr;
        }
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
        var attr = xe?.Attribute(OBJID_ELEMENT_NAME);
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
        xe.Add(new XAttribute(YaxlibNamespace + "id", objId));

        /*
        //_serializer.XmlNamespaceManager.RegisterNamespace(YaxlibNamespace, PrefixNamespace);


        XElement root = xe;
        {

            while (root.Parent != null)
            {
                root = root.Parent;
            }
        }

        root.Add(new XAttribute(XNamespace.Xmlns + "yaxlib", YaxlibNamespace.ToString()));

        xe.Add(
               new XAttribute(YaxlibNamespace + "id", objId)
            );

        Console.WriteLine(xe);
        */


        /*
        XNamespace ns = "http://www.sinairv.com/yaxlib/";

         

        xe.Add(new XAttribute(XNamespace.Xmlns + "yaxlib", ns.ToString()),
               new XAttribute(ns + "id", objId)
            ); 

        Console.WriteLine(xe);
        */

        /*
        XNamespace aw = "http://www.adventure-works.com";
        XElement root = new XElement(aw + "Root",
            new XAttribute(XNamespace.Xmlns + "aw", "http://www.adventure-works.com"),
            new XElement(aw + "Child", "child content")
        );
        Console.WriteLine(root);
        */
        /*

        XNamespace ns = "http://www.sinairv.com/yaxlib/";      
        xmlNamespaceManager.RegisterNamespace(ns, "yaxlib");

        XElement root = xe;
        {
            
            while (root.Parent != null)
            {
                root = root.Parent;
            }
        }
        xe.Add(new XAttribute(XNamespace.Xmlns + "yaxlib", "http://www.adventure-works.com"),
               new XAttribute(ns + "id", $"{objId}")            
            );

        //elem.Add(new XAttribute(Xmlhelper.ATTR_FLAG_MARK_OBJID, id));
       // var xatt = new XAttribute(ns + "id", $"{objId}");
      //  xe.Add(xatt);

        */
    }
    public static void Attribute_RefObjId(this XElement xe, int objId)
    {

    }
    public static bool IsNullXmlNode(this XElement xe)
    {
       // var debug = xe.Attributes().Select(x => x.Name.NamespaceName).ToList();
        var attributCnt = xe.Attributes()
            .Where(x=>x.Name.NamespaceName!= YaxlibNamespace)
            .Count();
        var isEmpty = xe.IsEmpty;
        var HasElements = xe.HasElements;

        return attributCnt == 0 && isEmpty && !HasElements;
    }
}

