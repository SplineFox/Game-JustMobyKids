using System;
using UnityEngine;

public interface IDragTarget
{
    event Action DragBegin;
    event Action DragEnd;
    
    GameObject GetDraggableGhost();
    void ReleaseDraggableGhost(GameObject draggableGhost);
}