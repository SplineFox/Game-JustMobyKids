public interface ILocalizationService
{
    string CurrentLocaleCode { get; }
    
    void SetLocaleCode(string localeCode);
    string GetText(string id);
    bool HasText(string id);
}