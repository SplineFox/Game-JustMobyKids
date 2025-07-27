using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropController : MonoBehaviour
{
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
        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(_ => OnPointerDown())
            .AddTo(_disposables);
        
        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonUp(0))
            .Subscribe(_ => OnPointerUp())
            .AddTo(_disposables);
        
        Observable.EveryUpdate()
            .Where(_ => _isDragging)
            .Subscribe(_ => OnDrag())
            .AddTo(_disposables);
        
        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonUp(0) && _isDragging)
            .Subscribe(_ => OnEndDrag())
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
        OnBeginDrag();
    }

    private void OnPointerDown()
    {
        _startMousePosition = Input.mousePosition;
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
    
    private void OnPointerUp()
    {
        if (_isHolding)
        {
            _isHolding = false;
            _dragTarget = null;
            _holdTime = 0f;
        }
    }

    private void OnBeginDrag()
    {
        _isDragging = true;
        _dragGhost = _dragTarget.GetDraggableGhost();
        _dragGhost.transform.SetParent(_dragContainer);
    }
    
    private void OnDrag()
    {
        _dragGhost.transform.position = Input.mousePosition;
    }

    private void OnEndDrag()
    {
        var pointerData = new PointerEventData(EventSystem.current) 
        {
            position = Input.mousePosition
        };
        
        _dragTarget.ReleaseDraggableGhost(_dragGhost);
        _dragGhost = null;
        
        if (_rayProvider.TryGetHit<IDropTarget>(pointerData, out var dropTarget) && dropTarget.CanDrop(_dragTarget, Input.mousePosition))
        {
            dropTarget.OnDrop(_dragTarget, Input.mousePosition);
        }
        
        _dragTarget = null;
        _isDragging = false;
    }
}