using System;
using UniRx;

public interface ILocalizationService
{
    IReadOnlyReactiveProperty<string> CurrentLocaleCode { get; }
    
    void Initialize(string localeCode);
    void SetLocaleCode(string localeCode);
    string GetText(string id);
    bool HasText(string id);
}