using System;
using System.Linq;
using System.Collections.Generic;

public class LocalizationService : ILocalizationService
{
    public event Action LocaleChanged;
    
    private readonly ILocalizationLoader _localizationLoader;
    private Dictionary<string, string> _localizedTexts = new();

    public string CurrentLocaleCode { get; private set; }

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
        if (CurrentLocaleCode == localeCode)
            return;
        
        CurrentLocaleCode = localeCode;
        var localizationData = _localizationLoader.LoadLocalizationData(localeCode);
        _localizedTexts = localizationData.ToDictionary(k => k.Id, v => v.Text);
        
        LocaleChanged?.Invoke();
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