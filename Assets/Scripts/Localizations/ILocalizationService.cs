using System;

public interface ILocalizationService
{
    event Action LocaleChanged;
    
    string CurrentLocaleCode { get; }
    
    void Initialize(string localeCode);
    void SetLocaleCode(string localeCode);
    string GetText(string id);
    bool HasText(string id);
}