using System;
using UnityEngine;
using Zenject;
using UniRx;

public class SaveInvoker : IInitializable, IDisposable
{
    private bool _isInitialized;
    private ISaveService _saveService;
    private CompositeDisposable _disposables = new();

    public SaveInvoker(ISaveService saveService)
    {
        _saveService = saveService;
    }

    public void Initialize()
    {
        _saveService.Load();
        _isInitialized = true;

        Observable.EveryApplicationFocus()
            .Where(hasFocus => hasFocus == false && _isInitialized)
            .Subscribe(_ => _saveService.Save())
            .AddTo(_disposables);
        
        Observable.EveryApplicationPause()
            .Where(isPaused => isPaused == true && _isInitialized)
            .Subscribe(_ => _saveService.Save())
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables?.Dispose();
    }
}