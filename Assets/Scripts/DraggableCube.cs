using System;
using UnityEngine;
using UnityEngine.UI;

public class DraggableCube : MonoBehaviour, IDragTarget
{
    [SerializeField] private Image _image;

    public event Action DragBegin;
    public event Action DragEnd;

    public RectTransform RectTransform => _image.rectTransform;

    public GameObject GetDraggableGhost()
    {
        var ghost = Instantiate(this.gameObject);
        var ghostRectTransform = ghost.GetComponent<RectTransform>();
        ghostRectTransform.sizeDelta = Vector2.one * 180f;
        
        var image = ghost.GetComponent<Image>();
        image.raycastTarget = false;
        
        SetTransparent();
        DragBegin?.Invoke();
        return ghost;
    }

    public void ReleaseDraggableGhost(GameObject draggableGhost)
    {
        SetNormal();
        Destroy(draggableGhost);
        DragEnd?.Invoke();
    }

    private void SetTransparent()
    {
        var color = _image.color;
        color.a = 0.5f;
        
        _image.color = color;
    }

    private void SetNormal()
    {
        var color = _image.color;
        color.a = 1f;
        
        _image.color = color;
    }
}