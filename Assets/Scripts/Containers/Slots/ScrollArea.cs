using System;
using System.Collections.Generic;
using Zenject;
using UnityEngine;
using UnityEngine.UI;

public class ScrollArea : MonoBehaviour, IInitializable, IDisposable
{
    [SerializeField] private ScrollRect _scrollRect;
    
    private GameConfiguration _gameConfiguration;
    private SlotFactory _slotFactory;
    
    private readonly List<Slot> _slots = new();

    [Inject]
    public void Construct(GameConfiguration gameConfiguration, SlotFactory slotFactory)
    {
        _gameConfiguration = gameConfiguration;
        _slotFactory = slotFactory;
    }
    
    public void Initialize()
    {
        foreach (var configuration in _gameConfiguration.Configurations)
        {
            var slot = _slotFactory.Create(configuration);
            
            slot.transform.SetParent(transform);
            slot.transform.localScale = Vector3.one;
            
            slot.ElementDragBegin += OnDragBegin;
            slot.ElementDragEnd += OnDragEnd;
            
            _slots.Add(slot);
        }
    }

    public void Dispose()
    {
        foreach (var slot in _slots)
        {
            slot.ElementDragBegin -= OnDragBegin;
            slot.ElementDragEnd -= OnDragEnd;
        }
    }

    private void OnDragBegin()
    {
        _scrollRect.enabled = false;
    }

    private void OnDragEnd()
    {
        _scrollRect.enabled = true;
    }
}