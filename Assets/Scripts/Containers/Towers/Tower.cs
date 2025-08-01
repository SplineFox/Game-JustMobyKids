using System;
using UniRx;
using Zenject;
using UnityEngine;
using System.Collections.Generic;

public class Tower : ElementContainer, IInitializable, IDropTarget
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RectTransform _dragTransform;
    
    private readonly Subject<Unit> _onElementAdded = new();
    private readonly Subject<Unit> _onElementMissed = new();
    private readonly Subject<Unit> _onElementDroppedOnMaxTower = new();
    
    private ISaveProvider _saveProvider;
    private ITowerAnimator _animator;
    private ITowerDropValidator _dropValidator;
    private ITowerPlacementProvider _placementProvider;
    
    private TowerSave _save;
    private ElementPool _elementPool;
    private ElementConfigurationDatabase _elementConfigurations;
    
    private float _towerHeight;
    private List<Element> _elements = new();
    
    public IObservable<Unit> OnElementAdded => _onElementAdded;
    public IObservable<Unit> OnElementMissed => _onElementMissed;
    public IObservable<Unit> OnElementDroppedOnMaxTower => _onElementDroppedOnMaxTower;
    private bool IsMaxHeightReached => _towerHeight >= _rectTransform.rect.height;

    [Inject]
    public void Construct(ElementPool elementPool, ElementConfigurationDatabase elementConfigurations,
        ITowerAnimator animator, 
        ITowerDropValidator dropValidator, 
        ITowerPlacementProvider placementProvider,
        ISaveProvider saveProvider)
    {
        _elementPool = elementPool;
        _elementConfigurations = elementConfigurations;
        _saveProvider = saveProvider;
        
        _animator = animator;
        _dropValidator = dropValidator;
        _placementProvider = placementProvider;
    }

    public void Initialize()
    {
        _save = _saveProvider.GetSaveObject<TowerSave>("tower");

        foreach (var elementSave in _save.ElementsData)
        {
            var configuration = _elementConfigurations.GetConfiguration(elementSave.ConfigurationId);
            var element = _elementPool.Spawn(configuration);
            
            element.SetContainer(this);
            element.CanBeDestroyed = true;
            element.RectTransform.SetParent(_rectTransform);
            element.RectTransform.localScale = Vector3.one;
            element.RectTransform.anchoredPosition = elementSave.AnchoredPosition;
            
            _elements.Add(element);
        }
        RecalculateTowerHeight();
    }
    
    public override void AddElement(Element element)
    {
        element.SetContainer(this);
        element.CanBeDestroyed = true;
        
        _elements.Add(element);
        _save.ElementsData.Add(new TowerElementData
        {
            ConfigurationId = element.Configuration.Id,
            AnchoredPosition = element.RectTransform.anchoredPosition
        });

        _onElementAdded.OnNext(Unit.Default);
        RecalculateTowerHeight();
    }

    public override void RemoveElement(Element element)
    {
        var elementIndex = _elements.IndexOf(element);
        
        SetElementsDraggable(false);
        _animator.PlayRearrangeAnimation(_elements, elementIndex, () =>
        {
            _save.ElementsData.RemoveAt(elementIndex);
            _elements.RemoveAt(elementIndex);
            
            SetElementsDraggable(true);
            RefreshSavedPositions();
            RecalculateTowerHeight();
        });
    }

    public void OnDrop(DropEventData eventData)
    {
        if (_animator.IsAnimationPlaying ||
            !eventData.GameObject.TryGetComponent<Element>(out var element) ||
            _elements.Contains(element))
        {
            return;
        }

        if (IsMaxHeightReached)
        {
            _onElementDroppedOnMaxTower.OnNext(Unit.Default);
            return;
        }

        if (_dropValidator.CanDropElement(_elements, element, eventData.Position))
        {
            HandleDrop(element, eventData.Position);
            return;
        }
        
        HandleMissDrop(element, eventData.Position);
    }

    private void HandleDrop(Element element, Vector2 dropPosition)
    {
        var elementPosition = _placementProvider.GetPositionForElement(_elements, element, dropPosition, _rectTransform);
        
        element.CanBeDragged = false;
        element.RectTransform.SetParent(_dragTransform, true);
        _animator.PlayAddAnimation(element, elementPosition, () =>
        {
            element.RectTransform.SetParent(_rectTransform, true);
            element.CanBeDragged = true;
            AddElement(element);
        });
    }
    
    private void HandleMissDrop(Element element, Vector2 dropPosition)
    {
        element.SetContainer(null);
        element.CanBeDragged = false;
        element.RectTransform.SetParent(_dragTransform, true);
        _animator.PlayMissAnimation(element, dropPosition,() =>
        {
            _elementPool.Despawn(element);
            _onElementMissed.OnNext(Unit.Default);
        });
    }

    private void RecalculateTowerHeight()
    {
        _towerHeight = 0f;

        foreach (var element in _elements)
            _towerHeight += element.RectTransform.rect.height;
    }

    private void RefreshSavedPositions()
    {
        for (var index = 0; index < _save.ElementsData.Count; index++)
        {
            var elementData = _save.ElementsData[index];
            var element = _elements[index];
            
            elementData.AnchoredPosition = element.RectTransform.anchoredPosition;
        }
    }

    private void SetElementsDraggable(bool isDraggable)
    {
        foreach (var element in _elements)
            element.CanBeDragged = isDraggable;
    }
}
