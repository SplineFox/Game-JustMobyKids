using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropController : MonoBehaviour
{
    [SerializeField] private RayProvider _rayProvider;
    [SerializeField] private RectTransform _dragContainer;
    [SerializeField, Min(0f)] private float _holdTimeToDrag = 0.25f;
    [SerializeField, Min(0f)] private float _holdAllowedOffset = 30f;
    
    private IDraggableSource _draggableSource;
    private GameObject _draggableGhost;
    
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
            _draggableSource = null;
            return;
        }
        
        _holdTime += Time.deltaTime;
        if (_holdTime < _holdTimeToDrag) 
            return;
        
        _holdTime = 0f;
        _isDragging = true;
        _isHolding = false;
        _draggableGhost = _draggableSource.GetDraggableGhost();
        _draggableGhost.transform.SetParent(_dragContainer);
    }

    private void OnPointerDown()
    {
        _startMousePosition = Input.mousePosition;
        var pointerData = new PointerEventData(EventSystem.current) 
        {
            position = _startMousePosition
        };
        
        if (_rayProvider.TryGetHit<IDraggableSource>(pointerData, out var draggable))
        {
            _draggableSource = draggable;
            _isHolding = true;
        }
    }
    
    private void OnPointerUp()
    {
        if (_isHolding)
        {
            _isHolding = false;
            _draggableSource = null;
            _holdTime = 0f;
        }
    }
    
    private void OnDrag()
    {
        _draggableGhost.transform.position = Input.mousePosition;
    }

    private void OnEndDrag()
    {
        _draggableSource.ReleaseDraggableGhost(_draggableGhost);
        _draggableSource = null;
        _draggableGhost = null;
        _isDragging = false;
    }
}