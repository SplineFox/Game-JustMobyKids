using System;
using UnityEngine;

public interface IDragTarget
{
    event Action DragBegin;
    event Action DragEnd;
    
    RectTransform RectTransform { get; }
    
    GameObject GetDraggableGhost();
    void ReleaseDraggableGhost(GameObject draggableGhost);
}