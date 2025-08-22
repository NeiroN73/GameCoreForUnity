#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GameCore.Utils
{
    public static class TypeExtensions
    {
        public static bool InheritsOrImplements(this Type type, Type baseType)
        {
            type = ResolveGenericType(type);
            baseType = ResolveGenericType(baseType);

            while (type != typeof(object))
            {
                if (baseType == type || HasAnyInterfaces(type, baseType)) return true;

                type = ResolveGenericType(type.BaseType);
                if (type == null) return false;
            }

            return false;
        }
        
        public static string[] FilterTypes<TFilter>() where TFilter : class
        {
            var targetAssemblies = GetTargetAssemblies();
            var filteredTypes = targetAssemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => DefaultFilter(t, typeof(TFilter)))
                .OrderBy(x => x.Name)
                .ToList();
    
            return filteredTypes.Select(t => 
                    t.ReflectedType == null ? 
                        t.FullName : 
                        $"{t.ReflectedType.FullName}.{t.Name}")
                .ToArray();
        }

        public static Type GetType(string fullTypeName)
        {
            if (string.IsNullOrEmpty(fullTypeName))
                return null;
            
            var targetAssemblies = GetTargetAssemblies();
            foreach (var assembly in targetAssemblies)
            {
                var type = assembly.GetType(fullTypeName);
                if (type != null)
                {
                    return type;
                }
            }

            return default;
        }

        private static Type ResolveGenericType(Type type)
        {
            if (type is not {IsGenericType: true}) return type;

            var genericType = type.GetGenericTypeDefinition();
            return genericType != type ? genericType : type;
        }

        private static bool HasAnyInterfaces(Type type, Type interfaceType)
        {
            return type.GetInterfaces().Any(i => ResolveGenericType(i) == interfaceType);
        }
        
        private static IEnumerable<Assembly> GetTargetAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var targetAssemblies = assemblies
                .Where(x => x.GetTypes().Any(t => 
                    x.FullName.StartsWith("Assembly-CSharp") || x.FullName.StartsWith("GameCore")));
            return targetAssemblies;
        }
        
        private static bool DefaultFilter(Type type, Type filterType)
        {
            return !type.IsAbstract && !type.IsInterface && !type.IsGenericType && type.InheritsOrImplements(filterType);
        }
    }
}
#endif
