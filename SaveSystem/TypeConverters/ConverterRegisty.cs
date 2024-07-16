using LazySaveSystem;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class ConverterRegistry
{
    private static readonly Dictionary<Type, IConverter> converters = new()
        {
            { typeof(Texture2D), new Texture2DConverter() }
        };

    public static IConverter GetConverter(Type type)
    {
        if (converters.TryGetValue(type, out var converter))
        {
            return converter;
        }

        Debug.Log($"No converter registered for type {type.FullName}");
        return null;
    }
}