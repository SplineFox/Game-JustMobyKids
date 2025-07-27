using System;
using UniRx;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public Vector2 PointerPosition { get; private set; }
    public IObservable<Vector2> PointerMove { get; private set; }
    public IObservable<Vector2> PointerDown { get; private set; }
    public IObservable<Vector2> PointerUp { get; private set; }

    private void Awake()
    {
        PointerMove = Observable.EveryUpdate()
            .Select(_ => (Vector2)Input.mousePosition)
            .DistinctUntilChanged()
            .Do(position => PointerPosition = position);
        
        PointerDown = Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Select(_ => PointerPosition);

        PointerUp = Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonUp(0))
            .Select(_ => PointerPosition);
    }
}
