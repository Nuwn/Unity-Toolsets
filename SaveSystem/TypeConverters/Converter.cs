namespace LazySaveSystem
{
    public abstract class Converter<T> : IConverter
    {
        public abstract object Serialize(T data);
        public abstract T Deserialize(object data);

        public object Serialize(object data) => data is T typedData ? Serialize(typedData) : null;

        object IConverter.Deserialize(object data) => Deserialize(data);
    }
}