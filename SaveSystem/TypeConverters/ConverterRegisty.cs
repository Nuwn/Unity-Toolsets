using System;
using System.Collections.Generic;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace LazySaveSystem
{
    [AutoStaticsCleanup]
    public static partial class ConverterRegistry
    {
        private static readonly Dictionary<Type, IConverter> converters = new() { };
        private static readonly HashSet<Type> reportedMissing = new();

        public static IConverter GetConverter(Type type)
        {
            if (converters.TryGetValue(type, out var converter))
            {
                return converter;
            }

            ReportMissing(type);
            return new DefaultConverter();
        }

        public static void Register<T>(IConverter converter) => Register(typeof(T), converter);

        public static void Register(Type type, IConverter converter) => converters[type] = converter;

        public static void Unregister<T>() => Unregister(typeof(T));

        public static void Unregister(Type type) => converters.Remove(type);

        public static bool IsRegistered<T>() => IsRegistered(typeof(T));

        public static bool IsRegistered(Type type) => converters.ContainsKey(type);

        private static void ReportMissing(Type type)
        {
            if (!reportedMissing.Add(type))
                return;

            Debug.LogWarning(
                $"[SaveSystem] No converter registered for '{type.FullName}'. Falling back to JsonUtility, " +
                "which silently drops readonly fields, properties and dictionaries. Add one by deriving from " +
                $"Converter<{type.Name}>; it is registered automatically.");
        }
    }
}
