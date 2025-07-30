using System;
using System.Collections.Generic;
using UniRx;
using Zenject;
using UnityEngine;
using UnityEngine.UI;

public class ScrollArea : MonoBehaviour
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
    
    public void Start()
    {
        foreach (var configuration in _gameConfiguration.Configurations)
        {
            var slot = _slotFactory.Create(configuration);
            
            slot.transform.SetParent(transform);
            slot.transform.localScale = Vector3.one;
            
            slot.OnInteractionBegin.Subscribe(_ => _scrollRect.enabled = false).AddTo(this);
            slot.OnInteractionEnd.Subscribe(_ => _scrollRect.enabled = true).AddTo(this);
            
            _slots.Add(slot);
        }
    }
}