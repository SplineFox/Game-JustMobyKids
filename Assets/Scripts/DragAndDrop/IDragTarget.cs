using System;
using UnityEngine;

public interface IDragTarget
{
    event Action DragBegin;
    event Action DragEnd;
    
    bool CanBeDragged { get; set; }
    
    GameObject GetDraggableGhost();
    void ReleaseDraggableGhost(GameObject draggableGhost);
}