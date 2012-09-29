// Copyright 2009 - 2010 Sina Iravanian - <sina@sinairv.com>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YAXLib;
using System.Threading;
using System.Globalization;
using YAXLibTests.SampleClasses;
using YAXLibTests.SampleClasses.Namespace;

namespace YAXLibTests
{
    [TestClass]
    public class NamespaceTest
    {
        [TestMethod]
        public void SingleNamespaceSerializationTest()
        {
            const string result = @"<!-- This example shows usage of a custom default namespace -->
" + "<SingleNamespaceSample xmlns=\"http://namespaces.org/default\">" + @"
  <StringItem>This is a test string</StringItem>
  <IntItem>10</IntItem>
</SingleNamespaceSample>";

            var serializer = new YAXSerializer(typeof(SingleNamespaceSample), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(SingleNamespaceSample.GetInstance());
            Assert.AreEqual(result, got);
        }

        [TestMethod]
        public void MultipleNamespaceSerializationTest()
        {
            const string result = @"<!-- This example shows usage of a number of custom namespaces -->
" + "<ns1:MultipleNamespaceSample xmlns:ns1=\"http://namespaces.org/ns1\" xmlns:ns2=\"http://namespaces.org/ns2\" xmlns:ns3=\"http://namespaces.org/ns3\">" + @"
  <ns1:BoolItem>True</ns1:BoolItem>
  <ns2:StringItem>This is a test string</ns2:StringItem>
  <ns3:IntItem>10</ns3:IntItem>
</ns1:MultipleNamespaceSample>";

            var serializer = new YAXSerializer(typeof(MultipleNamespaceSample), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(MultipleNamespaceSample.GetSampleInstance());
            Assert.AreEqual(result, got);
        }

        [TestMethod]
        public void AttributeNamespaceSerializationTest()
        {
            const string result = "<AttributeNamespaceSample xmlns:ns=\"http://namespaces.org/ns\" xmlns=\"http://namespaces.org/default\">" + @"
  <Attribs " + "attrib=\"value\" ns:attrib2=\"value2\"" + @" />
</AttributeNamespaceSample>";

            var serializer = new YAXSerializer(typeof(AttributeNamespaceSample), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(AttributeNamespaceSample.GetSampleInstance());
            Assert.AreEqual(result, got);
        }

        [TestMethod]
        public void MemberAndClassDifferentNamespacesDeserializationTest()
        {
            const string result = @"<CellPhone_MemberAndClassDifferentNamespaces xmlns:x1=""http://namespace.org/x1"" xmlns=""http://namespace.org/nsmain"">
  <x1:TheName>HTC</x1:TheName>
  <OS>Windows Phone 8</OS>
</CellPhone_MemberAndClassDifferentNamespaces>";

            var serializer = new YAXSerializer(typeof(CellPhone_MemberAndClassDifferentNamespaces), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(CellPhone_MemberAndClassDifferentNamespaces.GetSampleInstance());
            Assert.AreEqual(result, got);
        }

        [TestMethod]
        public void MemberAndClassDifferentNamespacePrefixesSerializationTest()
        {
            const string result = 
@"<xmain:CellPhone_MemberAndClassDifferentNamespacePrefixes xmlns:xmain=""http://namespace.org/nsmain"" xmlns:x1=""http://namespace.org/x1"">
  <x1:TheName>HTC</x1:TheName>
  <xmain:OS>Windows Phone 8</xmain:OS>
</xmain:CellPhone_MemberAndClassDifferentNamespacePrefixes>";

            var serializer = new YAXSerializer(typeof(CellPhone_MemberAndClassDifferentNamespacePrefixes), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(CellPhone_MemberAndClassDifferentNamespacePrefixes.GetSampleInstance());
            Assert.AreEqual(result, got);
        }

        [TestMethod]
        public void MultiLevelMemberAndClassDifferentNamespacesSerializationTest()
        {
            const string result =
@"<CellPhone_MultiLevelMemberAndClassDifferentNamespaces xmlns:x1=""http://namespace.org/x1"" xmlns=""http://namespace.org/nsmain"">
  <Level1>
    <Level2>
      <x1:TheName>HTC</x1:TheName>
    </Level2>
  </Level1>
  <OS>Windows Phone 8</OS>
</CellPhone_MultiLevelMemberAndClassDifferentNamespaces>";

            var serializer = new YAXSerializer(typeof(CellPhone_MultiLevelMemberAndClassDifferentNamespaces), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(CellPhone_MultiLevelMemberAndClassDifferentNamespaces.GetSampleInstance());
            Assert.AreEqual(result, got);
        }

        [TestMethod]
        public void DictionaryNamespaceSerializationTest()
        {
            const string result = 
@"<CellPhone_DictionaryNamespace xmlns:x1=""http://namespace.org/x1"" xmlns:p1=""namespace/for/prices/only"" xmlns=""http://namespace.org/nsmain"">
  <x1:TheName>HTC</x1:TheName>
  <OS>Windows Phone 8</OS>
  <p1:Prices>
    <p1:KeyValuePairOfColorDouble>
      <p1:Key>Red</p1:Key>
      <p1:Value>120</p1:Value>
    </p1:KeyValuePairOfColorDouble>
    <p1:KeyValuePairOfColorDouble>
      <p1:Key>Blue</p1:Key>
      <p1:Value>110</p1:Value>
    </p1:KeyValuePairOfColorDouble>
    <p1:KeyValuePairOfColorDouble>
      <p1:Key>Black</p1:Key>
      <p1:Value>140</p1:Value>
    </p1:KeyValuePairOfColorDouble>
  </p1:Prices>
</CellPhone_DictionaryNamespace>";
            var serializer = new YAXSerializer(typeof(CellPhone_DictionaryNamespace), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(CellPhone_DictionaryNamespace.GetSampleInstance());
            Assert.AreEqual(result, got);
        }

        [TestMethod]
        public void SingleNamespaceDeserializationTest()
        {            
            var serializer = new YAXSerializer(typeof(SingleNamespaceSample), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string serialized = serializer.Serialize(SingleNamespaceSample.GetInstance());
            var deserialized = serializer.Deserialize(serialized) as SingleNamespaceSample;
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(0, serializer.ParsingErrors.Count);
        }

        [TestMethod]
        public void MultipleNamespaceDeserializationTest()
        {
            var serializer = new YAXSerializer(typeof(MultipleNamespaceSample), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string serialized = serializer.Serialize(MultipleNamespaceSample.GetSampleInstance());
            var deserialized = serializer.Deserialize(serialized) as MultipleNamespaceSample;
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(0, serializer.ParsingErrors.Count);
        }
        
        [TestMethod]
        public void AttributeNamespaceDeserializationTest()
        {
            var serializer = new YAXSerializer(typeof(AttributeNamespaceSample), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(AttributeNamespaceSample.GetSampleInstance());
            var deserialized = serializer.Deserialize(got) as AttributeNamespaceSample;
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(0, serializer.ParsingErrors.Count);
        }

        [TestMethod]
        public void MemberAndClassDifferentNamespacesSerializationTest()
        {
            var serializer = new YAXSerializer(typeof(CellPhone_MemberAndClassDifferentNamespaces), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(CellPhone_MemberAndClassDifferentNamespaces.GetSampleInstance());
            var deserialized = serializer.Deserialize(got) as CellPhone_MemberAndClassDifferentNamespaces;
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(0, serializer.ParsingErrors.Count);
        }

        [TestMethod]
        public void MemberAndClassDifferentNamespacePrefixesDeserializationTest()
        {
            var serializer = new YAXSerializer(typeof(CellPhone_MemberAndClassDifferentNamespacePrefixes), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(CellPhone_MemberAndClassDifferentNamespacePrefixes.GetSampleInstance());
            var deserialized = serializer.Deserialize(got) as CellPhone_MemberAndClassDifferentNamespacePrefixes;
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(0, serializer.ParsingErrors.Count);
        }

        [TestMethod]
        public void MultiLevelMemberAndClassDifferentNamespacesDeserializationTest()
        {
            var serializer = new YAXSerializer(typeof(CellPhone_MultiLevelMemberAndClassDifferentNamespaces), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(CellPhone_MultiLevelMemberAndClassDifferentNamespaces.GetSampleInstance());
            var deserialized = serializer.Deserialize(got) as CellPhone_MultiLevelMemberAndClassDifferentNamespaces;
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(0, serializer.ParsingErrors.Count);
        }

        [TestMethod]
        public void DictionaryNamespaceDeserializationTest()
        {
            var serializer = new YAXSerializer(typeof(CellPhone_DictionaryNamespace), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(CellPhone_DictionaryNamespace.GetSampleInstance());
            var deserialized = serializer.Deserialize(got) as CellPhone_DictionaryNamespace;
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(0, serializer.ParsingErrors.Count);
        }


        [TestMethod]
        public void CSProjParsingTest()
        {
            string csprojContent = @"<Project ToolsVersion=""4.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProductVersion>9.0.30729</ProductVersion>
    <SchemaVersion>2.0</SchemaVersion>
    <ProjectGuid>$guid$</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>$safeprojectname$</RootNamespace>
    <AssemblyName>$safeprojectname$</AssemblyName>
    <TargetFrameworkVersion>v4.0</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <DebugSymbols>False</DebugSymbols>
    <Optimize>False</Optimize>
    <WarningLevel>0</WarningLevel>
  </PropertyGroup>
  <PropertyGroup>
    <DebugSymbols>True</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>False</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
    <DocumentationFile>bin\Debug\$safeprojectname$.xml</DocumentationFile>
  </PropertyGroup>
  <PropertyGroup>
    <DebugSymbols>False</DebugSymbols>
    <DebugType>pdbonly</DebugType>
    <Optimize>True</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""$generatedproject$.EFDAL.Interfaces"">
      <HintPath>..\bin\$generatedproject$.EFDAL.Interfaces.dll</HintPath>
      <SpecificVersion>False</SpecificVersion>
    </Reference>
    <Reference Include=""System"">
      <SpecificVersion>False</SpecificVersion>
    </Reference>
    <Reference Include=""System.Core"">
      <RequiredTargetFramework>3.5</RequiredTargetFramework>
      <SpecificVersion>False</SpecificVersion>
    </Reference>
    <Reference Include=""nHydrate.EFCore, Version=0.0.0.0, Culture=neutral, processorArchitecture=MSIL"">
      <HintPath>..\bin\nHydrate.EFCore.dll</HintPath>
      <SpecificVersion>False</SpecificVersion>
    </Reference>
  </ItemGroup>
  <ItemGroup>
    <Reference Include=""$generatedproject$.EFDAL.Interfaces"">
      <HintPath>..\bin\$generatedproject$.EFDAL.Interfaces.dll</HintPath>
      <SpecificVersion>False</SpecificVersion>
    </Reference>
    <Reference Include=""System"">
      <SpecificVersion>False</SpecificVersion>
    </Reference>
    <Reference Include=""System.Core"">
      <RequiredTargetFramework>3.5</RequiredTargetFramework>
      <SpecificVersion>False</SpecificVersion>
    </Reference>
  </ItemGroup>
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>";

            var project = CsprojParser.Parse(csprojContent);
            string xml2 = CsprojParser.ParseAndRegenerateXml(csprojContent);
            Assert.AreEqual(csprojContent, xml2);
        }
    }
}
