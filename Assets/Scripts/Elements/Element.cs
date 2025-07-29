using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Element : MonoBehaviour, IDragTarget
{
    public event Action DragBegin;
    public event Action DragEnd;
    
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _image;

    private ElementGhost _ghost;
    private ElementContainer _container;
    private ElementConfiguration _configuration;
    private Tween _tween;

    public RectTransform RectTransform => _rectTransform;
    public ElementConfiguration Configuration => _configuration;

    [Inject]
    public void Construct(ElementGhost ghost)
    {
        _ghost = ghost;
    }
    
    public void Initialize(ElementConfiguration configuration)
    {
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

    public GameObject GetDraggableGhost()
    {
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0.5f);
        _ghost.Show(this);
        DragBegin?.Invoke();
        return _ghost.gameObject;
    }

    public void ReleaseDraggableGhost(GameObject draggableGhost)
    {
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1f);
        DragEnd?.Invoke();
        _ghost.Hide();
    }
}