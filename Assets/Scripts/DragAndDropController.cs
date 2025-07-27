using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropController : MonoBehaviour
{
    [SerializeField] private InputHandler _inputHandler;
    [SerializeField] private RayProvider _rayProvider;
    [SerializeField] private RectTransform _dragContainer;
    [SerializeField, Min(0f)] private float _holdTimeToDrag = 0.25f;
    [SerializeField, Min(0f)] private float _holdAllowedOffset = 30f;
    
    private IDragTarget _dragTarget;
    private GameObject _dragGhost;
    
    private bool _isHolding;
    private bool _isDragging;
    private float _holdTime;
    private Vector2 _startMousePosition;
    
    private readonly CompositeDisposable _disposables = new();
    
    public void Awake()
    {
        _inputHandler.PointerDown
            .Subscribe(OnPointerDown)
            .AddTo(_disposables);
        
        _inputHandler.PointerUp
            .Subscribe(OnPointerUp)
            .AddTo(_disposables);
        
        _inputHandler.PointerMove
            .Where(_ => _isDragging)
            .Subscribe(OnDrag)
            .AddTo(_disposables);
        
        _inputHandler.PointerUp
            .Where(_ => _isDragging)
            .Subscribe(OnEndDrag)
            .AddTo(_disposables);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }

    private void Update()
    {
        if (!_isHolding)
            return;
        
        var holdOffset = Vector2.Distance(_startMousePosition, Input.mousePosition);
        if (holdOffset > _holdAllowedOffset)
        {
            _holdTime = 0f;
            _isHolding = false;
            _dragTarget = null;
            return;
        }
        
        _holdTime += Time.deltaTime;
        if (_holdTime < _holdTimeToDrag) 
            return;
        
        _holdTime = 0f;
        _isHolding = false;
        OnBeginDrag(Input.mousePosition);
    }

    private void OnPointerDown(Vector2 position)
    {
        _startMousePosition = position;
        var pointerData = new PointerEventData(EventSystem.current) 
        {
            position = _startMousePosition
        };
        
        if (_rayProvider.TryGetHit<IDragTarget>(pointerData, out var dragTarget))
        {
            _dragTarget = dragTarget;
            _isHolding = true;
        }
    }
    
    private void OnPointerUp(Vector2 position)
    {
        if (_isHolding)
        {
            _isHolding = false;
            _dragTarget = null;
            _holdTime = 0f;
        }
    }

    private void OnBeginDrag(Vector2 position)
    {
        _isDragging = true;
        _dragGhost = _dragTarget.GetDraggableGhost();
        _dragGhost.transform.SetParent(_dragContainer);
        _dragGhost.transform.position = position;
    }
    
    private void OnDrag(Vector2 position)
    {
        _dragGhost.transform.position = position;
    }

    private void OnEndDrag(Vector2 pointerPosition)
    {
        var pointerData = new PointerEventData(EventSystem.current) 
        {
            position = pointerPosition
        };
        
        _dragTarget.ReleaseDraggableGhost(_dragGhost);
        _dragGhost = null;
        
        if (_rayProvider.TryGetHit<IDropTarget>(pointerData, out var dropTarget) && dropTarget.CanDrop(_dragTarget, pointerPosition))
        {
            dropTarget.OnDrop(_dragTarget, pointerPosition);
        }
        
        _dragTarget = null;
        _isDragging = false;
    }
}