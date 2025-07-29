using System.Collections.Generic;

public interface ILocalizationLoader
{
    List<LocalizationData> LoadLocalizationData(string localeCode);
}