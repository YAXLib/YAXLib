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
            string got = serializer.Serialize(MultipleNamespaceSample.GetInstance());
            Assert.AreEqual(result, got);
        }

        [TestMethod]
        public void AttributeNamespaceSerializationTest()
        {
            const string result = "<AttributeNamespaceSample xmlns:ns=\"http://namespaces.org/ns\" xmlns=\"http://namespaces.org/default\">" + @"
  <Attribs " + "attrib=\"value\" ns:attrib2=\"value2\"" + @" />
</AttributeNamespaceSample>";

            var serializer = new YAXSerializer(typeof(AttributeNamespaceSample), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(AttributeNamespaceSample.GetInstance());
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
            string serialized = serializer.Serialize(MultipleNamespaceSample.GetInstance());
            var deserialized = serializer.Deserialize(serialized) as MultipleNamespaceSample;
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(0, serializer.ParsingErrors.Count);
        }
        
        [TestMethod]
        public void AttributeNamespaceDeserializationTest()
        {
            var serializer = new YAXSerializer(typeof(AttributeNamespaceSample), YAXExceptionHandlingPolicies.DoNotThrow, YAXExceptionTypes.Warning, YAXSerializationOptions.SerializeNullObjects);
            string got = serializer.Serialize(AttributeNamespaceSample.GetInstance());
            var deserialized = serializer.Deserialize(got) as AttributeNamespaceSample;
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(0, serializer.ParsingErrors.Count);
        }

        [TestMethod]
        public void CSProjParsingTest()
        {
            string csprojContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""4.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
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
	</PropertyGroup>
	<PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
		<DebugSymbols>true</DebugSymbols>
		<DebugType>full</DebugType>
		<Optimize>false</Optimize>
		<OutputPath>bin\Debug\</OutputPath>
		<DefineConstants>DEBUG;TRACE</DefineConstants>
		<ErrorReport>prompt</ErrorReport>
		<WarningLevel>4</WarningLevel>
		<DocumentationFile>bin\Debug\$safeprojectname$.xml</DocumentationFile>
	</PropertyGroup>
	<PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
		<DebugType>pdbonly</DebugType>
		<Optimize>true</Optimize>
		<OutputPath>bin\Release\</OutputPath>
		<DefineConstants>TRACE</DefineConstants>
		<ErrorReport>prompt</ErrorReport>
		<WarningLevel>4</WarningLevel>
	</PropertyGroup>
	<ItemGroup>
		<Reference Include=""$generatedproject$.EFDAL.Interfaces"">
			<HintPath>..\bin\$generatedproject$.EFDAL.Interfaces.dll</HintPath>
		</Reference>
		<Reference Include=""System"" />
		<Reference Include=""System.Core"">
			<RequiredTargetFramework>3.5</RequiredTargetFramework>
		</Reference>
		<Reference Include=""nHydrate.EFCore, Version=0.0.0.0, Culture=neutral, processorArchitecture=MSIL"">
			<SpecificVersion>False</SpecificVersion>
			<HintPath>..\bin\nHydrate.EFCore.dll</HintPath>
		</Reference>
	</ItemGroup>
	<ItemGroup>
		<Reference Include=""$generatedproject$.EFDAL.Interfaces"">
			<HintPath>..\bin\$generatedproject$.EFDAL.Interfaces.dll</HintPath>
		</Reference>
		<Reference Include=""System"" />
		<Reference Include=""System.Core"">
			<RequiredTargetFramework>3.5</RequiredTargetFramework>
		</Reference>
	</ItemGroup>
	<Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>
";

            var project = CsprojParser.Parse(csprojContent);

            string xml2 = CsprojParser.ParseAndRegenerateXml(csprojContent);

            Console.WriteLine(xml2);

            Assert.AreEqual(csprojContent, xml2);
        }
    }
}
