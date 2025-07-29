using System;
using System.Collections.Generic;
using Zenject;
using UnityEngine;

public class Tower : ElementContainer, IInitializable, IDropTarget
{
    public event Action ElementDroppedOnMaxTower;
    public event Action ElementMissed;
    public event Action ElementAdded;
    
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RectTransform _dragTransform;
    
    private ITowerAnimator _animator;
    private ITowerDropValidator _dropValidator;
    private ITowerPlacementProvider _placementProvider;
    
    private TowerSave _save;
    private ElementPool _elementPool;
    private ElementConfigurationDatabase _elementConfigurations;
    
    private float _towerHeight;
    private List<Element> _elements = new();
    
    private bool IsMaxHeightReached => _towerHeight >= _rectTransform.rect.height;

    [Inject]
    public void Construct(ElementPool elementPool, ElementConfigurationDatabase elementConfigurations,
        ISaveProvider saveProvider)
    {
        _elementPool = elementPool;
        _save = saveProvider.GetSaveObject<TowerSave>("tower");
        
        _animator = new TowerAnimator();
        _dropValidator = new TowerDropValidator();
        _placementProvider = new TowerPlacementProvider();
    }

    public void Initialize()
    {
        foreach (var elementSave in _save.ElementsData)
        {
            var configuration = _elementConfigurations.GetConfiguration(elementSave.ConfigurationId);
            var element = _elementPool.Spawn(configuration);
            
            element.SetContainer(this);
            element.CanBeDestroyed = true;
            element.RectTransform.SetParent(_rectTransform);
            element.RectTransform.anchoredPosition = elementSave.AnchoredPosition;
            
            _elements.Add(element);
        }
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

        ElementAdded?.Invoke();
        RecalculateTowerHeight();
    }

    public override void RemoveElement(Element element)
    {
        var elementIndex = _elements.IndexOf(element);
        
        _animator.PlayRearrangeAnimation(_elements, elementIndex, () =>
        {
            _save.ElementsData.RemoveAt(elementIndex);
            _elements.RemoveAt(elementIndex);
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
            ElementDroppedOnMaxTower?.Invoke();
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
            AddElement(element);
            element.CanBeDragged = true;
            element.RectTransform.SetParent(_rectTransform, true);
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
            ElementMissed?.Invoke();
        });
    }

    private void RecalculateTowerHeight()
    {
        _towerHeight = 0f;

        foreach (var element in _elements)
            _towerHeight += element.RectTransform.rect.height;
    }
}
