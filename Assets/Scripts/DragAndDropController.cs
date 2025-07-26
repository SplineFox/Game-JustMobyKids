using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropController : MonoBehaviour
{
    [SerializeField] private RayProvider _rayProvider;
    [SerializeField] private RectTransform _dragContainer;
    
    private bool _isDragging;
    
    private readonly CompositeDisposable _disposables = new();
    
    public void Awake()
    {
        Observable.EveryUpdate()
            .Where(x => Input.GetMouseButtonDown(0))
            .Subscribe(_ => OnPointerDown())
            .AddTo(_disposables);
        
        Observable.EveryUpdate()
            .Where(x => Input.GetMouseButtonUp(0))
            .Subscribe(_ => OnPointerUp())
            .AddTo(_disposables);
        
        Observable.EveryUpdate()
            .Where(_ => _isDragging)
            .Subscribe(_ => OnDrag())
            .AddTo(_disposables);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }

    private void OnPointerDown()
    {
        var pointerData = new PointerEventData(EventSystem.current) 
        {
            position = Input.mousePosition
        };
        
        if (_rayProvider.TryGetHit<IDraggableTarget>(pointerData, out var draggable))
        {
            Debug.Log("Hit");
        }
    }
    
    private void OnPointerUp()
    {
    }
    
    private void OnDrag()
    {
    }
}