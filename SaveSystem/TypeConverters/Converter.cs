using System;

namespace LazySaveSystem
{
    public abstract class Converter<T> : IConverter
    {
        public Type TargetType => typeof(T);

        public abstract string Serialize(T data);

        public abstract T Deserialize(string data);

        public string Serialize<T1>(T1 data) => Serialize((T)(object)data);

        public T1 Deserialize<T1>(string data) => (T1)(object)Deserialize(data);
    }
}
