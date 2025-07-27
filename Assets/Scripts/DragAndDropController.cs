using UniRx;
using UnityEngine;

public class DragAndDropController : MonoBehaviour
{
    [SerializeField] private InputHandler _inputHandler;
    [SerializeField] private DragHoldDetector _dragDetector;
    [SerializeField] private RayProvider _rayProvider;
    [SerializeField] private RectTransform _dragContainer;
    
    private IDragTarget _dragTarget;
    private GameObject _dragGhost;
    private bool _isDragging;
    
    private readonly CompositeDisposable _disposables = new();
    
    private void Awake()
    {
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

    private void OnDestroy()
    {
        _disposables.Dispose();
    }

    private void OnPointerDown(Vector2 position)
    {
        if (_rayProvider.TryGetHit<IDragTarget>(position, out var dragTarget))
        {
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
        _isDragging = true;
        _dragGhost = _dragTarget.GetDraggableGhost();
        _dragGhost.transform.SetParent(_dragContainer);
        _dragGhost.transform.position = position;
    }
    
    private void OnUpdateDrag(Vector2 position)
    {
        _dragGhost.transform.position = position;
    }

    private void OnEndDrag(Vector2 position)
    {
        if (_rayProvider.TryGetHit<IDropTarget>(position, out var dropTarget) 
            && dropTarget.CanDrop(_dragTarget, position))
        {
            dropTarget.OnDrop(_dragTarget, position);
        }
        
        _dragTarget.ReleaseDraggableGhost(_dragGhost);
        _dragGhost = null;
        _dragTarget = null;
        _isDragging = false;
    }
}