using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationConfiguration", menuName = "JustMobyKids/LocalizationConfiguration")]
public class LocalizationConfiguration : ScriptableObject
{
    [SerializeField] private string _defaultLocaleCode;
    [SerializeField] private List<string> _localeCodes;
    
    public string DefaultLocaleCode => _defaultLocaleCode;
    public IReadOnlyList<string> LocaleCodes => _localeCodes;
}