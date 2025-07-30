using System;
using Zenject;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Element : MonoBehaviour, IDragTarget
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _image;
    
    private readonly Subject<Unit> _onDragBegin = new();
    private readonly Subject<Unit> _onDragEnd = new();

    private ElementGhost _ghost;
    private ElementContainer _container;
    private ElementConfiguration _configuration;
    private Tween _tween;

    public bool CanBeDestroyed { get; set; }
    public bool CanBeDragged { get; set; }
    public IObservable<Unit> OnDragBegin => _onDragBegin;
    public IObservable<Unit> OnDragEnd => _onDragEnd;
    public RectTransform RectTransform => _rectTransform;
    public ElementConfiguration Configuration => _configuration;

    [Inject]
    public void Construct(ElementGhost ghost)
    {
        _ghost = ghost;
    }
    
    public void Initialize(ElementConfiguration configuration)
    {
        CanBeDragged = true;
        _configuration = configuration;
        _image.sprite = configuration.Sprite;
    }

    private void OnDestroy()
    {
        _tween?.Kill();
    }

    public void SetContainer(ElementContainer container)
    {
        if (_container == container)
            return;
        
        _container?.RemoveElement(this);
        _container = container;
    }

    public void OnGhostDragBegin()
    {
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0.5f);
        _onDragBegin.OnNext(Unit.Default);
    }

    public void OnGhostDragEnd()
    {
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1f);
        _onDragEnd.OnNext(Unit.Default);
    }

    public GameObject GetDraggableGhost()
    {
        _ghost.Show(this);
        return _ghost.gameObject;
    }

    public void ReleaseDraggableGhost(GameObject draggableGhost)
    {
        _ghost.Hide();
    }
    
    public void PlayAppearAnimation(Action onComplete = null)
    {
        _tween?.Kill();
        
        _rectTransform.localScale = Vector3.zero;
        _tween = _rectTransform.DOScale(Vector3.one, 1f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void PlayDisappearAnimation(Action onComplete = null)
    {
        _tween?.Kill();
        
        _rectTransform.localScale = Vector3.one;
        _tween = _rectTransform.DOScale(Vector3.zero, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() => onComplete?.Invoke());
    }
}