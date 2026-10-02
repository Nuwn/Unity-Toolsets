using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace LazySaveSystem
{
    /// <summary>
    /// Registers every concrete <see cref="Converter{T}"/> in the loaded assemblies, keyed by the type
    /// argument it was closed over. Deriving from <c>Converter&lt;T&gt;</c> is the whole registration step:
    /// converters never reference each other and this package never references them, so converters live
    /// in whichever assembly owns the type they convert.
    /// </summary>
    // Holds no mutable state: the only static is a readonly Type, and Unity re-invokes the load hook on
    // every play session even with domain reload disabled. Resetting it on exit would only risk nulling
    // converterBase out from under the next session.
    [NoAutoStaticsCleanup]
    public static class ConverterDiscovery
    {
        private static readonly Type converterBase = typeof(Converter<>);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterOnLoad() => DiscoverAndRegister();

        /// <summary>
        /// Every instantiable converter in the loaded assemblies, ordered by name so the build is stable.
        /// </summary>
        public static IReadOnlyList<Type> FindConverterTypes()
        {
            var found = new List<Type>();

            foreach (var assembly in GetLoadedAssemblies())
            {
                foreach (var type in GetTypes(assembly))
                {
                    if (type == null || !type.IsClass || type.IsAbstract || type.ContainsGenericParameters)
                        continue;

                    if (GetTargetType(type) == null)
                        continue;

                    if (GetParameterlessConstructor(type) == null)
                    {
                        Debug.LogWarning(
                            $"[SaveSystem] '{type.FullName}' is a converter but has no parameterless " +
                            "constructor, so it cannot be discovered. Add one or register it manually.");
                        continue;
                    }

                    found.Add(type);
                }
            }

            found.Sort((a, b) => string.CompareOrdinal(a.FullName, b.FullName));
            return found;
        }

        /// <summary>
        /// The <c>T</c> a converter was closed over, or null when the type is not a converter.
        /// </summary>
        public static Type GetTargetType(Type converterType)
        {
            for (var type = converterType.BaseType; type != null; type = type.BaseType)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == converterBase)
                    return type.GetGenericArguments()[0];
            }

            return null;
        }

        /// <summary>
        /// Discovered converters grouped by the target type they convert, ordered by target name. Each
        /// group is ordered by converter name, so the first entry is the one that would win registration.
        /// </summary>
        public static IReadOnlyList<(Type Target, IReadOnlyList<Type> Converters)> FindConvertersByTargetType()
        {
            var byTarget = new Dictionary<Type, List<Type>>();

            foreach (var converterType in FindConverterTypes())
            {
                var targetType = GetTargetType(converterType);

                if (!byTarget.TryGetValue(targetType, out var converters))
                {
                    converters = new List<Type>();
                    byTarget[targetType] = converters;
                }

                converters.Add(converterType);
            }

            return byTarget
                .OrderBy(pair => pair.Key.FullName, StringComparer.Ordinal)
                .Select(pair => (Target: pair.Key, Converters: (IReadOnlyList<Type>)pair.Value))
                .ToList();
        }

        /// <summary>
        /// The target types that more than one discovered converter claims. A duplicate is not an error,
        /// but it means one converter is silently unused, so it is worth reporting.
        /// </summary>
        public static IReadOnlyList<(Type Target, IReadOnlyList<Type> Converters)> GetConverterConflicts() =>
            FindConvertersByTargetType().Where(entry => entry.Converters.Count > 1).ToList();

        /// <summary>
        /// Registers any converter whose target has no converter yet. Anything already registered wins, so
        /// an explicit <c>ConverterRegistry.Register</c> always overrides discovery. When two converters
        /// claim the same target, the one that sorts first by name is used and the ambiguity is warned about.
        /// </summary>
        /// <returns>How many converters were newly registered.</returns>
        public static int DiscoverAndRegister()
        {
            var registered = 0;

            foreach (var (target, converters) in FindConvertersByTargetType())
            {
                if (converters.Count > 1)
                {
                    Debug.LogWarning(
                        $"[SaveSystem] {converters.Count} converters claim '{target.FullName}': " +
                        $"{string.Join(", ", converters.Select(type => type.Name))}. " +
                        $"'{converters[0].Name}' is used because discovery is ordered by name. " +
                        "Drop the duplicate, or register the one you want explicitly.");
                }

                if (ConverterRegistry.IsRegistered(target))
                    continue;

                if (Activator.CreateInstance(converters[0], true) is not IConverter converter)
                    continue;

                ConverterRegistry.Register(target, converter);
                registered++;
            }

            return registered;
        }

        private static IEnumerable<Assembly> GetLoadedAssemblies()
        {
#if UNITY_6000_5_OR_NEWER
            return UnityEngine.Assemblies.CurrentAssemblies.GetLoadedAssemblies();
#else
            return AppDomain.CurrentDomain.GetAssemblies();
#endif
        }

        private static IEnumerable<Type> GetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                // A partially loaded assembly still yields the types that did resolve.
                return e.Types.Where(type => type != null);
            }
        }

        private static ConstructorInfo GetParameterlessConstructor(Type type) => type.GetConstructor(
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            binder: null,
            types: Type.EmptyTypes,
            modifiers: null);
    }
}
