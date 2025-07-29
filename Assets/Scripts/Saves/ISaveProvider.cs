public interface ISaveProvider
{
    T GetSaveObject<T>(string id) where T : ISaveObject, new();
}