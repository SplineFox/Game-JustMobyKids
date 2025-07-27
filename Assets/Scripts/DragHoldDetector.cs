using System;
using UniRx;
using UnityEngine;

public class DragHoldDetector : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _holdTimeToDrag = 0.25f;
    [SerializeField, Min(0f)] private float _holdAllowedOffset = 30f;
    
    private readonly Subject<Vector2> _holdCompletedSubject = new();

    private IDisposable _holdTimer;
    private Vector2 _holdStartPosition;
    private Vector2 _holdPosition;

    public IObservable<Vector2> HoldCompleted => _holdCompletedSubject;
    
    public void BeginHold(Vector2 position)
    {
        ResetHold();
        
        _holdStartPosition = position;
        _holdTimer = Observable.Timer(TimeSpan.FromSeconds(_holdTimeToDrag))
            .Subscribe(_ =>
            {
                _holdCompletedSubject.OnNext(_holdPosition);
                _holdTimer = null;
            })
            .AddTo(this);
    }

    public void UpdateHold(Vector2 position)
    {
        _holdPosition = position;
        if (_holdTimer == null)
            return;
        
        var holdOffset = Vector2.Distance(_holdStartPosition, _holdPosition);
        if (holdOffset < _holdAllowedOffset)
            return;
            
        ResetHold();
    }

    public void ResetHold()
    {
        _holdTimer?.Dispose();
        _holdTimer = null;
    }
}