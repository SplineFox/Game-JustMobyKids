using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;

public class LocalizationService : ILocalizationService
{
    private readonly ILocalizationLoader _localizationLoader;
    private readonly ReactiveProperty<string> _currentLocaleCode = new();
    private Dictionary<string, string> _localizedTexts = new();

    public IReadOnlyReactiveProperty<string> CurrentLocaleCode => _currentLocaleCode;
    
    public LocalizationService(ILocalizationLoader localizationLoader)
    {
        _localizationLoader = localizationLoader;
    }

    public void Initialize(string localeCode)
    {
        SetLocaleCode(localeCode);
    }

    public void SetLocaleCode(string localeCode)
    {
        if (_currentLocaleCode.Value == localeCode)
            return;
        
        var localizationData = _localizationLoader.LoadLocalizationData(localeCode);
        
        _localizedTexts = localizationData.ToDictionary(k => k.Id, v => v.Text);
        _currentLocaleCode.Value = localeCode;
    }

    public string GetText(string id)
    {
        return _localizedTexts.TryGetValue(id, out var text) 
            ? text 
            : string.Empty;
    }

    public bool HasText(string id)
    {
        return _localizedTexts.ContainsKey(id);
    }
}