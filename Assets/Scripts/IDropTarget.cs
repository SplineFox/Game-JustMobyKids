public interface IDropTarget
{
    bool CanDrop(IDraggableTarget draggable);
    void OnDrop(IDraggableTarget draggable);
}