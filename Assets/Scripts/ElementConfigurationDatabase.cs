using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementConfigurationDatabase", menuName = "JustMobyKids/ElementConfigurationDatabase")]
public class ElementConfigurationDatabase : ScriptableObject
{
    [SerializeField] private List<ElementConfiguration> _configurations;

    public ElementConfiguration GetConfiguration(string id)
    {
        var configuration = _configurations.FirstOrDefault(configuration => configuration.Id == id);
        if (configuration == null)
            throw new Exception($"Failed to get configuration with id \"{id}\" in \"{name}\".");
        
        return configuration;
    }
}