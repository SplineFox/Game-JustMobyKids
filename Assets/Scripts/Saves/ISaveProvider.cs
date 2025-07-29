public interface ISaveProvider
{
    T GetSaveObject<T>(string objectId) where T : ISaveObject, new();
}