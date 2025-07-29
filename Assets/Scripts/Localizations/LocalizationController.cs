using System;
using Zenject;

public class LocalizationController : IInitializable, IDisposable
{
    private readonly LocalizationConfiguration _configuration;
    private readonly ILocalizationService _localizationService;
    private readonly ISaveProvider _saveProvider;
    
    private LocalizationSave _save;

    public LocalizationController(LocalizationConfiguration configuration,
        ILocalizationService localizationService, ISaveProvider saveProvider)
    {
        _configuration = configuration;
        _localizationService = localizationService;
        _saveProvider = saveProvider;
    }

    public void Initialize()
    {
        _save = _saveProvider.GetSaveObject<LocalizationSave>("localization");
        
        var localeCode = string.IsNullOrEmpty(_save.LocaleCode)
            ? _configuration.DefaultLocaleCode
            : _save.LocaleCode;

        _localizationService.Initialize(localeCode);
        _localizationService.LocaleChanged += OnLocaleChanged;
    }

    public void Dispose()
    {
        _localizationService.LocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged()
    {
        _save.LocaleCode = _localizationService.CurrentLocaleCode;
    }
}