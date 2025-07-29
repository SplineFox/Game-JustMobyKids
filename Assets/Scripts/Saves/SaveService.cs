using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveService : ISaveService
{
    private Dictionary<string, ISaveObject> _saveObjects = new();
    private string _savePath;

    public SaveService()
    {
        _savePath = SaveUtils.GetFilePath("saveData.json");
    }
    
    public void Save()
    {
        try
        {
            var saveData = new Dictionary<string, string>();
            foreach (var obj in _saveObjects)
            {
                saveData[obj.Key] = JsonUtility.ToJson(obj.Value);
            }
            var json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(_savePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save data: {e.Message}");
        }
    }

    public void Load()
    {
        try
        {
            if (!File.Exists(_savePath))
                return;
            
            var json = File.ReadAllText(_savePath);
            var saveData = JsonUtility.FromJson<Dictionary<string, string>>(json);
                
            foreach (var pair in saveData)
            {
                if (!_saveObjects.ContainsKey(pair.Key))
                    continue;
                        
                JsonUtility.FromJsonOverwrite(pair.Value, _saveObjects[pair.Key]);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data: {e.Message}");
        }
    }

    public T GetSaveObject<T>(string objectId) where T : ISaveObject, new()
    {
        if (!_saveObjects.ContainsKey(objectId))
            _saveObjects[objectId] = new T();
        
        return (T)_saveObjects[objectId];
    }
}