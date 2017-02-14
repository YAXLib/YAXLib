using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Reflection;
using NUnit.Framework;
using YAXLib;

namespace YAXLibTests
{
	[TestFixture]
	internal class TestMethodLister
	{
		[Test]
		public void GetAllUnitTests()
		{
#if FXCORE
			var assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(GetCurrentyAssemblyDllPath());
#else
			var assembly = System.Reflection.Assembly.LoadFile(GetCurrentyAssemblyDllPath());
#endif
			//get testfixture classes in assembly.
			var testTypes = from t in assembly.GetTypes()
							let attributes = t.GetCustomAttributes(typeof(NUnit.Framework.TestFixtureAttribute), true)
							where attributes != null && attributes.Length > 0
							orderby t.Name
							select t;

			var sb = new StringBuilder();
			foreach (var type in testTypes)
			{
				//get test methods of assembly.
#if FXCORE
				var testMethods = from m in ReflectionBridgeExtensions.GetMethods(type)
					let attributes = m.CustomAttributes.Where(a => a.AttributeType == typeof(TestAttribute)).ToArray()
#else
				var testMethods = from m in type.GetMethods()
					let attributes = m.GetCustomAttributes(typeof(TestAttribute), true)
#endif
					where attributes != null && attributes.Length > 0
					orderby m.Name
					select m;
				
				foreach (var method in testMethods)
				{
					sb.AppendLine($"{type.Name},{method.Name}");
				}
			}
			Console.Write(sb.ToString());
		}

		private string GetCurrentyAssemblyDllPath()
		{
#if FXCORE
			var codeBase = typeof(TestMethodLister).GetTypeInfo().Assembly.CodeBase;
#else
			var codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
#endif
			var uri = new UriBuilder(codeBase);
			var path = Uri.UnescapeDataString(uri.Path);
			return path;
			//return System.IO.Path.GetDirectoryName(path);
		}
	}
}
