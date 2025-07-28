using System;
using Zenject;
using UnityEngine;

public class Slot : ElementContainer
{
    public Action ElementDragBegin;
    public Action ElementDragEnd;
    
    [SerializeField] private RectTransform _rectTransform;
    
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
        SpawnElement();
    }

    public override void AddElement(Element element)
    {
        _element = element;
        _element.SetContainer(this);
        
        _element.DragBegin += ElementDragBegin;
        _element.DragEnd += ElementDragEnd;
        
        _element.RectTransform.SetParent(_rectTransform);
        _element.transform.localScale = Vector3.one;
        
        _element.RectTransform.offsetMin = Vector2.zero;
        _element.RectTransform.offsetMax = Vector2.zero;
        
        _element.RectTransform.anchorMin = Vector2.zero;
        _element.RectTransform.anchorMax = Vector2.one;
    }

    public override void RemoveElement(Element element)
    {
        if (_element != element)
            return;
        
        _element.DragBegin -= ElementDragBegin;
        _element.DragEnd -= ElementDragEnd;

        SpawnElement();
    }

    private void SpawnElement()
    {
        var element = _elementPool.Spawn(_elementConfiguration);
        AddElement(element);
        element.PlayAppearAnimation();
    }
}