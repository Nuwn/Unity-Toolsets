namespace LazySaveSystem
{
    public abstract class Converter<T> : IConverter
    {
        public abstract string Serialize(T data);
        public abstract T Deserialize(string data);

        public string Serialize<T1>(T1 data) => data is T typedData ? Serialize(typedData) : null;

        public T1 Deserialize<T1>(string data) => Deserialize<T1>(data);
    }
}