using System;
using UniRx;
using Zenject;
using UnityEngine;

public class Slot : ElementContainer
{
    [SerializeField] private RectTransform _rectTransform;
    
    private readonly Subject<Unit> _onInteractionBegin = new();
    private readonly Subject<Unit> _onInteractionEnd = new();
    private readonly CompositeDisposable _elementSubscriptions = new();
    
    private ElementConfiguration _elementConfiguration;
    private ElementPool _elementPool;
    private Element _element;
    
    public IObservable<Unit> OnInteractionBegin => _onInteractionBegin;
    public IObservable<Unit> OnInteractionEnd => _onInteractionEnd;

    [Inject]
    public void Construct(ElementConfiguration elementConfiguration, ElementPool elementPool)
    {
        _elementConfiguration = elementConfiguration;
        _elementPool = elementPool;
    }

    private void Start()
    {
        SpawnElement(false);
    }

    public override void AddElement(Element element)
    {
        _element = element;
        _element.SetContainer(this);
        _element.CanBeDestroyed = false;
        
        _elementSubscriptions.Clear();
        
        _element.OnDragBegin
            .Subscribe(_ => _onInteractionBegin.OnNext(Unit.Default))
            .AddTo(_elementSubscriptions);

        _element.OnDragEnd
            .Subscribe(_ => _onInteractionEnd.OnNext(Unit.Default))
            .AddTo(_elementSubscriptions);
        
        FitElementToSlot(element);
    }

    public override void RemoveElement(Element element)
    {
        if (_element != element)
            return;

        _element = null;
        _elementSubscriptions.Clear();
        
        SpawnElement(true);
    }

    private void SpawnElement(bool shouldAnimate = false)
    {
        var element = _elementPool.Spawn(_elementConfiguration);
        AddElement(element);
        
        if(shouldAnimate)
            element.PlayAppearAnimation();
    }

    private void FitElementToSlot(Element element)
    {
        element.RectTransform.SetParent(_rectTransform);
        
        element.RectTransform.offsetMin = Vector2.zero;
        element.RectTransform.offsetMax = Vector2.zero;
        
        element.RectTransform.anchorMin = Vector2.one * 0.5f;
        element.RectTransform.anchorMax = Vector2.one * 0.5f;
        
        element.RectTransform.localScale = Vector3.one;
        element.RectTransform.sizeDelta = _rectTransform.sizeDelta;
    }
}