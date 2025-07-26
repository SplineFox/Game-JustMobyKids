using System;
using UnityEngine;

public interface IDraggableSource
{
    event Action DragBegin;
    event Action DragEnd;
    
    GameObject GetDraggableGhost();
    void ReleaseDraggableGhost(GameObject draggableGhost);
}