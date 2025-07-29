using UniRx;
using Zenject;

public class SaveInvoker : IInitializable
{
    private ISaveService _saveService;
    private CompositeDisposable _disposables;

    public SaveInvoker(ISaveService saveService)
    {
        _saveService = saveService;
    }
    
    public void Initialize()
    {
        _saveService.Load();

        Observable.EveryApplicationFocus()
            .Where(hasFocus => !hasFocus)
            .Do(_ => _saveService.Save());
        
        Observable.EveryApplicationPause()
            .Where(isPaused => isPaused)
            .Do(_ => _saveService.Save());
    }
}