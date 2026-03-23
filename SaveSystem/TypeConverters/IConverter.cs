namespace LazySaveSystem
{
    public interface IConverter
    {
        string Serialize<T>(T data);
        T Deserialize<T>(string data);
    }
}