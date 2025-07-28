using System;
using UnityEngine;
using Zenject;

public class Slot : MonoBehaviour
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
        _element = _elementPool.Spawn(_elementConfiguration);
        _element.RectTransform.SetParent(_rectTransform);
        _element.RectTransform.anchorMin = Vector2.zero;
        _element.RectTransform.anchorMax = Vector2.one;
    }
}