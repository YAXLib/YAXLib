// Copyright (C) Sina Iravanian, Julian Verdurmen, axuno gGmbH and other contributors.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using YAXLib;
using YAXLib.Options; 

namespace YAXLibTests;
[TestFixture]
public class MarkObjId2AvertSefRef01
{
    #region Test01
    public class CPack
    {
        public class CBoxA
        {
            public int Int02 { get; set; } = 333;
            public string Name { get; set; }
            public CBoxB Child { get; set; }
        }
        public class CBoxB
        {
            public int Int01 { get; set; } = 444;
            public string Name { get; set; }
            public CBoxC Child { get; set; }
        }
        public class CBoxC
        {
            public int Int02 { get; set; } = 555;
            public string Name { get; set; }
            public CBoxA Child { get; set; }
        }
        public int Int01 { get; set; } = 666;
        public string Name { get; set; }
        public CBoxA ItWillLoop { get; set; }
        public static CPack GetSampleInstance()
        {
            CBoxA boxa = new() { Name = "AAA" };
            CBoxB boxb = new() { Name = "BBB" };
            CBoxC boxc = new() { Name = "CCC" };
            boxa.Child = boxb;
            boxb.Child = boxc;
            boxc.Child = boxa;
            return new CPack { Name = "PackLoop", ItWillLoop = boxa };
        }
    }
    [Test]
    public void Se01()
    {
        var result = """
            <CPack xmlns:yaxlib="http://www.sinairv.com/>
              <Int01>666</Int01>
              <Name>PackLoop</Name>
              <ItWillLoop yaxlib:id="1000">
                <Int02>333</Int02>
                <Name>AAA</Name>
                <Child>
                  <Int01>444</Int01>
                  <Name>BBB</Name>
                  <Child>
                    <Int02>555</Int02>
                    <Name>CCC</Name>
                    <Child yaxlib:ref="1000" />
                  </Child>
                </Child>
              </ItWillLoop>
            </CPack>
            """;


        var serializer = new YAXSerializer<CPack>(
        new SerializerOptions
        {
            SerializationOptions = YAXLib.Enums.YAXSerializationOptions.MarkObjId2AvertSefRef
        });


        var got = serializer.Serialize(CPack.GetSampleInstance());
        Console.WriteLine("got:" + got);
        Console.WriteLine("result:" + result);
        Assert.AreEqual(got, result);
    }
    [Test]
    public void De01()
    {
        var xmlInput = """
            <CPack xmlns:yaxlib="http://www.sinairv.com/>
              <Int01>666</Int01>
              <Name>PackLoop</Name>
              <ItWillLoop yaxlib:id="1000">
                <Int02>333</Int02>
                <Name>AAA</Name>
                <Child>
                  <Int01>444</Int01>
                  <Name>BBB</Name>
                  <Child>
                    <Int02>555</Int02>
                    <Name>CCC</Name>
                    <Child yaxlib:ref="1000" />
                  </Child>
                </Child>
              </ItWillLoop>
            </CPack>
            """;


        var serializer = new YAXSerializer<CPack>(
            new SerializerOptions
            {
                SerializationOptions = YAXLib.Enums.YAXSerializationOptions.MarkObjId2AvertSefRef
            });


        var got = serializer.Deserialize(xmlInput);
        Console.WriteLine(got);
        Assert.AreSame(got.ItWillLoop, got.ItWillLoop.Child.Child.Child);
    }

    #endregion Test01

    public class CPack02
    {
        public class CBox
        {
            public string Name { get; set; }
        }

        public List<CBox> BoxList { get; set; }


        public static CPack02 GetSampleInstance()
        {
            var newone = new CPack02() { BoxList = new List<CBox>() };

            for (int i = 0; i < 5; i++)
            {
                newone.BoxList.Add(new CBox
                {
                    Name = $"Node{i}",
                });
            }

            newone.BoxList[0] = newone.BoxList[2];
            newone.BoxList[4] = newone.BoxList[2];

            return newone;
        }


    }

    [Test]
    public void Se02()
    {
        var result = """
         <CPack02 xmlns:yaxlib="http://www.sinairv.com/>
           <BoxList>
             <CBox yaxlib:id="1000">
               <Name>Node2</Name>
             </CBox>
             <CBox>
               <Name>Node1</Name>
             </CBox>
             <CBox yaxlib:ref="1000" />
             <CBox>
               <Name>Node3</Name>
             </CBox>
             <CBox yaxlib:ref="1000" />
           </BoxList>
         </CPack02>
         """;


        var serializer = new YAXSerializer<CPack02>(
        new SerializerOptions
        {
            SerializationOptions = YAXLib.Enums.YAXSerializationOptions.MarkObjId2AvertSefRef
        });


        var got = serializer.Serialize(CPack02.GetSampleInstance());
        Console.WriteLine("got:" + got);
        Console.WriteLine("result:" + result);
        Assert.AreEqual(got, result);
    }
    [Test]
    public void De02()
    {
        var xmlInput = """
            <CPack02 xmlns:yaxlib="http://www.sinairv.com/>
              <BoxList>
                <CBox yaxlib:id="1000">
                  <Name>Node2</Name>
                </CBox>
                <CBox>
                  <Name>Node1</Name>
                </CBox>
                <CBox yaxlib:ref="1000" />
                <CBox>
                  <Name>Node3</Name>
                </CBox>
                <CBox yaxlib:ref="1000" />
              </BoxList>
            </CPack02>
            """;


        var serializer = new YAXSerializer<CPack02>(
            new SerializerOptions
            {
                SerializationOptions = YAXLib.Enums.YAXSerializationOptions.MarkObjId2AvertSefRef
            });


        var got = serializer.Deserialize(xmlInput);
        Console.WriteLine(got);

        Assert.AreSame(got.BoxList[0], got.BoxList[2]);
        Assert.AreSame(got.BoxList[4], got.BoxList[2]);
    }
}
