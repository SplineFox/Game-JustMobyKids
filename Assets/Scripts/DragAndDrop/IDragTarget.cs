using System;
using UniRx;
using UnityEngine;

public interface IDragTarget
{
    IObservable<Unit> OnDragBegin { get; }
    IObservable<Unit> OnDragEnd { get; }

    bool CanBeDragged { get; set; }
    
    void OnGhostDragBegin();
    void OnGhostDragEnd();
    GameObject GetDraggableGhost();
    void ReleaseDraggableGhost(GameObject draggableGhost);
}