using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollArea : MonoBehaviour
{
    public List<DraggableCube> items = new();
    public ScrollRect scroll;

    private void Awake()
    {
        foreach (var item in items)
        {
            item.DragBegin += OnDragBegin;
            item.DragEnd += OnDragEnd;
        }
    }

    private void OnDestroy()
    {
        foreach (var item in items)
        {
            item.DragBegin -= OnDragBegin;
            item.DragEnd -= OnDragEnd;
        }
    }

    private void OnDragBegin()
    {
        scroll.enabled = false;
    }

    private void OnDragEnd()
    {
        scroll.enabled = true;
    }
}
