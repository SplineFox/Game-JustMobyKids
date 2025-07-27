using UnityEngine;

public interface IDropTarget
{
    bool CanDrop(DropEventData eventData);
    void OnDrop(DropEventData eventData);
}