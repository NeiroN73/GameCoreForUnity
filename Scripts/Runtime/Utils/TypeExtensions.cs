using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace GameCore.Utils
{
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, string[]> TypeCache = new();
        private static readonly ConcurrentDictionary<string, Type> LoadedTypes = new();
        private static Assembly[] _cachedAssemblies;
        private static DateTime _lastAssemblyCacheTime;
        private static readonly TimeSpan AssemblyCacheRefreshInterval = TimeSpan.FromSeconds(5);

        public static bool InheritsOrImplements(this Type type, Type baseType)
        {
            type = ResolveGenericType(type);
            baseType = ResolveGenericType(baseType);

            while (type != typeof(object))
            {
                if (baseType == type || HasAnyInterfaces(type, baseType))
                    return true;

                type = ResolveGenericType(type.BaseType);
                if (type == null)
                    return false;
            }

            return false;
        }

        public static string[] FilterTypes<TFilter>() where TFilter : class
        {
            var filterType = typeof(TFilter);
            
            if (TypeCache.TryGetValue(filterType, out var cachedTypes))
                return cachedTypes;

            var targetAssemblies = GetTargetAssemblies();
            var filteredTypes = new List<Type>();

            foreach (var assembly in targetAssemblies)
            {
                Type[] assemblyTypes;
                try
                {
                    assemblyTypes = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException)
                {
                    continue;
                }

                foreach (var type in assemblyTypes)
                {
                    if (DefaultFilter(type, filterType))
                    {
                        filteredTypes.Add(type);
                    }
                }
            }

            filteredTypes.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));

            var result = new string[filteredTypes.Count];
            for (int i = 0; i < filteredTypes.Count; i++)
            {
                result[i] = filteredTypes[i].FullName;
            }

            TypeCache[filterType] = result;
            return result;
        }

        public static Type GetType(string fullTypeName)
        {
            if (string.IsNullOrEmpty(fullTypeName))
                return null;
            
            if (LoadedTypes.TryGetValue(fullTypeName, out var cachedType))
                return cachedType;

            var targetAssemblies = GetTargetAssemblies();
            foreach (var assembly in targetAssemblies)
            {
                var type = assembly.GetType(fullTypeName);
                if (type != null)
                {
                    LoadedTypes[fullTypeName] = type;
                    return type;
                }
            }

            LoadedTypes[fullTypeName] = null;
            return null;
        }

        private static Type ResolveGenericType(Type type)
        {
            if (!type.IsGenericType) 
                return type;
            
            var genericType = type.GetGenericTypeDefinition();
            return genericType != type ? genericType : type;
        }

        private static bool HasAnyInterfaces(Type type, Type interfaceType)
        {
            var interfaces = type.GetInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                if (ResolveGenericType(interfaces[i]) == interfaceType)
                    return true;
            }
            return false;
        }

        private static Assembly[] GetTargetAssemblies()
        {
            var now = DateTime.Now;
            if (_cachedAssemblies != null && (now - _lastAssemblyCacheTime) < AssemblyCacheRefreshInterval)
                return _cachedAssemblies;

            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var filtered = new List<Assembly>();
            
            foreach (var assembly in allAssemblies)
            {
                var fullName = assembly.FullName;
                if (fullName != null && (fullName.StartsWith("Assembly-CSharp") || fullName.StartsWith("GameCore")))
                {
                    filtered.Add(assembly);
                }
            }

            _cachedAssemblies = filtered.ToArray();
            _lastAssemblyCacheTime = now;
            return _cachedAssemblies;
        }

        private static bool DefaultFilter(Type type, Type filterType)
        {
            if (type.IsAbstract || type.IsInterface || type.IsGenericType)
                return false;
                
            return type.InheritsOrImplements(filterType);
        }
    }
}