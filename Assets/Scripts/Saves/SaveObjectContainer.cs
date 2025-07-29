using System;
using UnityEngine;

/// <summary>
/// Обёртка над ISaveObject для сериализации/десериализации.
/// </summary>
[Serializable]
public class SaveObjectContainer
{
    [SerializeField] private string _id;
    [SerializeField] private string _json;
    
    [NonSerialized] private bool _deserialized;
    [NonSerialized] private ISaveObject _saveObject;
    
    public string Id => _id;
    public bool Deserialized => _deserialized;
    public ISaveObject SaveObject => _saveObject;
    
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