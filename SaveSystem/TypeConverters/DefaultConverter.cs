using System;

namespace LazySaveSystem
{
    internal class DefaultConverter : IConverter
    {
        public string Serialize<T>(T data)
        {
            return Convert.ToBase64String(SaveSystem.SerializeToBase64(data));
        }

        public T Deserialize<T>(string data)
        {
            return (T)SaveSystem.DeserializeFromBase64(data);
        }
    }
}