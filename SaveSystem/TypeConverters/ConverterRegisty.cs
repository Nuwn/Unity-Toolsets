using System;
using System.Collections.Generic;
using UnityEngine;

namespace LazySaveSystem
{
    public static class ConverterRegistry
    {
        private static readonly Dictionary<Type, IConverter> converters = new()
            {
            };

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