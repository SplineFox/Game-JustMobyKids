using Zenject;

public class LocalizationInitializer : IInitializable
{
    private readonly LocalizationConfiguration _configuration;
    private readonly ILocalizationService _localizationService;
    private readonly ISaveProvider _saveProvider;

    public LocalizationInitializer(LocalizationConfiguration configuration, 
        ILocalizationService localizationService, ISaveProvider saveProvider)
    {
        _configuration = configuration;
        _localizationService = localizationService;
        _saveProvider = saveProvider;
    }

    public void Initialize()
    {
    }
}