using UnityEngine;

public interface IDropTarget
{
    bool CanDrop(IDragTarget dragTarget, Vector2 dropPosition);
    void OnDrop(IDragTarget dragTarget, Vector2 dropPosition);
}