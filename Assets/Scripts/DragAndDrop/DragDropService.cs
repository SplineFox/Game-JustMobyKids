using System;
using UniRx;
using UnityEngine;
using Zenject;

public class DragDropService : MonoBehaviour, IInitializable, IDisposable
{
    [SerializeField] private DragHoldDetector _dragDetector;
    [SerializeField] private RayHitProvider _rayHitProvider;
    [SerializeField] private RectTransform _dragContainer;

    private readonly CompositeDisposable _disposables = new();
    private LayerMask _dragTargetLayer;
    private LayerMask _dropTargetLayer;

    private InputService _inputHandler;
    private IDragTarget _dragTarget;
    private GameObject _dragObject;
    private GameObject _dragGhost;
    private bool _isDragging;

    [Inject]
    public void Construct(InputService inputHandler)
    {
        _inputHandler = inputHandler;
    }

    public void Initialize()
    {
        _dragTargetLayer = LayerMask.NameToLayer("DragTarget");
        _dropTargetLayer = LayerMask.NameToLayer("DropTarget");

        _inputHandler.PointerDown
            .Subscribe(OnPointerDown)
            .AddTo(_disposables);

        _inputHandler.PointerMove
            .Subscribe(OnPointerMove)
            .AddTo(_disposables);

        _inputHandler.PointerUp
            .Subscribe(OnPointerUp)
            .AddTo(_disposables);

        _dragDetector.HoldCompleted
            .Subscribe(OnBeginDrag)
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables?.Dispose();
    }

    private void OnPointerDown(Vector2 position)
    {
        if (_rayHitProvider.TryGetHit(position, _dragTargetLayer, out var hitObject) &&
            hitObject.TryGetComponent<IDragTarget>(out var dragTarget) &&
            dragTarget.CanBeDragged)
        {
            _dragObject = hitObject;
            _dragTarget = dragTarget;

            _dragDetector.BeginHold(position);
        }
    }

    private void OnPointerMove(Vector2 position)
    {
        if (_isDragging)
        {
            OnUpdateDrag(position);
            return;
        }

        _dragDetector.UpdateHold(position);
    }

    private void OnPointerUp(Vector2 position)
    {
        if (_isDragging)
        {
            OnEndDrag(position);
            return;
        }

        _dragDetector.ResetHold();
    }

    private void OnBeginDrag(Vector2 position)
    {
        _dragGhost = _dragTarget.GetDraggableGhost();
        _dragTarget.OnGhostDragBegin();
        
        _dragGhost.transform.SetParent(_dragContainer);
        _dragGhost.transform.position = position;
        _dragGhost.transform.localScale = Vector3.one;
        _isDragging = true;
    }

    private void OnUpdateDrag(Vector2 position)
    {
        _dragGhost.transform.position = position;
    }

    private void OnEndDrag(Vector2 position)
    {
        var eventData = new DropEventData(position, _dragObject);

        _dragTarget.ReleaseDraggableGhost(_dragGhost);
        _dragTarget.OnGhostDragEnd();

        if (_rayHitProvider.TryGetHit(position, _dropTargetLayer, out var hitObject) &&
            hitObject.TryGetComponent<IDropTarget>(out var dropTarget))
        {
            dropTarget.OnDrop(eventData);
        }

        _dragGhost = null;
        _dragTarget = null;
        _dragObject = null;
        _isDragging = false;
    }
}