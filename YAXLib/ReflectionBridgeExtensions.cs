#define REFLECTIONBRIDGE

using System;
using System.Reflection;
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
using Microsoft.Extensions.DependencyModel;
#endif

#if REFLECTIONBRIDGE
using System.Collections.Generic;
using System.Linq;
#endif

namespace YAXLib
{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
	/// <summary>
	/// Credits Michael Whelan. His source code is published on 
	/// http://www.michael-whelan.net/replacing-appdomain-in-dotnet-core/
	/// </summary>
	public class AppDomain
	{
		public static AppDomain CurrentDomain { get; private set; }

		static AppDomain()
		{
			CurrentDomain = new AppDomain();
		}

		public Assembly[] GetAssemblies()
		{
			var assemblies = new List<Assembly>();
			var dependencies = DependencyContext.Default.RuntimeLibraries;
			foreach (var library in dependencies)
			{
				if (IsCandidateCompilationLibrary(library))
				{
					var assembly = Assembly.Load(new AssemblyName(library.Name));
					assemblies.Add(assembly);
				}
			}
			return assemblies.ToArray();
		}

		private static bool IsCandidateCompilationLibrary(RuntimeLibrary compilationLibrary)
		{
			return compilationLibrary.Name == nameof(YAXLib)
				|| compilationLibrary.Dependencies.Any(d => d.Name.StartsWith(nameof(YAXLib)));
		}
	}
#endif

	public static class ReflectionBridgeExtensions
	{
		public static Assembly GetAssembly(this Type type)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			return type.GetTypeInfo().Assembly;
#else
            return type.Assembly;
#endif
		}

		public static bool IsSealed(this Type type)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			return type.GetTypeInfo().IsSealed;
#else
            return type.IsSealed;
#endif
		}

		public static bool IsAbstract(this Type type)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			return type.GetTypeInfo().IsAbstract;
#else
            return type.IsAbstract;
#endif
		}

		public static bool IsEnum(this Type type)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
		}

		public static bool IsClass(this Type type)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			return type.GetTypeInfo().IsClass;
#else
            return type.IsClass;
#endif
		}

		public static bool IsPrimitive(this Type type)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			return type.GetTypeInfo().IsPrimitive;
#else
            return type.IsPrimitive;
#endif
		}

		public static bool IsPublic(this Type type)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			return type.GetTypeInfo().IsPublic;
#else
            return type.IsPublic;
#endif
		}

		public static bool IsNestedPublic(this Type type)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			return type.GetTypeInfo().IsNestedPublic;
#else
            return type.IsNestedPublic;
#endif
		}

		public static bool IsFromLocalAssembly(this Type type)
		{
#if SILVERLIGHT
            string assemblyName = type.GetAssembly().FullName;
#else
			string assemblyName = type.GetAssembly().GetName().FullName;
#endif

			try
			{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
				Assembly.Load(new AssemblyName { Name = assemblyName });
#else
                Assembly.Load(assemblyName);
#endif
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool IsGenericType(this Type type)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
		}

		public static bool IsGenericTypeDefinition(this Type type)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			return type.GetTypeInfo().IsGenericTypeDefinition;
#else
            return type.IsGenericTypeDefinition;
#endif
		}

		public static bool IsInterface(this Type type)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			return type.GetTypeInfo().IsInterface;
#else
            return type.IsInterface;
#endif
		}

		public static Type BaseType(this Type type)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			return type.GetTypeInfo().BaseType;
#else
            return type.BaseType;
#endif
		}

		public static bool IsValueType(this Type type)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
		}

		public static T GetPropertyValue<T>(this Type type, string propertyName, object target)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			PropertyInfo property = type.GetTypeInfo().GetProperty(propertyName, typeof(T));
			return (T)property.GetValue(target);
#else
            return (T) type.InvokeMember(propertyName, BindingFlags.GetProperty, null, target, null);
#endif
		}

		public static  PropertyInfo GetProperty(this Type type, string name, Type returnType)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			return type.GetTypeInfo().GetProperty(name, returnType);
#else
            return type.GetProperty(name, returnType);
#endif
		}

		public static PropertyInfo GetProperty(this Type type, string name, Type[] types)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			return type.GetTypeInfo().GetProperty(name, types);
#else
            return type.GetProperty(name, types);
#endif
		}


		public static void SetPropertyValue(this Type type, string propertyName, object target, object value)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			PropertyInfo property = type.GetTypeInfo().GetProperty(propertyName);
			property.SetValue(target, value);
#else
            type.InvokeMember(propertyName, BindingFlags.SetProperty, null, target, new object[] { value });
#endif
		}

		public static void SetFieldValue(this Type type, string fieldName, object target, object value)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			FieldInfo field = type.GetTypeInfo().GetField(fieldName);
			field.SetValue(target, value);
#else
            type.InvokeMember(fieldName, BindingFlags.SetField, null, target, new object[] { value });
#endif
		}

		public static object InvokeMethod<T>(this Type type, string methodName, object target, T value)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			MethodInfo method = type.GetTypeInfo().GetMethod(methodName, new Type[] {value?.GetType()});
			return method.Invoke(target, new object[] { value });
#else
            return type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, target, new object[] {value});
#endif
		}

		public static object InvokeMethod(this Type type, string methodName, object target, object[] arg)
		{
#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
			Type[] argTypes = null;
			if (arg != null)
			{
				argTypes = new Type[arg.Length];
				for (var i = 0; i < argTypes.Length; i++)
				{
					argTypes[i] = arg[i].GetType();
				}
			}
			MethodInfo method = type.GetTypeInfo().GetMethod(methodName, argTypes);
			return method.Invoke(target, arg);
#else
			return type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, target, arg);
#endif
		}

#if REFLECTIONBRIDGE && (!(NET40 || NET35 || NET20 || SILVERLIGHT))
		public static IEnumerable<MethodInfo> GetMethods(this Type someType)
		{
			var t = someType;
			while (t != null)
			{
				var ti = t.GetTypeInfo();
				foreach (var m in ti.GetMethods())
					yield return m;
				t = ti.BaseType;
			}
		}
		/*
		public static Type[] GetGenericArguments(this Type type)
		{
			return type.GetTypeInfo().GenericTypeArguments;
		}
		*/
		/*
        public static bool IsAssignableFrom(this Type type, Type otherType)
        {
            return type.GetTypeInfo().IsAssignableFrom(otherType.GetTypeInfo());
        }*/

		public static bool IsSubclassOf(this Type type, Type c)
		{
			return type.GetTypeInfo().IsSubclassOf(c);
		}

		public static Attribute[] GetCustomAttributes(this Type type)
		{
			return type.GetTypeInfo().GetCustomAttributes().ToArray();
		}

		public static Attribute[] GetCustomAttributes(this Type type, bool inherit)
		{
			return type.GetTypeInfo().GetCustomAttributes(inherit).ToArray();
		}

		public static Attribute[] GetCustomAttributes(this Type type, Type attributeType, bool inherit)
		{
			return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit).Cast<Attribute>().ToArray();
		}
#endif
	}
}
