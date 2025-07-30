using UniRx;
using Zenject;
using UnityEngine;

public class Slot : ElementContainer
{
    public readonly Subject<Unit> OnInteractionBegin = new();
    public readonly Subject<Unit> OnInteractionEnd = new();
    
    [SerializeField] private RectTransform _rectTransform;
    
    private CompositeDisposable _elementSubscriptions = new();
    private ElementConfiguration _elementConfiguration;
    private ElementPool _elementPool;
    private Element _element;

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
        
        _element.RectTransform.SetParent(_rectTransform);
        
        _element.RectTransform.offsetMin = Vector2.zero;
        _element.RectTransform.offsetMax = Vector2.zero;
        
        _element.RectTransform.anchorMin = Vector2.one * 0.5f;
        _element.RectTransform.anchorMax = Vector2.one * 0.5f;
        
        _element.RectTransform.localScale = Vector3.one;
        _element.RectTransform.sizeDelta = _rectTransform.sizeDelta;
        
        _elementSubscriptions.Clear();
        
        _element.OnDragBegin
            .Subscribe(_ => OnInteractionBegin.OnNext(Unit.Default))
            .AddTo(_elementSubscriptions);

        _element.OnDragEnd
            .Subscribe(_ => OnInteractionEnd.OnNext(Unit.Default))
            .AddTo(_elementSubscriptions);
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
}