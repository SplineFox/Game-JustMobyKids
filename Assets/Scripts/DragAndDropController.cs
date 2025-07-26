using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropController : MonoBehaviour
{
    [SerializeField] private RayProvider _rayProvider;
    [SerializeField] private RectTransform _dragContainer;
    
    private IDraggableSource _draggableSource;
    private GameObject _draggableGhost;
    
    private bool _isWaitingDragging;
    private bool _isDragging;
    private float _timeToDrag;
    
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
        if (!_isWaitingDragging)
            return;
        
        _timeToDrag += Time.deltaTime;
        if (_timeToDrag < 1f) 
            return;
        
        _timeToDrag = 0f;
        _isDragging = true;
        _isWaitingDragging = false;
        _draggableGhost = _draggableSource.GetDraggableGhost();
        _draggableGhost.transform.SetParent(_dragContainer);
    }

    private void OnPointerDown()
    {
        var pointerData = new PointerEventData(EventSystem.current) 
        {
            position = Input.mousePosition
        };
        
        if (_rayProvider.TryGetHit<IDraggableSource>(pointerData, out var draggable))
        {
            _draggableSource = draggable;
            _isWaitingDragging = true;
        }
    }
    
    private void OnPointerUp()
    {
        if (_isWaitingDragging)
        {
            _isWaitingDragging = false;
            _draggableSource = null;
            _timeToDrag = 0f;
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