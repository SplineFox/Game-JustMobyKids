using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tower : ElementContainer, IDropTarget
{
    public event Action ElementDroppedOnMaxTower;
    public event Action ElementMissed;
    public event Action ElementAdded;
    
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RectTransform _dragTransform;
    
    private ElementPool _elementPool;
    private List<Element> _elements = new();

    private float _towerHeight;
    private Sequence _sequence;
    
    public bool IsMaxHeightReached => _towerHeight >= _rectTransform.rect.height;

    [Inject]
    public void Construct(ElementPool elementPool)
    {
        _elementPool = elementPool;
    }

    private void OnDestroy()
    {
        _sequence?.Kill();
    }

    public override void AddElement(Element element)
    {
        _elements.Add(element);
        
        element.SetContainer(this);
        element.transform.localScale = Vector3.one;
        
        ElementAdded?.Invoke();
        
        RecalculateTowerHeight();
    }

    public override void RemoveElement(Element element)
    {
        var elementIndex = _elements.IndexOf(element);
        _elements.RemoveAt(elementIndex);
    }

    public void OnDrop(DropEventData eventData)
    {
        Debug.Log(eventData.Position);
        
        if (_sequence != null && _sequence.IsPlaying() ||
            !eventData.GameObject.TryGetComponent<Element>(out var element))
        {
            return;
        }

        if (IsMaxHeightReached)
        {
            ElementDroppedOnMaxTower?.Invoke();
            return;
        }

        if (_elements.Count == 0)
        {
            AddElement(element);
            element.RectTransform.SetParent(_dragTransform, true);
            DropFirst(element, eventData.Position);
            element.RectTransform.SetParent(_rectTransform, true);
            return;
        }

        if (IsCollidedWithAnyElement(eventData))
        {
            var lastElement = _elements.Last();
            AddElement(element);
            element.RectTransform.SetParent(_dragTransform, true);
            DropLast(element, lastElement);
            element.RectTransform.SetParent(_rectTransform, true);
            return;
        }
        
        element.SetContainer(null);
        element.RectTransform.SetParent(_dragTransform, true);
        element.RectTransform.position = eventData.Position;
        element.PlayDisappearAnimation(() =>
        {
            _elementPool.Despawn(element);
            ElementMissed?.Invoke();
        });
    }

    private bool IsCollidedWithAnyElement(DropEventData eventData)
    {
        foreach (var element in _elements)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(element.RectTransform, eventData.Position))
                return true;
        }
        
        return false;
    }
    
    private void DropFirst(Element element, Vector2 dropPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, dropPosition, null,
            out var localPosition
        );

        var bottomY = - _rectTransform.rect.height * 0.5f;
        
        var posX = localPosition.x;
        var posY = bottomY + element.RectTransform.rect.height * 0.5f;
        var posZ = element.RectTransform.position.z;
        var pos = new Vector3(posX, posY, posZ);

        element.RectTransform.position = _rectTransform.TransformPoint(pos);
    }

    private void DropLast(Element element, Element lastElement)
    {
        var lastHeight = lastElement.RectTransform.rect.height;
        
        var height = element.RectTransform.rect.height;
        var width = element.RectTransform.rect.height;

        var posXOffset = Random.Range(-1f, 1f) * (width * 0.5f);
        var posX = lastElement.RectTransform.localPosition.x + posXOffset;
        var posY = lastElement.RectTransform.localPosition.y + (lastHeight + height) * 0.5f;
        var posZ = lastElement.RectTransform.localPosition.z;
        var pos = new Vector3(posX, posY, posZ);
        
        element.RectTransform.position = _rectTransform.TransformPoint(pos);
    }

    private void RecalculateTowerHeight()
    {
        _towerHeight = 0f;

        foreach (var element in _elements)
            _towerHeight += element.RectTransform.rect.height;
    }
}
