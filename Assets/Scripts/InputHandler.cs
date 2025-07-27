using System;
using UniRx;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Vector2 _lastPointerPosition;
    
    public IObservable<Vector2> PointerMove { get; private set; }
    public IObservable<Vector2> PointerDown { get; private set; }
    public IObservable<Vector2> PointerUp { get; private set; }

    private void Awake()
    {
        PointerMove = Observable.EveryUpdate()
            .Select(_ => (Vector2)Input.mousePosition)
            .DistinctUntilChanged()
            .Do(position => _lastPointerPosition = position);
        
        PointerDown = Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Select(_ => _lastPointerPosition);

        PointerUp = Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonUp(0))
            .Select(_ => _lastPointerPosition);
    }
}
