using System;
using UnityEngine;
using Zenject;

public class Slot : MonoBehaviour
{
    public Action ElementDragBegin;
    public Action ElementDragEnd;
    
    private ElementConfiguration _elementConfiguration;
    private ElementPool _elementPool;

    [Inject]
    public void Construct(ElementConfiguration elementConfiguration, ElementPool elementPool)
    {
        _elementConfiguration = elementConfiguration;
        _elementPool = elementPool;
    }
}