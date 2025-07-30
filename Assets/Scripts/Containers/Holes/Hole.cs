using System;
using Zenject;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class Hole : MonoBehaviour, IDropTarget
{
    public readonly Subject<Unit> OnElementDestroyed = new();
    public readonly Subject<Unit> OnElementDestroyRejected = new();
    
    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private RectTransform _maskContainer;
    [SerializeField] private RectTransform _dragContainer;

    [SerializeField] private RectTransform _animationEndPoint;
    [SerializeField, Min(0f)] private float _animationDuration;
    [SerializeField, Min(0f)] private float _animationMaskTime;

    private ElementPool _elementPool;
    private Element _element;
    
    private CanvasScaleProvider _scaleProvider;
    private Sequence _sequence;

    [Inject]
    public void Construct(ElementPool elementPool, CanvasScaleProvider scaleProvider)
    {
        _elementPool = elementPool;
        _scaleProvider = scaleProvider;
    }

    private void OnDestroy()
    {
        _sequence?.Kill();
    }

    public void OnDrop(DropEventData eventData)
    {
        if (_element ||
            !_collider2D.OverlapPoint(eventData.Position) ||
            !eventData.GameObject.TryGetComponent<Element>(out var element))
        {
            return;
        }

        if (!element.CanBeDestroyed)
        {
            OnElementDestroyRejected.OnNext(Unit.Default);
            return;
        }

        _element = element;
        _element.SetContainer(null);
        _element.CanBeDragged = false;

        PlayDropAnimation(() =>
        {
            _elementPool.Despawn(_element);
            _element = null;
            OnElementDestroyed.OnNext(Unit.Default);
        });
    }

    private void PlayDropAnimation(Action onComplete)
    {
        _element.RectTransform.SetParent(_dragContainer, true);
        _element.RectTransform.localScale = Vector3.one;

        var jumpPower = _scaleProvider.Scale.y * 700f;

        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .Append(_element.RectTransform.DOJump(_animationEndPoint.position, jumpPower, 1, _animationDuration))
            .Insert(0f,
                _element.RectTransform.DOLocalRotate(Vector3.forward * 360f, _animationDuration,
                    RotateMode.FastBeyond360))
            .InsertCallback(_animationMaskTime, () => _element.RectTransform.SetParent(_maskContainer))
            .OnComplete(() => onComplete?.Invoke());
    }
}