public interface IConverter
{
    object Serialize(object data);
    object Deserialize(object data);
}