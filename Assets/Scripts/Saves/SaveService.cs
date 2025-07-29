using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class SaveService : ISaveService
{
    private readonly Dictionary<string, SaveObjectContainer> _containers = new();
    private readonly string _savePath = SaveUtils.GetFilePath("saveData.json");

    public void Save()
    {
        try
        {
            var saveData = _containers.Values.ToList();
            File.WriteAllText(_savePath, JsonConvert.SerializeObject(saveData, Formatting.Indented));
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
        }
    }

    public void Load()
    {
        if (!File.Exists(_savePath))
            return;

        try
        {
            var json = File.ReadAllText(_savePath);
            var saveData = new List<SaveObjectContainer>();
            
            JsonConvert.PopulateObject(json, saveData);

            foreach (var save in saveData)
                _containers[save.Id] = save;
        }
        catch (Exception e)
        {
            Debug.LogError($"Load failed: {e.Message}");
        }
    }

    public T GetSaveObject<T>(string id) where T : ISaveObject, new()
    {
        if (!_containers.TryGetValue(id, out var container))
        {
            container = new SaveObjectContainer(id, new T());
            _containers.Add(container.Id, container);
        }
        
        if (!container.Deserialized)
            container.Deserialize<T>();
        
        return (T)container.SaveObject;
    }
}