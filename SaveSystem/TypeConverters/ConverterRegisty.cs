using System;
using System.Collections.Generic;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace LazySaveSystem
{
    [AutoStaticsCleanup]
    public static partial class ConverterRegistry
    {
        private static readonly Dictionary<Type, IConverter> converters = new() {};

        public static IConverter GetConverter(Type type)
        {
            if (converters.TryGetValue(type, out var converter))
            {
                return converter;
            }

            return new DefaultConverter();
        }
    }
}