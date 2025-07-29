using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class LocalizationLoader : ILocalizationLoader
{
    public List<LocalizationData> LoadLocalizationData(string localeCode)
    {
        var assetName = $"Localization_{localeCode}";
        var asset = Resources.Load<TextAsset>(assetName);
        
        if (asset == null)
            throw new Exception($"Failed to load localization for \"{localeCode}\": file not found.");

        return JsonConvert.DeserializeObject<List<LocalizationData>>(asset.text);
    }
}