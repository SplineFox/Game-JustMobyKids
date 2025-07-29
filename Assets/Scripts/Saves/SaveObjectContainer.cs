using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// Обёртка над ISaveObject для сериализации/десериализации.
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public class SaveObjectContainer
{
    [JsonProperty] private string _id;
    [JsonProperty] private string _json;
    
    private bool _deserialized;
    private ISaveObject _saveObject;
    
    public string Id => _id;
    public bool Deserialized => _deserialized;
    public ISaveObject SaveObject => _saveObject;
    
    [JsonConstructor]
    public SaveObjectContainer(string id, string json)
    {
        _id = id;
        _json = json;
        _deserialized = false;
    }
    
    public SaveObjectContainer(string id, ISaveObject saveObject)
    {
        _id = id;
        _saveObject = saveObject;
        _deserialized = true;
    }

    public void Serialize()
    {
        if (_saveObject == null)
            return;
        
        if (_deserialized)
            _json = JsonUtility.ToJson(_saveObject);
    }

    public void Deserialize<T>() where T : ISaveObject
    {
        _saveObject = JsonUtility.FromJson<T>(_json);
        _deserialized = true;
    }
}