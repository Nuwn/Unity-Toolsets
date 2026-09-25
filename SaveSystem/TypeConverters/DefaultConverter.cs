using UnityEngine;

namespace LazySaveSystem
{
    internal class DefaultConverter : IConverter
    {
        public string Serialize<T>(T data)
        {
            return JsonUtility.ToJson(data);
        }

        public T Deserialize<T>(string data)
        {
            return JsonUtility.FromJson<T>(data);
        }
    }
}