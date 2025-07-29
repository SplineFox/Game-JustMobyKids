using System;
using Zenject;
using DG.Tweening;
using UnityEngine;

public class Hole : MonoBehaviour, IDropTarget
{
    public event Action ElementCantBeDestroyed;
    
    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private RectTransform _maskContainer;
    [SerializeField] private RectTransform _dragContainer;

    [SerializeField] private RectTransform _animationEndPoint;
    [SerializeField, Min(0f)] private float _animationDuration;
    [SerializeField, Min(0f)] private float _animationMaskTime;

    private ElementPool _elementPool;
    private Element _element;
    private Sequence _sequence;

    [Inject]
    public void Construct(ElementPool elementPool)
    {
        _elementPool = elementPool;
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
            ElementCantBeDestroyed?.Invoke();
            return;
        }

        _element = element;
        _element.SetContainer(null);
        _element.CanBeDragged = false;

        PlayDropAnimation(() =>
        {
            _elementPool.Despawn(_element);
            _element = null;
        });
    }

    private void PlayDropAnimation(Action onComplete)
    {
        _element.RectTransform.SetParent(_dragContainer, true);
        _element.RectTransform.localScale = Vector3.one;

        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .Append(_element.RectTransform.DOJump(_animationEndPoint.position, 700f, 1, _animationDuration))
            .Insert(0f,
                _element.RectTransform.DOLocalRotate(Vector3.forward * 360f, _animationDuration,
                    RotateMode.FastBeyond360))
            .InsertCallback(_animationMaskTime, () => _element.RectTransform.SetParent(_maskContainer))
            .OnComplete(() => onComplete?.Invoke());
    }
}